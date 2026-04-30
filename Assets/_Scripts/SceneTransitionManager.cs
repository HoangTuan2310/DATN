using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton that owns all scene transition logic.
/// Other systems (Stair, MainMenu, etc.) call into this instead of
/// touching SceneManager directly.
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "StartScene";
    [SerializeField] private string nextFloorSceneName = "MainScene";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>Load the main menu / base scene.</summary>
    public void GoToMainMenu()
    {
        LoadScene(mainMenuSceneName);
    }

    /// <summary>Load the next dungeon floor.</summary>
    public void GoToNextFloor()
    {
        LoadScene(nextFloorSceneName);
    }

    /// <summary>Load a scene by exact name.</summary>
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneTransitionManager] Cannot load scene: name is null or empty.");
            return;
        }

        Debug.Log($"[SceneTransitionManager] Loading scene: '{sceneName}'");
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>Reload whichever scene is currently active.</summary>
    public void ReloadCurrentScene()
    {
        string current = SceneManager.GetActiveScene().name;
        Debug.Log($"[SceneTransitionManager] Reloading current scene: '{current}'");
        SceneManager.LoadScene(current);
    }

    /// <summary>Quit the application (no-op in editor).</summary>
    public void QuitGame()
    {
        Debug.Log("[SceneTransitionManager] Quitting application.");
        Application.Quit();
    }
}
