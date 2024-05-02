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

    string[] QuestionDesc = new string[44];
    string[,] QuestionResp = new string[44, 4];
    int[] CorrectQuestionResp = new int[44];

    int QuestionNum;
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

        QuestionNum = Random.Range(0, 44);

        QuestionDescText.text = QuestionDesc[QuestionNum];

        for (int i = 0; i < 4; i++) {
            QuestionRespText[i].text = QuestionResp[QuestionNum, i];
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

        for (int i = 0; i < 4; i++) {
            QuestionRespBtn[i].interactable = false;
        }

        Random.InitState((int) System.DateTime.Now.Ticks);

        if (index == CorrectQuestionResp[QuestionNum]) {
            int numOfGiftedPs = Random.Range(1, 4);

            Debug.Log(numOfGiftedPs);

            for (int i = 0; i < numOfGiftedPs; i++) {
                StaticData.PowerUpNo[Random.Range(0, 3)] += 1;
            }

            CorrectScreen.SetActive(true);
            FindObjectOfType<AudioManager>().PlaySound("Correct");
        } else {
            IncorrectScreen.SetActive(true);

            FindObjectOfType<AudioManager>().PlaySound("Wrong");

            for (int i = 0; i < 4; i++) {
                QuestionBtnBg[i].color = Color.red;
            }
        }

        QuestionBtnBg[CorrectQuestionResp[QuestionNum]].color = Color.green;

        StartCoroutine(waiting());
    }

    public IEnumerator waiting() {
        yield return new WaitForSeconds(1f);

        SceneTransition.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Main Game");
    }
}
