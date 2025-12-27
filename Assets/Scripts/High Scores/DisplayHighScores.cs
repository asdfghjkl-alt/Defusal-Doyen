using UnityEngine;
using TMPro;
using System.IO;
using System.Xml;

// Class present in High Scores page
public class HighScores : MonoBehaviour
{
    // Text for high scores
    [SerializeField] private TMP_Text[] HighScoreText;

    // Text for the banner
    [SerializeField] private TMP_Text BannerText;

    // Start is called before the first frame update
    void Start()
    {
        // Initially gets Easy Mode Scores
        GetHighScoreData("E");
    }

    public void GetHighScoreData(string mode) {
        if (mode == "E") {
            // If mode chosen is easy, then then change banner text
            BannerText.text = "High Scores (Easy)";
        } else if (mode == "H") {
            // If mode chosen is hard, then then change banner text
            BannerText.text = "High Scores (Hard)";
        }

        // Path of the High Scores File
        string path = Application.persistentDataPath + "/HighScoresData/HighScores" + mode + ".txt";

        // If file doesn't exist in that path
        if (!File.Exists(path)) {
            // Creates a directory
            Directory.CreateDirectory(Application.persistentDataPath + "/HighScoresData");
            
            // Makes the file in that path, and gives it default values of -1
            using (StreamWriter writer = new StreamWriter(path, false)) { 
                for (int i = 0; i < 5; i++) {
                    writer.WriteLine(-1);
                }
            }
        }

        // Reads the file
        using (StreamReader reader = new StreamReader(path)) {
            for (int i = 0; i < 5; i++) {
                // Reads the times in the file
                string time = reader.ReadLine();
                
                if (time == "-1") {
                    // This means that this slot is empty
                    HighScoreText[i].text = (i + 1).ToString() + ". NA";
                } else {
                    // Converts from string to float
                    float timeAsFloat = float.Parse(time);
                    
                    // Rounds the timer as an integer to find minutes and seconds
                    int roundedTimer = (int) timeAsFloat;

                    // Calculation to find minutes and seconds
                    int minutes = roundedTimer / 60;
                    int seconds = roundedTimer % 60;

                    // Displays the high scores on the screen
                    HighScoreText[i].text = (i + 1).ToString() + ". " + minutes.ToString() + "MIN " + seconds.ToString() + "S";
                }
            }
        }
    }
}
