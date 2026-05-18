using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton that persists across scenes.
/// - Plays the correct BGM clip for each scene automatically.
/// - Scrollbar controls master volume (0-1).
/// - Toggle mutes / unmutes without losing the saved volume level.
/// </summary>
public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    [Header("UI References")]
    public Scrollbar volumeScrollbar;
    public Toggle muteToggle;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float defaultVolume = 1f;
    [Range(0f, 1f)]
    public float bgmVolume = 0.5f;

    [Header("BGM Per Scene")]
    public AudioClip startSceneBGM;
    public AudioClip mainSceneBGM;
    public AudioClip dungeonBGM;

    private AudioSource bgmSource;
    private float savedVolume;
    private bool isMuted = false;
    private bool wasPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = GetComponent<AudioSource>();
        if (bgmSource == null)
            bgmSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
        bgmSource.volume = bgmVolume;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        savedVolume = defaultVolume;
        AudioListener.volume = savedVolume;

        if (volumeScrollbar != null)
        {
            volumeScrollbar.value = savedVolume;
            volumeScrollbar.onValueChanged.AddListener(OnScrollbarChanged);
        }

        if (muteToggle != null)
        {
            muteToggle.isOn = true;
            muteToggle.onValueChanged.AddListener(OnToggleChanged);
        }

        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    private void PlayBGMForScene(string sceneName)
    {
        AudioClip clip = null;

        switch (sceneName)
        {
            case "StartScene":
                clip = startSceneBGM;
                break;
            case "MainScene":
                clip = mainSceneBGM;
                break;
            case "RoomContent":
            case "BossRoom":
                clip = dungeonBGM;
                break;
        }

        if (clip == null || bgmSource.clip == clip) return;

        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    private void OnScrollbarChanged(float value)
    {
        savedVolume = value;
        if (!isMuted)
            AudioListener.volume = savedVolume;

        if (isMuted && value > 0f)
        {
            isMuted = false;
            if (muteToggle != null)
                muteToggle.SetIsOnWithoutNotify(true);
            AudioListener.volume = savedVolume;
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        isMuted = !isOn;
        if (isMuted)
        {
            AudioListener.volume = 0f;
        }
        else
        {
            AudioListener.volume = savedVolume;
            if (volumeScrollbar != null)
                volumeScrollbar.SetValueWithoutNotify(savedVolume);
        }
    }

    public void SetVolume(float value)
    {
        value = Mathf.Clamp01(value);
        savedVolume = value;
        if (!isMuted)
            AudioListener.volume = savedVolume;
        if (volumeScrollbar != null)
            volumeScrollbar.SetValueWithoutNotify(savedVolume);
    }

    public void IncreaseVolume(float amount = 0.1f) => SetVolume(savedVolume + amount);
    public void DecreaseVolume(float amount = 0.1f) => SetVolume(savedVolume - amount);

public void ApplyAudioSaveData(float volume, bool muted)
    {
        savedVolume = Mathf.Clamp01(volume);
        isMuted = muted;
        AudioListener.volume = isMuted ? 0f : savedVolume;
        if (volumeScrollbar != null)
            volumeScrollbar.SetValueWithoutNotify(savedVolume);
        if (muteToggle != null)
            muteToggle.SetIsOnWithoutNotify(!isMuted);
    }

    public void Mute()       => muteToggle?.SetIsOnWithoutNotify(false);
    public void Unmute()     => muteToggle?.SetIsOnWithoutNotify(true);
    public void ToggleMute() { if (muteToggle != null) muteToggle.isOn = !muteToggle.isOn; }

public void GetAudioSaveData(out float volume, out bool muted)
    {
        volume = savedVolume;
        muted  = isMuted;
    }

}
