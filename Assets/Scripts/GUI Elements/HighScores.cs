using UnityEngine;
using TMPro;
using System.IO;

public class HighScores : MonoBehaviour
{
    [SerializeField] private TMP_Text[] QuestionRespText;

    // Start is called before the first frame update
    void Start()
    {
        string path = Application.dataPath + "/HighScores.txt";

        if (!File.Exists(path)) {
            File.WriteAllText(path, "");
        }

        Random.InitState((int) System.DateTime.Now.Ticks);

        using (StreamReader read = new StreamReader(path)) {
            for (int i = 0; i < 5; i++) {
                string time = read.ReadLine();
                
                if (time == "-1") {
                    QuestionRespText[i].text = (i + 1).ToString() + ". NA";
                } else {
                    float timeAsFloat = float.Parse(time);

                    int roundedTimer = (int) timeAsFloat;

                    int minutes = roundedTimer / 60;
                    int seconds = roundedTimer % 60;
                    QuestionRespText[i].text = minutes.ToString() + "MIN " + seconds.ToString() + "S";
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
