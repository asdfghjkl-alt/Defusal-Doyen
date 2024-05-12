using UnityEngine;
using System.IO;

// Class in Endscreen Win Screen
public class IsHighScore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float[] HighScoreTimes = new float[5];

        string path = Application.dataPath + "/HighScoresData/HighScores.txt";

        if (!File.Exists(path)) {
            Directory.CreateDirectory(Application.dataPath + "/HighScoresData");
            using (StreamWriter writer = new StreamWriter(path, false)) { 
                for (int i = 0; i < 5; i++) {
                    writer.WriteLine(-1);
                }
            }
        }

        using (StreamReader reader = new StreamReader(path)) {
            for (int i = 0; i < 5; i++) {
                string time = reader.ReadLine();
                HighScoreTimes[i] = float.Parse(time);
            }
        }

        bool endedLoop = false;
        int index = 0;

        while (index < 5 && !endedLoop) {
            if (HighScoreTimes[index] == -1) {
                HighScoreTimes[index] = StaticData.timer;
                endedLoop = true;
            } else {
                if (StaticData.timer < HighScoreTimes[index]) {
                    endedLoop = true;

                    float timeToInput = StaticData.timer;

                    for (int i = index; i < 5; i++) {
                        float tempTime = HighScoreTimes[i];

                        HighScoreTimes[i] = timeToInput;

                        timeToInput = tempTime;
                    }
                }
            }

            index += 1;
        }

        using (StreamWriter writer = new StreamWriter(path, false)) { 
            for (int i = 0; i < 5; i++) {
                writer.WriteLine(HighScoreTimes[i]);
            }
        }
    }
}
