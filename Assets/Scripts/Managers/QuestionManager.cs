using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using System.Collections;

public class QuestionManager : MonoBehaviour
{
    // Text for Question Response Buttons
    [SerializeField] private TMP_Text[] QuestionRespText;

    // Getting component of Button of the Question Response Buttons
    [SerializeField] private Button[] QuestionRespBtn;

    // Image component of Question Response Buttons
    // To change color of button backgrounds
    [SerializeField] private Image[] QuestionBtnBg;
    
    // Text for the Question
    [SerializeField] private TMP_Text QuestionDescText;

    // Text for the countdown
    [SerializeField] private TMP_Text CountdownText;

    // Screen that pops up when user gets a question right
    [SerializeField] private GameObject CorrectScreen;

    // Screen that pops up when user gets a question wrong
    [SerializeField] private GameObject IncorrectScreen;

    // Controls animation for transitioning between scenes
    [SerializeField] private Animator SceneTransition;

    // Text to tell user what power ups the user will get as a reward
    // For getting Question right
    [SerializeField] private TMP_Text[] PowerUpRewards;

    // Storing data on the Questions 
    // (Question Prompt/Description, Possible Question Responses, Correct Response)
    string[] QuestionDesc = new string[44];
    string[,] QuestionResp = new string[44, 4];
    int[] CorrectQuestionResp = new int[44];

    // Stores which question has been chosen
    int questionNum;

    // Stores how much time the user has remaining to answer question
    float remainingTime = 15;

    // Keeps track if a button has been clicked
    bool btnClicked = false;

    // Start is called before the first frame update
    void Start()
    {
        // Path for the Question Data
        string path = Application.dataPath + "/QuestionData.txt";
        
        // Makes Random actually random
        Random.InitState((int) System.DateTime.Now.Ticks);


        // Reads Question Data file to get 
        // (Question Prompt/Description, Possible Question Responses, Correct Response)
        using (StreamReader reader = new StreamReader(path)) {

            // Loops through 44 Questions
            for (int i = 0; i < 44; i++) {
                QuestionDesc[i] = reader.ReadLine();

                for (int j = 0; j < 4; j++) {
                    QuestionResp[i, j] = reader.ReadLine();
                }
                CorrectQuestionResp[i] = int.Parse(reader.ReadLine());
                CorrectQuestionResp[i] -= 1;
            }
        }

        // Randomly generates the question number for which question
        // The user should answer
        questionNum = Random.Range(0, 44);

        // Displays the Question Prompt/Description
        QuestionDescText.text = QuestionDesc[questionNum];

        // Displays the Question Responses as Buttons
        for (int i = 0; i < 4; i++) {
            QuestionRespText[i].text = QuestionResp[questionNum, i];
        }
    }

    void FixedUpdate() {
        // If a button hasn't been clicked
        if (!btnClicked) {

            // Deducts time from timer
            remainingTime -= Time.fixedDeltaTime;

            // If user has ran out of time
            if (remainingTime < 0) {
                // Inputs a wrong answer
                QBtnClick(4);
            } else {
                // Function to display remaining time in seconds
                int roundedTime = (int) remainingTime;

                CountdownText.text = "Countdown: " + roundedTime.ToString();
            }
        }
    }

    // Function will be called by button (Assigned in Unity Editor)
    public void QBtnClick(int index) {
        // Tracks that a button has been clicked
        btnClicked = true;

        // Stores the old values of how many power ups the user has
        int[] OldPowerUpNo = new int[4];

        // Makes Question response buttons non-interactable
        // Also stores old values of how many power ups the user has
        for (int i = 0; i < 4; i++) {
            QuestionRespBtn[i].interactable = false;
            OldPowerUpNo[i] = StaticData.PowerUpNo[i];
        }

        // Makes Random actually random
        Random.InitState((int) System.DateTime.Now.Ticks);

        // If the user has gotten the question correct
        if (index == CorrectQuestionResp[questionNum]) {
            // User gets 1-2 bomb testers
            int numOfGiftedBombTest = Random.Range(1, 3);

            // Increases number of bomb testers the user has
            StaticData.PowerUpNo[2] += numOfGiftedBombTest;

            // Increases either number of anti bomb or bomb flagger by 1
            StaticData.PowerUpNo[Random.Range(0, 2)] += 1;

            // 1/3 Chance to get Plane Cho power up
            if (Random.Range(0, 3) == 0) {
                StaticData.PowerUpNo[3] += 1;
            }

            // Sets Correct Screen to be visible
            CorrectScreen.SetActive(true);

            // Plays sound indicating user got the question correct
            FindObjectOfType<AudioManager>().PlaySound("Correct");

            // Displays how many power ups the user has won
            for (int i = 0; i < 4; i++) {
                // Checks how many they have won through measuring 
                // Change of number of power ups
                PowerUpRewards[i].text = "X" + (StaticData.PowerUpNo[i] - OldPowerUpNo[i]);
            }
        } else {
            // Sets Incorrect Screen to be visible
            IncorrectScreen.SetActive(true);

            // Plays sound indicating user has gotten the question wrong
            FindObjectOfType<AudioManager>().PlaySound("Wrong");

            // Changes each of the buttons to have a red background
            for (int i = 0; i < 4; i++) {
                QuestionBtnBg[i].color = Color.red;
            }
        }

        // Sets the Correct Question button to be green
        QuestionBtnBg[CorrectQuestionResp[questionNum]].color = Color.green;

        // Function to transition back to game
        StartCoroutine(BackToGame());
    }

    public IEnumerator BackToGame() {
        // Waits for 2 seconds
        yield return new WaitForSeconds(2f);

        // Starts scene transition animation and waits for animation to be over
        SceneTransition.SetTrigger("Start");
        yield return new WaitForSeconds(1f);

        // Loads Main Game Scene
        SceneManager.LoadScene("Main Game");
    }
}
