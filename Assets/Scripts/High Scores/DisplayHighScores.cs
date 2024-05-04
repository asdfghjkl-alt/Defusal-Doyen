using UnityEngine;
using TMPro;
using System.IO;

public class HighScores : MonoBehaviour
{
    [SerializeField] private TMP_Text[] HighScoreText;

    // Start is called before the first frame update
    void Start()
    {
        string path = Application.dataPath + "/HighScoresData/HighScores.txt";

        if (!File.Exists(path)) {
            Directory.CreateDirectory(Application.dataPath + "/HighScoresData");
            
            using (StreamWriter writer = new StreamWriter(path, false)) { 
                for (int i = 0; i < 5; i++) {
                    writer.WriteLine(-1);
                }
            }
        }

        Random.InitState((int) System.DateTime.Now.Ticks);

        using (StreamReader reader = new StreamReader(path)) {
            for (int i = 0; i < 5; i++) {
                string time = reader.ReadLine();
                
                if (time == "-1") {
                    HighScoreText[i].text = (i + 1).ToString() + ". NA";
                } else {
                    float timeAsFloat = float.Parse(time);

                    int roundedTimer = (int) timeAsFloat;

                    int minutes = roundedTimer / 60;
                    int seconds = roundedTimer % 60;
                    HighScoreText[i].text = (i + 1).ToString() + ". " + minutes.ToString() + "MIN " + seconds.ToString() + "S";
                }
            }
        }
    }
}
