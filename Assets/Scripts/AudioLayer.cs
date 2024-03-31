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
        Debug.Log("changed audio level!");
        UpdateAudioVolume();
    }

    private void UpdateAudioVolume() 
    {
        audioSource.volume = baseVolume * AudioManager.getMultiplier(audioType);
    }
}

public static class AudioManager 
{
    public static event EventHandler OnAudioChanged;

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

    public static float getMultiplier(AudioType type) 
    {
        if (type == AudioType.Music)
        {
            return musicMultiplier;
        }
        else 
        {
            return sfxMultiplier;
        }
    }
}