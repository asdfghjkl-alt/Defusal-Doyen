using TMPro;
using UnityEngine;

// This is called in the End Game Win Screen
public class DisplayTime : MonoBehaviour
{
    // Reference to the Timer Text and assigned in Unity Editor
    // Allows setting text on the Timer Text
    [SerializeField] private TMP_Text TimerText;
 
    // Start is called before the first frame update
    void Start()
    {
        int roundedTimer = (int) StaticData.timer;

        // Calculation for minutes and seconds
        int minutes = roundedTimer / 60;
        int seconds = roundedTimer % 60;
        
        // Displaying the minutes and seconds
        TimerText.text = minutes.ToString() + "MIN " + seconds.ToString() + "S";
    }
}
