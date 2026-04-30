using UnityEngine;

/// <summary>
/// Attach to a Goldcoin / Heart prefab.
/// The item stays still until the player enters the attract radius,
/// then flies toward them and is collected (destroyed) once close enough.
/// All settings are editable in the Unity Inspector.
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [Header("Attraction")]
    [Tooltip("Tag used to find the player GameObject.")]
    public string playerTag = "Player";

    [Tooltip("The item only starts flying when the player comes within this distance.")]
    [Min(0f)]
    public float attractRadius = 2f;

    [Tooltip("Speed (units/sec) at which the item flies toward the player.")]
    [Min(0f)]
    public float flySpeed = 6f;

    [Tooltip("Acceleration applied each second while flying (set to 0 for constant speed).")]
    [Min(0f)]
    public float acceleration = 4f;

    [Header("Collection")]
    [Tooltip("Distance at which the item is considered collected and destroyed.")]
    [Min(0.01f)]
    public float collectRadius = 0.3f;

    [Tooltip("Seconds before the item auto-destroys itself if never collected (0 = never).")]
    [Min(0f)]
    public float lifetime = 12f;

    // ── private state ──────────────────────────────────────────────
    private Transform _player;
    private float _spawnTime;
    private float _currentSpeed;
    private bool  _attracting; // latches true once player enters attractRadius

    private void Start()
    {
        _spawnTime    = Time.time;
        _currentSpeed = flySpeed;

        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            _player = playerObj.transform;
    }

    private void Update()
    {
        // Auto-destroy by lifetime
        if (lifetime > 0f && Time.time - _spawnTime >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        if (_player == null) return;

        float dist = Vector3.Distance(transform.position, _player.position);

        // Collect
        if (dist <= collectRadius)
        {
            Destroy(gameObject);
            return;
        }

        // Latch attraction on once the player steps inside the attract radius.
        // Stays on even if the player moves away again.
        if (!_attracting && dist <= attractRadius)
            _attracting = true;

        if (_attracting)
        {
            _currentSpeed += acceleration * Time.deltaTime;
            Vector3 dir = (_player.position - transform.position).normalized;
            transform.position += dir * _currentSpeed * Time.deltaTime;
        }
    }

#if UNITY_EDITOR
    // Visualize radii in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }
#endif
}
