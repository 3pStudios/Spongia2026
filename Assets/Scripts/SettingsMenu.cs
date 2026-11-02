using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public AudioManager audioManager;
    public void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }
    public void OnChangedMusic(float volume)
    {
        if(audioManager) { audioManager.musicSource.volume = volume; }
    }
    public void OnChangedSFX(float volume)
    {
        if (audioManager) { audioManager.sfxSource.volume = volume; }
    }
}
