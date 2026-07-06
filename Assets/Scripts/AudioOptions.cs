using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioOptions : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField]
    private AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField]
    private Slider bgmSlider;

    [SerializeField]
    private Slider sfxSlider;

    [SerializeField]
    private Slider voiceSlider;

    private const string BGMVolumeKey = "BGMVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string VoiceVolumeKey = "VoiceVolume";

    private void Start()
    {
        float savedBGMVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 0.1f);
        float savedSFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        float savedVoiceVolume = PlayerPrefs.GetFloat(VoiceVolumeKey, 1f);

        if (bgmSlider != null)
        {
            bgmSlider.value = savedBGMVolume;
            bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = savedSFXVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (voiceSlider != null)
        {
            voiceSlider.value = savedVoiceVolume;
            voiceSlider.onValueChanged.AddListener(SetVoiceVolume);
        }

        SetBGMVolume(savedBGMVolume);
        SetSFXVolume(savedSFXVolume);
        SetVoiceVolume(savedVoiceVolume);
    }

    public void SetBGMVolume(float volume)
    {
        SetMixerVolume("BGMVolume", volume);
        PlayerPrefs.SetFloat(BGMVolumeKey, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        SetMixerVolume("SFXVolume", volume);
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
        PlayerPrefs.Save();
    }

    public void SetVoiceVolume(float volume)
    {
        SetMixerVolume("VoiceVolume", volume);
        PlayerPrefs.SetFloat(VoiceVolumeKey, volume);
        PlayerPrefs.Save();
    }

    private void SetMixerVolume(string parameterName, float volume)
    {
        if (audioMixer == null)
            return;

        float safeVolume = Mathf.Clamp(volume, 0.0001f, 1f);
        float volumeInDecibels = Mathf.Log10(safeVolume) * 20f;

        audioMixer.SetFloat(parameterName, volumeInDecibels);
    }
}
