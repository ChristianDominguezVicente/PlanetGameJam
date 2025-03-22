using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Toggle screen;
    [SerializeField] private TMP_Dropdown quality;
    [SerializeField] private GameObject settingsMenu;

    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
            LoadMusic();
        else
            ChangeMusic();

        if (PlayerPrefs.HasKey("SFXVolume"))
            LoadSFX();
        else
            ChangeSFX();

        if (PlayerPrefs.HasKey("Screen"))
            LoadScreen();
        else
            FullScreen();

        if (PlayerPrefs.HasKey("Quality"))
            LoadQuality();
        else
            ChangeQuality();
    }

    public void FullScreen()
    {
        Screen.fullScreen = screen.isOn;
        if (screen.isOn == false)
            PlayerPrefs.SetInt("Screen", 1);
        else
            PlayerPrefs.SetInt("Screen", 0);
    }

    public void ChangeMusic()
    {
        float volumen = musicSlider.value;
        audioMixer.SetFloat("Music", Mathf.Log10(volumen) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volumen);
    }

    public void ChangeSFX()
    {
        float volumen = SFXSlider.value;
        audioMixer.SetFloat("SFX", Mathf.Log10(volumen) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volumen);
    }

    public void ChangeQuality()
    {
        int index = quality.value;
        PlayerPrefs.SetInt("Quality", index);
        QualitySettings.SetQualityLevel(index);
    }

    private void LoadMusic()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        ChangeMusic();
    }

    private void LoadSFX()
    {
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        ChangeSFX();
    }

    private void LoadScreen()
    {
        if (PlayerPrefs.GetInt("Screen") == 1)
            screen.isOn = false;
        else
            screen.isOn = true;
        FullScreen();
    }

    private void LoadQuality()
    {
        quality.value = PlayerPrefs.GetInt("Quality");
        ChangeQuality();
    }
}
