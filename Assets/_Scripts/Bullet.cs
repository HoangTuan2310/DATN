using UnityEngine;

/// <summary>
/// Fired by an enemy. Travels in a straight line, deals damage and applies
/// knockback to the player on contact, then destroys itself.
///
/// Setup:
///  - Attach to a GameObject with a Rigidbody2D and a trigger Collider2D.
///  - Call Launch(direction, owner) immediately after instantiation.
///  - Set the bullet's layer to match the enemy layer so Health.GetHit's
///    same-layer guard passes correctly.
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Travel speed in units per second.")]
    public float speed = 6f;

    [Tooltip("Damage dealt to the player.")]
    public int damage = 1;

    [Tooltip("Seconds before the bullet auto-destroys if it hits nothing.")]
    public float lifetime = 4f;

    // Set by whoever fires the bullet (the enemy GameObject).
    // Used as the 'sender' in GetHit so the layer check and knockback
    // direction are both correct.
    private GameObject owner;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Call this right after Instantiate to give the bullet its direction and owner.
    /// </summary>
    public void Launch(Vector2 direction, GameObject ownerObject)
    {
        owner = ownerObject;
        rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore the owner and all its children
        if (owner != null && other.transform.IsChildOf(owner.transform))
            return;

        // Search for Health on the hit collider or any of its parents
        Health health = other.GetComponent<Health>();
        if (health == null)
            health = other.GetComponentInParent<Health>();

        if (health != null)
        {
            GameObject sender = owner != null ? owner : gameObject;
            health.GetHit(damage, sender);

            // Apply knockback — check both the hit object and its parents
            KnockbackFeedback knockback = other.GetComponent<KnockbackFeedback>();
            if (knockback == null)
                knockback = other.GetComponentInParent<KnockbackFeedback>();
            if (knockback != null)
                knockback.PlayFeedback(sender);

            Destroy(gameObject);
            return;
        }

        // Destroy on hitting any solid obstacle (walls, foreground tiles)
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
}
