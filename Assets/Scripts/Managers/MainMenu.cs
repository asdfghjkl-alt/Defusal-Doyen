using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Sets animator for scene transition
    [SerializeField] private Animator SceneTransition;

    // Start is called before the first frame update

    public void Start() {
        // Sets screen resolution based on user's platform
        if (Application.platform == RuntimePlatform.WindowsPlayer) {
            Screen.SetResolution( 1600, 900, FullScreenMode.Windowed);
        } else {
            Screen.SetResolution( 1920, 1080, FullScreenMode.Windowed);
        }
    }

    // Functions to go to different pages
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

    // Opens a tutorial page at the index
    public void OpenTutorialPg(string index) {
        StartCoroutine(waitLoadingScene("Tutorial " + index));
    }

    public IEnumerator waitLoadingScene(string SceneName) {
        // Plays click sound when btn clicked
        FindObjectOfType<AudioManager>().PlaySound("Click");

        // Sets transition to change scene
        SceneTransition.SetTrigger("Start");

        // Waits for animation to be over to load scene
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneName);
    }
}
