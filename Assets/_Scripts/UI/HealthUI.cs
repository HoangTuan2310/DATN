using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public int health;
    public int maxHealth;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Health playHealth;

    void Update()
    {
        health = playHealth.currentHealth;
        maxHealth = playHealth.maxHealth;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i<health) 
                hearts[i].sprite = fullHeart;
            else 
                hearts[i].sprite = emptyHeart;

            if (i < maxHealth)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
    }

    public void SetTarget(Health health)
    {
        playHealth = health;
    }
}