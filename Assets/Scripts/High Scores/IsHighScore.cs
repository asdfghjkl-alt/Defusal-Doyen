using UnityEngine;
using System.IO;
using System.Runtime.CompilerServices;

public class IsHighScore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float[] highScoreTimes = new float[5];

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
                highScoreTimes[i] = float.Parse(time);
            }
        }

        bool endedLoop = false;
        int index = 0;

        while (index < 5 && !endedLoop) {
            if (highScoreTimes[index] == -1) {
                highScoreTimes[index] = StaticData.timer;
                endedLoop = true;
            } else {
                if (StaticData.timer < highScoreTimes[index]) {
                    endedLoop = true;

                    float timeToInput = StaticData.timer;

                    for (int i = index; i < 5; i++) {
                        float tempTime = highScoreTimes[i];

                        highScoreTimes[i] = timeToInput;

                        timeToInput = tempTime;
                    }
                }
            }

            index += 1;
        }

        using (StreamWriter writer = new StreamWriter(path, false)) { 
            for (int i = 0; i < 5; i++) {
                writer.WriteLine(highScoreTimes[i]);
            }
        }
    }
}
