using UnityEngine;

public class SoundManger : MonoBehaviour
{
    [HideInInspector]public static SoundManger Instance;

    [Header("AudioSource")]
    public AudioSource BGMSourse;
    public AudioSource ShotSourse;

    [Header("BGMClips")]
    public AudioClip[] BGMClips;
    private AudioClip currentClips;

    [Header("ShortClips")]
    public AudioClip[] ShortClips;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBGMMusic(0);
    }

    void Update()
    {
        
    }

    public void PlayBGMMusic(int index)
    {
        if(currentClips != BGMClips[index])
        { 
            BGMSourse.clip = BGMClips[index];
            BGMSourse.Play();
            currentClips = BGMClips[index];
        }
            
    }

    public void PlayShortMusic(int index)
    {
            ShotSourse.PlayOneShot(ShortClips[index]);
    }


}
