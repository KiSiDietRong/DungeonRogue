using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("AudioMixer")]
    public AudioMixer masterMixer;

    [Header("Volume Values")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    void Awake()
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

    void Start()
    {
        ApplyVolumes();
    }

    // Chuyển volume 0-1 sang dB để set vào mixer
    private float ToDecibel(float linear)
    {
        return Mathf.Log10(Mathf.Clamp(linear, 0.0001f, 1f)) * 20f;
    }

    private void ApplyVolumes()
    {
        masterMixer.SetFloat("MasterVol", ToDecibel(masterVolume));
        masterMixer.SetFloat("MusicVol", ToDecibel(musicVolume));
        masterMixer.SetFloat("SFXVol", ToDecibel(sfxVolume));
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        masterMixer.SetFloat("MasterVol", ToDecibel(value));
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        masterMixer.SetFloat("MusicVol", ToDecibel(value));
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        masterMixer.SetFloat("SFXVol", ToDecibel(value));
    }
}
