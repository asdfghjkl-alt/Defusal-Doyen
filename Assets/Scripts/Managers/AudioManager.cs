using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;

    // Variable only here to check that there is only 1 Audio Manager
    static AudioManager instance;
    
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (Sound s in Sounds) {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.loop = s.loop;
                
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
            }

            PlaySound("Background Sound");
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    public void PlaySound(string SoundName)
    {
        foreach (Sound s in Sounds) {
            if (s.name == SoundName) {
                s.source.Play();
            }
        }
    }
}
