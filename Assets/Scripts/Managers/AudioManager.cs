using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    static AudioManager instance;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (Sound s in sounds) {
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
        foreach (Sound s in sounds) {
            if (s.name == SoundName) {
                s.source.Play();
            }
        }
    }
}
