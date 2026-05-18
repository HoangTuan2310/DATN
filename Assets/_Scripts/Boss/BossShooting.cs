using UnityEngine;

/// <summary>
/// Attach to the Boss GameObject.
/// Every <see cref="fireInterval"/> seconds, fires <see cref="bulletCount"/>
/// bullets in a fan aimed at the player. The centre of the fan always points
/// directly at the player; bullets spread evenly across <see cref="fanAngle"/> degrees.
/// </summary>
public class BossShooting : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Bullet prefab to instantiate (must have a Bullet component).")]
    [SerializeField] private GameObject bulletPrefab;

    [Tooltip("Tag used to locate the player.")]
    [SerializeField] private string playerTag = "Player";

    [Header("Firing pattern")]
    [Tooltip("Seconds between each volley.")]
    [SerializeField] private float fireInterval = 2f;

    [Tooltip("Total number of bullets per volley.")]
    [SerializeField][Min(1)] private int bulletCount = 5;

    [Tooltip("Total spread angle of the fan in degrees. " +
             "E.g. 60 spreads bullets across a 60-degree arc centred on the player.")]
    [SerializeField][Min(0f)] private float fanAngle = 60f;

    // ── private state ─────────────────────────────────────────────────

    private Transform player;
    private float timer;

    // ── lifecycle ─────────────────────────────────────────────────────

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("[BossShooting] Player not found. Check playerTag.", this);

        // Offset the first shot so it doesn't fire immediately at t=0
        timer = fireInterval;
    }

    private void Update()
    {
        if (player == null) return;
        if (PauseController.IsGamePaused) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = fireInterval;
            FireVolley();
        }
    }

    // ── shooting ──────────────────────────────────────────────────────

    private void FireVolley()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("[BossShooting] bulletPrefab is not assigned.", this);
            return;
        }

        // Direction from boss to player — this is the fan centre
        Vector2 toPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;

        // Angle of the centre direction in world space
        float centreAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        // When there is only one bullet, fire it straight at the player
        float startAngle = bulletCount > 1 ? centreAngle - fanAngle * 0.5f : centreAngle;
        float angleStep = bulletCount > 1 ? fanAngle / (bulletCount - 1) : 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet bullet = b.GetComponent<Bullet>();
            if (bullet != null)
                bullet.Launch(dir, gameObject);
        }
    }

#if UNITY_EDITOR
    // Visualise the fan arc in the Scene view when the Boss is selected
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || player == null) return;

        Vector2 toPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
        float centreAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        Gizmos.color = Color.red;
        DrawGizmoRay(centreAngle - fanAngle * 0.5f);
        DrawGizmoRay(centreAngle);
        DrawGizmoRay(centreAngle + fanAngle * 0.5f);
    }

    private void DrawGizmoRay(float angleDeg)
    {
        Vector2 dir = new Vector2(
            Mathf.Cos(angleDeg * Mathf.Deg2Rad),
            Mathf.Sin(angleDeg * Mathf.Deg2Rad));
        Gizmos.DrawRay(transform.position, (Vector3)dir * 3f);
    }
#endif
}