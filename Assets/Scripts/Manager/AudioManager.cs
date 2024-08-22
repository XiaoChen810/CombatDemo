using System.IO;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource soundEffectSource;

    private void Awake()
    {
        // 单例模式
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

    private void Start()
    {
        if (!backgroundMusicSource.isPlaying)
        {
            PlayBackgroundMusic("Twilight Battle");
        }
    }

    // 播放背景音乐
    public void PlayBackgroundMusic(string musicName)
    {
        string path = Path.Combine("Music", musicName);
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip != null)
        {
            backgroundMusicSource.clip = clip;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }
        else
        {
            Debug.LogWarning("背景音乐未找到：" + path);
        }
    }

    // 播放一次音效
    public void PlaySoundEffect(string soundName)
    {
        string path = Path.Combine("Music", soundName);
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip != null)
        {
            soundEffectSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("音效未找到：" + path);
        }
    }
}
