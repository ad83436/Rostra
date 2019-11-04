using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;
    public AudioClip worldMapMusic1;
    public AudioClip battleMusic1;
    public AudioClip bosseMusic1;
    public AudioClip victoryMusic1;
    public AudioClip defeatMusic1;

    public enum audioManagerState
    {
        notPlaying,
        playing,
        switching,
        muted
    }

    public audioManagerState currentState = audioManagerState.notPlaying;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        switch(currentState)
        {
            case audioManagerState.switching:
                break;
        }
    }
}
