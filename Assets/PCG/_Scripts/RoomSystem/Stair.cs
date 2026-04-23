using UnityEngine;
using UnityEngine.SceneManagement;

public class Stair : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string baseSceneName = "StartScene";
    [SerializeField] private string nextFloorSceneName = "MainScene";

    [Header("Interaction")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private GameObject dialoguePanel;
    private bool playerInRange = false;
    private bool dialogueOpen = false;

    private void Awake()
    {
        
        Transform found = transform.Find("DialoguePanel");
        if (found != null)
        {
            
            dialoguePanel = found.gameObject;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey) && !dialogueOpen)
        {
            Debug.Log("123");
            ToggleDialogue(true);
        }
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
            ToggleDialogue(false);
        }
    }

    private void ToggleDialogue(bool visible)
    {
        dialogueOpen = visible;
        if (dialoguePanel != null)
            dialoguePanel.SetActive(visible);
    }

    public void OnChooseBase()
    {
        ToggleDialogue(false);
        SceneManager.LoadScene(baseSceneName);
    }

    public void OnChooseNextFloor()
    {
        ToggleDialogue(false);
        SceneManager.LoadScene(nextFloorSceneName);
    }

    public void OnChooseCancel()
    {
        ToggleDialogue(false);
    }
}
