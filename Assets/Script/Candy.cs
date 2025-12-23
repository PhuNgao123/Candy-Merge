using UnityEngine;
using System;

public class Candy : MonoBehaviour
{
    public static event Action<int, Vector3> OnCandyMerged;
    // int = level mới, Vector3 = vị trí merge

    [Header("Candy info")]
    public string prefabPrefix = "Candy";

    private bool isMerging;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isMerging) return;
        if (!other.TryGetComponent(out Candy otherCandy)) return;
        if (otherCandy.isMerging) return;
        if (!IsSameCandy(otherCandy)) return;

        // chỉ 1 bên merge
        if (otherCandy.GetInstanceID() < GetInstanceID()) return;

        Merge(otherCandy);
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

        // 🔔 báo cho manager
        OnCandyMerged?.Invoke(newLevel, pos);

        Destroy(other.gameObject);
        Destroy(gameObject);
    }

    public bool TryGetLevel(out int level)
    {
        level = 1;
        return tag.Length > 1 && int.TryParse(tag.Substring(1), out level);
    }
}
