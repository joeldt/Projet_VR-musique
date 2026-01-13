using UnityEngine;

public class GlobalAudioPlayer : MonoBehaviour
{
    public static GlobalAudioPlayer Instance;

    private AudioSource audioSource;
    private AudioClip currentClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Play / Pause / Resume
    public void Toggle(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        // Même musique → Pause / Resume
        if (currentClip == clip)
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
            else
                audioSource.UnPause();

            return;
        }

        // Nouvelle musique
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
        currentClip = clip;
    }

    // Stop total
    public void Stop()
    {
        if (audioSource == null) return;

        audioSource.Stop();
        currentClip = null;
    }
}
