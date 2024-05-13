using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    // Sprites for a muted and unmuted button
    [SerializeField] private Sprite MutedSprite;
    [SerializeField] private Sprite UnmutedSprite;

    // Gets image component of the button
    [SerializeField] private Image ImageRendRef;

    void Start() {
        // Changes sprite based on whether the sound is on or off
        if (StaticData.soundOn) {
            ImageRendRef.sprite = UnmutedSprite;
        } else {
            ImageRendRef.sprite = MutedSprite;
        }
    }

    public void ChangeBGMusic() {
        // Plays a click sound
        FindObjectOfType<AudioManager>().PlaySound("Click");

        // Whether sound is on is changed to the opposite value
        StaticData.soundOn = !StaticData.soundOn;

        if (StaticData.soundOn) {
            // If the sound is on, then play sound and change sprite
            FindObjectOfType<AudioManager>().PlaySound("Background Sound");
            ImageRendRef.sprite = UnmutedSprite;
        } else {
            // If sound off, then play sound and change sprite
            FindObjectOfType<AudioManager>().StopSound("Background Sound");
            ImageRendRef.sprite = MutedSprite;
        }
    }
}
