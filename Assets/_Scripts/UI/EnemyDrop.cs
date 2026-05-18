using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to an enemy. When the enemy is destroyed, it spawns
/// Goldcoin and/or Heart prefabs based on configurable drop entries.
/// All settings are editable in the Unity Inspector.
/// </summary>
public class EnemyDrop : MonoBehaviour
{
    [System.Serializable]
    public class DropEntry
    {
        public GameObject prefab;
        [Range(0, 1)] public float dropChance = 0.5f;
        public int minQuantity = 1;
        public int maxQuantity = 1;
        public float scatterRadius = 0.5f;
    }

    public List<DropEntry> drops = new List<DropEntry>();

    public void DropLoot()
    {
        foreach (var entry in drops)
        {
            if (entry.prefab == null)
                continue;

            if (Random.value > entry.dropChance)
                continue;

            int count = Random.Range(
                entry.minQuantity,
                entry.maxQuantity + 1
            );

            for (int i = 0; i < count; i++)
            {
                Vector2 offset =
                    Random.insideUnitCircle * entry.scatterRadius;

                Vector3 pos =
                    transform.position +
                    new Vector3(offset.x, offset.y, 0);

                Instantiate(entry.prefab, pos, Quaternion.identity);
            }
        }
    }
}
