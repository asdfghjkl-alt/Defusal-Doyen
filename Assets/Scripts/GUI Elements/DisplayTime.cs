using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayTime : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
 
    // Start is called before the first frame update
    void Start()
    {
        int roundedTimer = (int)StaticData.timer;

        // Calculation for minutes and seconds
        int minutes = roundedTimer / 60;
        int seconds = roundedTimer % 60;
        
        // Displaying the minutes and seconds
        timerText.text = minutes.ToString() + "MIN " + seconds.ToString() + "S";
    }
}
