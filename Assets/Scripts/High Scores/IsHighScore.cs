using UnityEngine;
using System.IO;

// Class in Endscreen Win Screen
public class IsHighScore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Preparing array to store High Score Times
        float[] HighScoreTimes = new float[5];


        // Path for High Scores File
        string path = Application.dataPath + "/HighScoresData/HighScores" + StaticData.difficulty + ".txt";

        if (!File.Exists(path)) {
            // Create directory HighScoresData if directory doesn't exist

            Directory.CreateDirectory(Application.dataPath + "/HighScoresData");

            // Creates Unfilled Data in file
            using (StreamWriter writer = new StreamWriter(path, false)) { 
                for (int i = 0; i < 5; i++) {
                    writer.WriteLine(-1);
                }
            }
        }

        // Reads file for times and inputs them into the array
        using (StreamReader reader = new StreamReader(path)) {
            for (int i = 0; i < 5; i++) {
                string time = reader.ReadLine();
                HighScoreTimes[i] = float.Parse(time);
            }
        }

        // Tracking whether to end loop and the index of the high scores times
        // the loop is passing through
        bool endedLoop = false;
        int index = 0;

        // Loops through the 5 times
        while (index < 5 && !endedLoop) {
            if (HighScoreTimes[index] == -1) {
                // If the slot isn't filled, then put the player's timer into it
                HighScoreTimes[index] = StaticData.timer;
                endedLoop = true;
            } else {
                // If the player timer is less than one of the high score times
                if (StaticData.timer < HighScoreTimes[index]) {

                    // The loop can end
                    endedLoop = true;

                    // Time to replace in the high scores file
                    // Also serves as a temporary store of the time
                    // that was previously there to move down
                    float timeToInput = StaticData.timer;

                    for (int i = index; i < 5; i++) {
                        // Stores the time previously in that slot temporarily
                        float tempTime = HighScoreTimes[i];

                        // Replaces the time with the time that is less
                        HighScoreTimes[i] = timeToInput;

                        // Stores the time previously in the slot
                        // To input into the next slot
                        timeToInput = tempTime;
                    }
                }
            }

            index += 1;
        }

        using (StreamWriter writer = new StreamWriter(path, false)) { 
            for (int i = 0; i < 5; i++) {
                // Writes the new High Score Times onto file
                writer.WriteLine(HighScoreTimes[i]);
            }
        }
    }
}
