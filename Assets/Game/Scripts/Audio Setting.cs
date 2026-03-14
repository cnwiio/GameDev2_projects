using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    private void Start()
    {
        LoadVolumeSettings();
    }

    public void SetMasterVolume(float level)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("MasterVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("MusicVolume", level);
    }

    public void SetSFXVolume(float level)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("SFXVolume", level);
    }

    public void SetAmbientVolume(float level)
    {
        audioMixer.SetFloat("AmbientVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("AmbientVolume", level);
    }

    private void LoadVolumeSettings()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1f));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1f));
        SetAmbientVolume(PlayerPrefs.GetFloat("AmbientVolume", 1f));
    }
}