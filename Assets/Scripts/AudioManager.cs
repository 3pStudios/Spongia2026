//Spusta Hudbu a SFX cez samostatne tracky, aby sa dalo menit hlasitost samostatne


using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Tracks")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip cardPlaySFX;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip shieldSFX;
    [SerializeField] private AudioClip damageSFX;
    [SerializeField] private AudioClip buttonClickSFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Start menu" || scene.name == "Map")
        {
            PlayMusic(menuMusic);
        }
        else
        {
            PlayMusic(gameMusic);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayCard() => PlaySFX(cardPlaySFX);
    public void PlayHit() => PlaySFX(hitSFX);
    public void PlayShield() => PlaySFX(shieldSFX);
    public void PlayDamage() => PlaySFX(damageSFX);
    public void PlayButton() => PlaySFX(buttonClickSFX);

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }
}