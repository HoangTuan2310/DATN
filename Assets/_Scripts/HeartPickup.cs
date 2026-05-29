using UnityEngine;

/// <summary>
/// Add to the Heart prefab alongside ItemPickup.
/// When the player enters the collect radius, heals 1 currentHealth
/// (clamped to maxHealth) and destroys this object.
/// </summary>
public class HeartPickup : MonoBehaviour
{
    [Tooltip("Tag used to identify the player.")]
    public string playerTag = "Player";

    [Tooltip("Distance at which the heart is collected. Should match ItemPickup.collectRadius.")]
    [Min(0.01f)]
    public float collectRadius = 0.3f;

    private Transform _player;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            _player = playerObj.transform;
    }

    private void Update()
    {
        if (_player == null) return;

        if (Vector3.Distance(transform.position, _player.position) <= collectRadius)
        {
            Health health = _player.GetComponent<Health>();
            if (health != null && health.currentHealth < health.maxHealth)
            {
                health.currentHealth++;
            }
            Destroy(gameObject);
        }
    }
}
