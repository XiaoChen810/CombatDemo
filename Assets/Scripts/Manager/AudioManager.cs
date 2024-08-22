using System.IO;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource soundEffectSource;

    private void Awake()
    {
        // ����ģʽ
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

    // ���ű�������
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
            Debug.LogWarning("��������δ�ҵ���" + path);
        }
    }

    // ����һ����Ч
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
            Debug.LogWarning("��Чδ�ҵ���" + path);
        }
    }
}
