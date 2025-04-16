using System;
using UnityEngine;

public class BeatAnalyzer : MonoBehaviour
{
    //Song beats per minute
    //This is determined by the song you're trying to sync up to
    public float songBpm;

    //The offset to the first beat of the song in seconds
    public float firstBeatOffset;

    //an AudioSource attached to this GameObject that will play the music.
    public AudioSource musicSource;

    public AudioClip musicClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Start the music
        //musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}