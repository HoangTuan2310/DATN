using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalInteraction : MonoBehaviour, IInteractable
{
    public string playerTag = "Player";
    public string dungeonSceneName = "Dungeon";

    private bool _playerInRange = false;
    private DungeonDialogUI dialog;

    private void Awake()
    {
        dialog = Object.FindFirstObjectByType<DungeonDialogUI>();
    }

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (!_playerInRange || dialog == null) return;

        dialog.Show(EnterDungeon, CloseDialog);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInRange = false;
    }

    private void EnterDungeon()
    {
        SceneManager.LoadScene(dungeonSceneName);
    }

    private void CloseDialog() { }
}