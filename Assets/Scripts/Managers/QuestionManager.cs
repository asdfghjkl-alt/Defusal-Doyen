using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using System.Collections;

public class QuestionManager : MonoBehaviour
{
    [SerializeField] private TMP_Text[] QuestionRespText;
    [SerializeField] private Button[] QuestionRespBtn;
    [SerializeField] private Image[] QuestionBtnBg;
    [SerializeField] private TMP_Text QuestionDescText;
    [SerializeField] private TMP_Text CountdownText;
    [SerializeField] private GameObject CorrectScreen;
    [SerializeField] private GameObject IncorrectScreen;
    [SerializeField] private Animator SceneTransition;
    [SerializeField] private TMP_Text[] PowerUpRewards;

    string[] QuestionDesc = new string[44];
    string[,] QuestionResp = new string[44, 4];
    int[] CorrectQuestionResp = new int[44];

    int questionNum;
    float remainingTime = 15;
    bool btnClicked = false;

    // Start is called before the first frame update
    void Start()
    {
        string path = Application.dataPath + "/QuestionData.txt";

        if (!File.Exists(path)) {
            File.WriteAllText(path, "");
        }

        Random.InitState((int) System.DateTime.Now.Ticks);

        using (StreamReader reader = new StreamReader(path)) {
            for (int i = 0; i < 44; i++) {
                QuestionDesc[i] = reader.ReadLine();

                for (int j = 0; j < 4; j++) {
                    QuestionResp[i, j] = reader.ReadLine();
                }
                CorrectQuestionResp[i] = int.Parse(reader.ReadLine());
                CorrectQuestionResp[i] -= 1;
            }
        }

        questionNum = Random.Range(0, 44);

        QuestionDescText.text = QuestionDesc[questionNum];

        for (int i = 0; i < 4; i++) {
            QuestionRespText[i].text = QuestionResp[questionNum, i];
        }
    }

    void FixedUpdate() {
        if (!btnClicked) {
            remainingTime -= Time.fixedDeltaTime;

            if (remainingTime < 0) {
                QBtnClick(4);
            } else {
                int roundedTime = (int) remainingTime;

                CountdownText.text = "Countdown: " + roundedTime.ToString();
            }
        }
    }

    public void QBtnClick(int index) {
        btnClicked = true;

        int[] OldPowerUpNo = new int[4];

        for (int i = 0; i < 4; i++) {
            QuestionRespBtn[i].interactable = false;
            OldPowerUpNo[i] = StaticData.PowerUpNo[i];
        }

        Random.InitState((int) System.DateTime.Now.Ticks);

        if (index == CorrectQuestionResp[questionNum]) {
            int numOfGiftedBombTest = Random.Range(1, 3);

            StaticData.PowerUpNo[2] += numOfGiftedBombTest;

            StaticData.PowerUpNo[Random.Range(0, 2)] += 1;

            if (Random.Range(0, 2) == 0) {
                StaticData.PowerUpNo[3] += 1;
            }

            CorrectScreen.SetActive(true);
            FindObjectOfType<AudioManager>().PlaySound("Correct");

            for (int i = 0; i < 4; i++) {
                PowerUpRewards[i].text = "X" + (StaticData.PowerUpNo[i] - OldPowerUpNo[i]);
            }
        } else {
            IncorrectScreen.SetActive(true);

            FindObjectOfType<AudioManager>().PlaySound("Wrong");

            for (int i = 0; i < 4; i++) {
                QuestionBtnBg[i].color = Color.red;
            }
        }

        QuestionBtnBg[CorrectQuestionResp[questionNum]].color = Color.green;

        StartCoroutine(Waiting());
    }

    public IEnumerator Waiting() {
        yield return new WaitForSeconds(2f);

        SceneTransition.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Main Game");
    }
}
