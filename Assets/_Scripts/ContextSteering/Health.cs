using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Health : MonoBehaviour
{
    public int currentHealth, maxHealth;

    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;

    //public SpriteRenderer playerRenderer;

    [SerializeField]
    private bool isDead = false;

    void Start()
    {
        HealthUI ui = FindFirstObjectByType<HealthUI>();
        if (ui != null)
            ui.SetTarget(this);
    }

    public void InitializeHealth(int healthValue)
    {
        currentHealth = healthValue;
        maxHealth = healthValue;
        isDead = false;
    }

    public void GetHit(int amount, GameObject sender)
    {
        if (isDead)
            return;
        if (sender.layer == gameObject.layer)
            return;

        currentHealth -= amount;


        if (currentHealth > 0)
        {
            OnHitWithReference?.Invoke(sender);
        }
        else
        {
            OnDeathWithReference?.Invoke(sender);
            isDead = true;
            //playerRenderer.enabled = false;
            Destroy(gameObject);
        }
    }

private void OnDestroy()
    {
        if (isDead && EnemyManager.Instance != null)
        {
            EnemyManager.Instance.UnregisterEnemy(gameObject);
        }
    }

}
