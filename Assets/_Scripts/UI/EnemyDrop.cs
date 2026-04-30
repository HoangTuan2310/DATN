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
        [Tooltip("The prefab to drop (e.g. Goldcoin or Heart).")]
        public GameObject prefab;

        [Tooltip("Probability of this item dropping (0 = never, 1 = always).")]
        [Range(0f, 1f)]
        public float dropChance = 0.5f;

        [Tooltip("Minimum number of this item to drop when it triggers.")]
        [Min(0)]
        public int minQuantity = 1;

        [Tooltip("Maximum number of this item to drop when it triggers.")]
        [Min(1)]
        public int maxQuantity = 1;

        [Tooltip("Radius around the enemy in which items are scattered.")]
        public float scatterRadius = 0.5f;
    }

    [Header("Drop Table")]
    [Tooltip("Add one entry per droppable item (Goldcoin, Heart, etc.).")]
    public List<DropEntry> drops = new List<DropEntry>();

    // Call this from whatever destroys the enemy (e.g. a Health component).
    // If you simply Destroy(gameObject) you can also call it from OnDestroy.
    private void OnDestroy()
    {
        // Avoid spawning during editor/scene teardown
        if (!Application.isPlaying) return;

        foreach (DropEntry entry in drops)
        {
            if (entry.prefab == null) continue;

            if (Random.value <= entry.dropChance)
            {
                int count = Random.Range(entry.minQuantity, entry.maxQuantity + 1);
                for (int i = 0; i < count; i++)
                {
                    Vector2 offset = Random.insideUnitCircle * entry.scatterRadius;
                    Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0f);
                    Instantiate(entry.prefab, spawnPos, Quaternion.identity);
                }
            }
        }
    }
}
