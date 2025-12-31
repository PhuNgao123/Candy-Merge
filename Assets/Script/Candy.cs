using UnityEngine;
using System;
using System.Collections.Generic;

public class Candy : MonoBehaviour
{
    public static event Action<int, Vector3> OnCandyMerged;
    // int = level mới, Vector3 = vị trí merge
    public static event Action<List<Candy>,int> OnTripleMaxLevelDetected;

    [Header("Candy info")]
    public string prefabPrefix = "Candy";

    private bool isMerging;
    private List<Candy> touchingCandies = new List<Candy>();
    static public int maxLevel =0;

    private bool clusterConsumed = false;

    void Awake(){
        GameObject[] candyPrefabs;
        candyPrefabs= Resources.LoadAll<GameObject>("Prefab");

        if (candyPrefabs == null || candyPrefabs.Length == 0){
            Debug.LogWarning("Can not load Candy prefabs!");
            maxLevel=1;
        }else{
            maxLevel = candyPrefabs.Length;
        }
        
    }
   

    private void OnTriggerEnter2D(Collider2D other)
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.WakeUp();
        if(!other.TryGetComponent(out Candy otherCandy)) return;
        
        if (!touchingCandies.Contains(otherCandy))
        {
            touchingCandies.Add(otherCandy);
            
            
        }

        if (isMerging) return;
        if (otherCandy.isMerging) return;
        if (!IsSameCandy(otherCandy)) return;
        if (otherCandy.GetInstanceID() < GetInstanceID()) return;

        Merge(otherCandy);

        
    }
    private void OnTriggerStay2D(Collider2D other)
{
    if (!other.TryGetComponent(out Candy otherCandy)) return;
    if (!IsSameCandy(otherCandy)) return;

    CheckTripleMaxLevel();
}
    private void OnTriggerExit2D(Collider2D other)
    {
    if (other.TryGetComponent(out Candy otherCandy))
    {
        touchingCandies.Remove(otherCandy);
    }
    }

    private bool IsSameCandy(Candy other)
    {
        return tag == other.tag;
    }

    private void Merge(Candy other)
    {
        isMerging = true;
        other.isMerging = true;

        if (!TryGetLevel(out int level)) return;

        int newLevel = level + 1;
        string path = $"Prefab/{prefabPrefix}{newLevel}";
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null) return;

        Vector3 pos = (transform.position + other.transform.position) * 0.5f;
        Instantiate(prefab, pos, Quaternion.identity);

        OnCandyMerged?.Invoke(newLevel, pos);

        Destroy(other.gameObject);
        Destroy(gameObject);
    }

    public bool TryGetLevel(out int level)
    {
        level = 1;
        return tag.Length > 1 && int.TryParse(tag.Substring(1), out level);
    }

    public bool IsMaxLevel(){
        if (!TryGetLevel(out int myLevel)) return false;
        return myLevel == maxLevel;
    }

    void CheckTripleMaxLevel()
{
    if (!TryGetLevel(out int myLevel)) return;
    if (myLevel != maxLevel) return;

    List<Candy> cluster = new List<Candy>();
    cluster.Add(this);

    foreach (Candy candy in touchingCandies)
    {
        if (candy != null && candy.IsMaxLevel())
            cluster.Add(candy);
    }

    if (clusterConsumed) return;
if (cluster.Count < 3) return;

    Candy leader = cluster[0];
    foreach (Candy c in cluster)
    {
        if (c.GetInstanceID() < leader.GetInstanceID())
            leader = c;
    }

    if (leader != this) return;

    clusterConsumed = true;
OnTripleMaxLevelDetected?.Invoke(cluster, cluster.Count);
}

}
