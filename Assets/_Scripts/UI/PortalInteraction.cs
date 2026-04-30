using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attach to the Portal / Dungeon Entrance GameObject.
/// Subscribes to PlayerInput.OnInteract so the interact key is
/// configured once on the player, not here.
///
/// Fix applied: _playerInput is resolved in Awake (before OnEnable)
/// so the listener is always registered correctly on the first frame.
/// </summary>
public class PortalInteraction : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Tag of the player GameObject.")]
    public string playerTag = "Player";

    [Tooltip("Name of the dungeon scene to load (must be in Build Settings).")]
    public string dungeonSceneName = "Dungeon";

    [Tooltip("Show a press-key hint while the player is nearby.")]
    public bool showPromptHint = true;

    private bool _playerInRange = false;
    private DungeonDialogUI _dialog;
    private PlayerInput _playerInput;

    // Awake runs before OnEnable, so references are ready when we subscribe.
    private void Awake()
    {
        _dialog = Object.FindFirstObjectByType<DungeonDialogUI>();
        if (_dialog == null)
            Debug.LogWarning("[PortalInteraction] No DungeonDialogUI found in scene.");

        _playerInput = Object.FindFirstObjectByType<PlayerInput>();
        if (_playerInput == null)
            Debug.LogWarning("[PortalInteraction] No PlayerInput found in scene.");
    }

    private void OnEnable()
    {
        if (_playerInput != null)
            _playerInput.OnInteract.AddListener(TryOpenDialog);
    }

    private void OnDisable()
    {
        if (_playerInput != null)
            _playerInput.OnInteract.RemoveListener(TryOpenDialog);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInRange = true;
        //if (showPromptHint && _dialog != null)
        //    _dialog.ShowHint("Press [F] to enter dungeon");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        _playerInRange = false;
        //if (_dialog != null) _dialog.HideHint();
    }

    private void TryOpenDialog()
    {
        if (_playerInRange && _dialog != null)
            _dialog.Show(EnterDungeon, CloseDialog);
    }

    private void EnterDungeon()
    {
        SceneManager.LoadScene(dungeonSceneName);
    }

    private void CloseDialog() { }
}
