using UnityEngine;
using UnityEngine.UI;

public class PlayerPageUI : MonoBehaviour
{
    [Header("Heart display")]
    [SerializeField] private Transform heartContainer;
    [SerializeField] private GameObject heartTemplate;

    [Header("Strength display")]
    [SerializeField] private Transform strengthContainer;
    [SerializeField] private GameObject strengthTemplate;

    private void OnEnable() => Refresh();

    public void Refresh()
    {
        DrawHearts();
        DrawStrength();
    }

    private void DrawHearts()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;
        Health h = player.GetComponent<Health>();
        if (h == null) return;
        Fill(heartContainer, heartTemplate, h.maxHealth, "Heart");
    }

    private void DrawStrength()
    {
        WeaponParent wp = Object.FindFirstObjectByType<WeaponParent>();
        if (wp == null) return;
        Fill(strengthContainer, strengthTemplate, wp.damage, "Sword");
    }

    private static void Fill(Transform container, GameObject template, int count, string label)
    {
        if (container == null || template == null) return;
        if (count < 1) count = 1;
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);
            if (child.gameObject == template) continue;
            Destroy(child.gameObject);
        }
        template.SetActive(false);
        for (int i = 0; i < count; i++)
        {
            GameObject icon = Instantiate(template, container);
            icon.SetActive(true);
            icon.name = label + "_" + i;
        }
    }
}