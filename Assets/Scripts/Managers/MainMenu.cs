using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator SceneTransition;

    // Start is called before the first frame update

    public void Start() {
        if (Application.platform == RuntimePlatform.WindowsPlayer) {
            Screen.SetResolution( 1600, 900, FullScreenMode.Windowed);
        } else {
            Screen.SetResolution( 1920, 1080, FullScreenMode.Windowed);
        }
    }

    public void StartGame() {
        StartCoroutine(waitLoadingScene("Main Game"));
    }

    public void GoToHighScores() {
        StartCoroutine(waitLoadingScene("High Scores"));
    }

    public void GoToTutorial() {
        StartCoroutine(waitLoadingScene("Tutorial"));
    }

    public void GoToHome() {
        StartCoroutine(waitLoadingScene("Home"));
    }

    public void Quit() {
        Application.Quit();
    }

    public void OpenTutorialPg(string index) {
        StartCoroutine(waitLoadingScene("Tutorial " + index));
    }

    public IEnumerator waitLoadingScene(string SceneName) {
        FindObjectOfType<AudioManager>().PlaySound("Click");
        SceneTransition.SetTrigger("Start");

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneName);
    }
}
