using UnityEngine;
using System;
using System.Collections.Generic;

public class Candy : MonoBehaviour
{
    public static event Action<int, Vector3> OnCandyMerged;
    // int = level mới, Vector3 = vị trí merge
    public static event Action<List<Candy>,int> OnTripleMaxLevelDetected;

    public static GameObject[] GameOverLine;

    AudioManager audioManager;

    [Header("Candy info")]
    public string prefabPrefix = "Candy";

    private bool isMerging;
    private List<Candy> touchingCandies = new List<Candy>();
    static public int maxLevel =0;

    private bool clusterConsumed = false;
    [SerializeField] private float dropVelocityThreshold = 0.3f;

    private bool canPlayDropSFX = true;

    void Awake(){
        audioManager = AudioManager.Instance;

        if (CandyMergeManager.Prefabs == null)
    {
        Debug.LogError("Prefabs not loaded!");
        maxLevel = 1;
        return;
    }

    maxLevel = CandyMergeManager.Prefabs.Length;
        
    }
   

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
    if (rb == null) return;

    
    if (canPlayDropSFX && rb.linearVelocity.y < -dropVelocityThreshold)
    {
        audioManager.PlaySFX(audioManager.candyCollision);
        canPlayDropSFX = false; 
    }
        if(!other.TryGetComponent(out Candy otherCandy)) return;
        
        if (!touchingCandies.Contains(otherCandy))
        {
            touchingCandies.Add(otherCandy);
            CheckTripleMaxLevel();
            
        }

        if (isMerging) return;
        if (otherCandy.isMerging) return;
        if (!IsSameCandy(otherCandy)) return;
        if (otherCandy.GetInstanceID() < GetInstanceID()) return;

        Merge(otherCandy);

        
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
        
        if (CandyMergeManager.Prefabs == null || newLevel - 1 >= CandyMergeManager.Prefabs.Length)
        {
            Debug.LogWarning($"Cannot merge to level {newLevel}: Prefab not found or out of bounds");
            return;
        }
        Destroy(other.gameObject);
        Destroy(gameObject);

        GameObject prefab = CandyMergeManager.Prefabs[newLevel - 1];
        if (prefab == null) return;

        Vector3 pos = (transform.position + other.transform.position) * 0.5f;
        Instantiate(prefab, pos, Quaternion.identity);

        OnCandyMerged?.Invoke(newLevel, pos);

        
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
    if (clusterConsumed) return;
    if (!IsMaxLevel()) return;

    List<Candy> cluster = new List<Candy>();
    Queue<Candy> queue = new Queue<Candy>();

    cluster.Add(this);
    queue.Enqueue(this);

    while (queue.Count > 0)
    {
        Candy current = queue.Dequeue();

        foreach (Candy neighbor in current.touchingCandies)
        {
            if (neighbor == null) continue;
            if (!neighbor.IsMaxLevel()) continue;
            if (cluster.Contains(neighbor)) continue;

            cluster.Add(neighbor);
            queue.Enqueue(neighbor);
        }
    }

    if (cluster.Count < 3) return;

    // leader = smallest instanceID 
    Candy leader = cluster[0];
    foreach (Candy c in cluster)
        if (c.GetInstanceID() < leader.GetInstanceID())
            leader = c;

    if (leader != this) return;

    clusterConsumed = true;
    OnTripleMaxLevelDetected?.Invoke(cluster, cluster.Count);
}
}
