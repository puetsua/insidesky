#define MUSIC_CONTROLLER_DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMusicController
{
    void NextLevel();
    void PrevLevel();
    int GetCurrentLevel();
}

public class MusicController : MonoBehaviour, IMusicController
{
    [SerializeField]
    AudioClip[] musicClips;
    [SerializeField, Range(0.01f, 2f)]
    float trainsitionSpeed = 1;
    
    List<AudioSource> audioSources = new List<AudioSource>();


    public void NextLevel()
    {
        CurrentIndex = Mathf.Min(CurrentIndex + 1, musicClips.Length);
    }

    public void PrevLevel()
    {
        CurrentIndex = Mathf.Max(CurrentIndex - 1, 0);
    }

    public int GetCurrentLevel()
    {
        return CurrentIndex;
    }

    public int CurrentIndex
    {
        get; 
        private set;
    }

    void Start()
    {
        CurrentIndex = 0;

        if(musicClips.Length == 0)
        {
            Debug.Log("Music Controller need audio clip");
            this.enabled = false;
        }

        foreach(var musicClip in musicClips)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = musicClip;
            audioSource.playOnAwake = true;
            audioSource.loop = true;
            audioSource.volume = 0;
            audioSource.Play();
            audioSources.Add(audioSource);
        }
    }

    void Update()
    {
        DebugHandler();
        
        for(int i = 0; i < audioSources.Count; i++)
        {
            var audioSource = audioSources[i];

            if(i == CurrentIndex)
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, 1, Time.deltaTime * trainsitionSpeed);
                audioSource.volume = Mathf.Ceil(audioSource.volume * 1000) / 1000;
            }
            else
            {
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime * trainsitionSpeed);
                audioSource.volume = Mathf.Floor(audioSource.volume * 1000) / 1000;
            }

        }
        
    }

    void DebugHandler()
    {
        #if MUSIC_CONTROLLER_DEBUG
            if(Input.GetKeyDown(KeyCode.F7))
                PrevLevel();
            
            if(Input.GetKeyDown(KeyCode.F8))
                NextLevel();
        #endif
    }
}
