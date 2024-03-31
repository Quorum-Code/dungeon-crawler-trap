using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] Toggle musicToggle;
    [SerializeField] Slider musicSlider;
    [SerializeField] TMP_Text musicText;

    [SerializeField] Toggle sfxToggle;
    [SerializeField] Slider sfxSlider;
    [SerializeField] TMP_Text sfxText;

    public void OnEnable()
    {
        // Correct bars and toggles
        musicToggle.isOn = AudioManager.isMusicOn;
        musicSlider.value = AudioManager.getMultiplier(AudioType.Music);
        musicText.text = musicSlider.value.ToString("F2");

        sfxToggle.isOn = AudioManager.isMusicOn;
        sfxSlider.value = AudioManager.getMultiplier(AudioType.SFX);
        sfxText.text = sfxSlider.value.ToString("F2");
    }

    private void FixedUpdate()
    {
        musicText.text = musicSlider.value.ToString("F2");
        sfxText.text = sfxSlider.value.ToString("F2");
    }

    public void CloseSettings() 
    {
        int x = SceneManager.GetActiveScene().buildIndex;

        if (x != 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        gameObject.SetActive(false);
    }

    public void SetMusicVolume(float f) 
    {
        AudioManager.setMusicMutliplier(f);
    }

    public void SetSFXVolume(float f) 
    {
        AudioManager.setSFXMultiplier(f);
    }

    public void ToggleMusicOn() 
    {
        AudioManager.setMusicOn(!AudioManager.isMusicOn);
    }

    public void ToggleSFXOn() 
    {
        AudioManager.setSFXOn(!AudioManager.isSFXOn);
    }
}
