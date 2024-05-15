using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Gets array of sounds (Assigned in Unity Editor)
    public Sound[] Sounds;

    // Variable only here to check that there is only 1 Audio Manager
    static AudioManager instance;
    
    // Start is called before the first frame update
    void Start()
    {
        // If audio manager is already generated
        if (instance == null) {
            // Sets this game object to be the instance to exist
            instance = this;

            // Doesn't destroy game object when transitioning scenes
            DontDestroyOnLoad(gameObject);


            foreach (Sound s in Sounds) {
                // Adds audio source to sound, and other attributes
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.loop = s.loop;
                
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
            }

            // Plays the background music on start
            PlaySound("Background Sound");
        } 
        else if (instance != this) {
            // If this is a duplicated Audio Manager, then destroy it
            Destroy(gameObject);
        }
    }
    public void PlaySound(string soundName)
    {
        // Loops through each of the sounds until finding Sound
        // with name soundName
        foreach (Sound s in Sounds) {
            if (s.name == soundName) {
                // Plays the sound
                s.source.Play();
            }
        }
    }

    public void StopSound(string soundName) {
        // Loops through each of the sounds until finding Sound
        // with name soundName
        foreach (Sound s in Sounds) {
            if (s.name == soundName) {
                // Stops the sound
                s.source.Stop();
            }
        }
    }
}
