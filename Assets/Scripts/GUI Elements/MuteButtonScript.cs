using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    [SerializeField] private Sprite MutedSprite;
    [SerializeField] private Sprite UnmutedSprite;
    [SerializeField] private Image ImageRendRef;

    void Start() {
        if (StaticData.soundOn) {
            ImageRendRef.sprite = UnmutedSprite;
        } else {
            ImageRendRef.sprite = MutedSprite;
        }
    }

    public void ChangeBGMusic() {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        StaticData.soundOn = !StaticData.soundOn;

        if (StaticData.soundOn) {
            FindObjectOfType<AudioManager>().PlaySound("Background Sound");
            ImageRendRef.sprite = UnmutedSprite;
        } else {
            FindObjectOfType<AudioManager>().StopSound("Background Sound");
            ImageRendRef.sprite = MutedSprite;
        }
    }
}
