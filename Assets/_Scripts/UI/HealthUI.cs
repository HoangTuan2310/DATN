using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private Health playHealth;

    void Update()
    {
        if (playHealth == null) return;

        int currentHealth = playHealth.currentHealth;
        int maxHealth = playHealth.maxHealth;

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < currentHealth ? fullHeart : emptyHeart;
            //hearts[i].enabled = i < maxHealth;
        }
    }

    public void SetTarget(Health health)
    {
        playHealth = health;
    }

public void NotifyPlayerDied()
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].sprite = emptyHeart;
        playHealth = null;
    }

void Awake()
    {
        if (hearts == null || hearts.Length == 0)
        {
            var parent = transform.parent;
            if (parent != null)
            {
                var found = new System.Collections.Generic.List<Image>();
                for (int i = 0; i < parent.childCount; i++)
                {
                    var child = parent.GetChild(i);
                    if (child == transform) continue;
                    var img = child.GetComponent<Image>();
                    if (img != null)
                        found.Add(img);
                }
                hearts = found.ToArray();
            }
        }
    }
}