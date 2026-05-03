using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DialogueAudio : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true; // loop khi đang type
    }

    public void StartTypingSound()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopTypingSound()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}