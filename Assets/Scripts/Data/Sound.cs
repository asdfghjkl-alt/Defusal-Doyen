using UnityEngine;

[System.Serializable]
public class Sound {
    // Variable for actual sound file (Unity Engine Class)
    public AudioClip clip;

    // Allows an identifier for what the clip is named
    public string name;
    
    // Tells the program whether the audio should loop
    public bool loop;
    
    // Tells the program at which volume should the sound be played
    [Range(0f, 2f)] public float volume;

    // Tells the program at which pitch should the sound be played
    [Range(0.1f, 3f)] public float pitch;

    // Gives the class the ability to play the sound
    // through Unity's default class (AudioSource)
    [HideInInspector] public AudioSource source;
}