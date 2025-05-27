using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); } else { Destroy(gameObject); }
    }

    private void Start()
    {
        rf_PlayMusic("PenguinBallad");
    }

    public void rf_PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.soundName == name);

        if (s == null) { Debug.LogWarning("Sound not found"); }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
    public void rf_PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.soundName == name);

        if (s == null) { Debug.LogWarning("Sound not found"); }
        else
        {
            
            sfxSource.pitch = UnityEngine.Random.Range(0.85f, 1.15f);
            sfxSource.PlayOneShot(s.clip);
        }
    }
}