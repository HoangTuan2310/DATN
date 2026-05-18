using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Health : MonoBehaviour
{
    public int currentHealth, maxHealth;

    public UnityEvent<GameObject> OnHitWithReference, OnDeathWithReference;

    //public SpriteRenderer playerRenderer;

    [SerializeField]
    private bool isDead = false;

void Start()
    {
        if (!CompareTag("Player")) return;
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
            currentHealth = 0;
            isDead = true;

            GetComponent<EnemyDrop>()?.DropLoot();

            OnDeathWithReference?.Invoke(sender);

            if (CompareTag("Player"))
            {
                HealthUI ui = FindFirstObjectByType<HealthUI>();
                if (ui != null)
                    ui.NotifyPlayerDied();
            }

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
