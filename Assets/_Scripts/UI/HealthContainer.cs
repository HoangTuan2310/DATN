using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the HealthContainer GameObject.
/// Spawns one HealthImage prefab per point of the player's maxHealth,
/// then keeps the displayed sprites in sync with currentHealth each frame.
/// </summary>
public class HealthContainer : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject healthImagePrefab;

    [Header("Sprites")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private Health playerHealth;
    private Image[] heartImages;

    // ------------------------------------------------------------------ //
    // Public API called by Health.cs
    // ------------------------------------------------------------------ //

    public void SetTarget(Health health)
    {
        playerHealth = health;
        BuildHearts(health.maxHealth);
    }

    public void NotifyPlayerDied()
    {
        if (heartImages == null) return;
        foreach (var img in heartImages)
            if (img != null) img.sprite = emptyHeart;
        playerHealth = null;
    }

    // ------------------------------------------------------------------ //
    // Internal
    // ------------------------------------------------------------------ //

    private void BuildHearts(int maxHealth)
    {
        // Clear any existing children (editor placeholders or previous run)
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        heartImages = new Image[maxHealth];

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject go = Instantiate(healthImagePrefab, transform);
            go.name = "Heart_" + i;
            heartImages[i] = go.GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (playerHealth == null || heartImages == null) return;

        int current = playerHealth.currentHealth;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
                heartImages[i].sprite = i < current ? fullHeart : emptyHeart;
        }
    }
}
