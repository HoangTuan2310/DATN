using UnityEngine;
using UnityEngine.UI;

public class HealthUIBinder : MonoBehaviour
{
    public Image[] hearts;

    void Start()
    {
        HealthUI playerUI = FindFirstObjectByType<HealthUI>();
        if (playerUI != null)
        {
            playerUI.hearts = hearts;
        }
    }
}
