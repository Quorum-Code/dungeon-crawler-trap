using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AudioType 
{
    None,
    Music,
    SFX
}

public class AudioLayer : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] public AudioType audioType;
    [SerializeField] public float baseVolume;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        AudioManager.OnAudioChanged += OnAudioChanged;
        UpdateAudioVolume();
    }

    private void OnAudioChanged(object sender, EventArgs e) 
    {
        UpdateAudioVolume();
    }

    private void UpdateAudioVolume() 
    {
        audioSource.volume = baseVolume * AudioManager.getMultiplier(audioType);
    }

    private void OnDestroy()
    {
        AudioManager.OnAudioChanged -= OnAudioChanged;
    }
}

public static class AudioManager 
{
    public static event EventHandler OnAudioChanged;

    public static bool isMusicOn { get; private set; } = true;
    public static bool isSFXOn { get; private set; } = true;

    public static float musicMultiplier { get; private set; } = 0.1f;
    public static float sfxMultiplier { get; private set; } = 0.1f;


    public static void setMusicMutliplier(float multi) 
    {
        multi = Mathf.Clamp01(multi);
        musicMultiplier = multi;

        if (OnAudioChanged != null)
            OnAudioChanged(null, EventArgs.Empty);
    }

    public static void setSFXMultiplier(float multi) 
    {
        multi = Mathf.Clamp01(multi);
        sfxMultiplier = multi;

        if (OnAudioChanged != null)
            OnAudioChanged(null, EventArgs.Empty);
    }

    public static void setMusicOn(bool isOn) 
    {
        isMusicOn = isOn;
        OnAudioChanged(null, EventArgs.Empty);
    }

    public static void setSFXOn(bool isOn) 
    {
        isSFXOn = isOn;
        OnAudioChanged(null, EventArgs.Empty);
    }

    public static float getMultiplier(AudioType type) 
    {
        if (type == AudioType.Music)
        {
            if (isMusicOn)
                return musicMultiplier;
            else
                return 0f;
        }
        else 
        {
            if (isSFXOn)
                return sfxMultiplier;
            else
                return 0f;
        }
    }
}