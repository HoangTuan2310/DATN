using UnityEngine;
using UnityEngine.UI;

public class Stair : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private GameObject dialoguePanel;
    private GameObject warningPanel;
    private bool playerInRange = false;
    private bool dialogueOpen = false;

    private void Awake()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "DialoguePanel")
                dialoguePanel = child.gameObject;
            else if (child.name == "WarningPanel")
                warningPanel = child.gameObject;
        }

        if (dialoguePanel == null)
        {
            Debug.LogError("[Stair] DialoguePanel child not found! Check the Stair prefab hierarchy.", this);
        }
        else
        {
            Canvas canvas = dialoguePanel.GetComponent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                Debug.Log("[Stair] DialoguePanel Canvas switched to ScreenSpaceOverlay.");
            }

            if (dialoguePanel.GetComponent<GraphicRaycaster>() == null)
                dialoguePanel.AddComponent<GraphicRaycaster>();

            Button[] buttons = dialoguePanel.GetComponentsInChildren<Button>(true);

            if (buttons.Length > 0)
            {
                buttons[0].onClick.RemoveAllListeners();
                buttons[0].onClick.AddListener(OnMainMenuButton);
            }
            if (buttons.Length > 1)
            {
                buttons[1].onClick.RemoveAllListeners();
                buttons[1].onClick.AddListener(OnNextFloorButton);
            }
            if (buttons.Length > 2)
            {
                buttons[2].onClick.RemoveAllListeners();
                buttons[2].onClick.AddListener(OnCloseButton);
            }

            Debug.Log($"[Stair] Wired {buttons.Length} button(s) in DialoguePanel.");
            dialoguePanel.SetActive(false);
        }

        if (warningPanel == null)
        {
            Debug.LogWarning("[Stair] WarningPanel child not found!", this);
        }
        else
        {
            Canvas warnCanvas = warningPanel.GetComponent<Canvas>();
            if (warnCanvas != null && warnCanvas.renderMode == RenderMode.WorldSpace)
                warnCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            if (warningPanel.GetComponent<GraphicRaycaster>() == null)
                warningPanel.AddComponent<GraphicRaycaster>();

            warningPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && !dialogueOpen && Input.GetKeyDown(interactKey))
            TryOpenDialogue();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            CloseDialogue();
        }
    }

    private void TryOpenDialogue()
    {
        bool allDead = EnemyManager.Instance == null || EnemyManager.Instance.IsAllEnemiesDead;
        if (!allDead)
        {
            ShowWarning();
            return;
        }
        OpenDialogue();
    }

    private void OpenDialogue()
    {
        if (dialoguePanel == null) return;
        dialoguePanel.SetActive(true);
        dialogueOpen = true;
        Debug.Log("[Stair] Dialogue opened.");
    }

    private void CloseDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (warningPanel != null) warningPanel.SetActive(false);
        dialogueOpen = false;
        Debug.Log("[Stair] Dialogue closed.");
    }

    private void OnCloseButton() => CloseDialogue();

    private void ShowWarning()
    {
        if (warningPanel == null) return;
        warningPanel.SetActive(true);
        CancelInvoke(nameof(HideWarning));
        Invoke(nameof(HideWarning), 2f);
        Debug.Log("[Stair] Warning: enemies still alive.");
    }

    private void HideWarning()
    {
        if (warningPanel != null) warningPanel.SetActive(false);
    }

    private void OnMainMenuButton()
    {
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.GoToMainMenu();
        else
            Debug.LogError("[Stair] SceneTransitionManager not found in scene!");
    }

    private void OnNextFloorButton()
    {
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.GoToNextFloor();
        else
            Debug.LogError("[Stair] SceneTransitionManager not found in scene!");
    }}
