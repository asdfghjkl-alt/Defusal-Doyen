using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator SceneTransition;

    // Start is called before the first frame update

    public void Start() {
        Screen.SetResolution( 1600, 900, FullScreenMode.Windowed);
 
        // Setting register for width and height of game.
        PlayerPrefs.SetInt( "Screenmanager Resolution Width", 1600 );
        PlayerPrefs.SetInt( "Screenmanager Resolution Width", 900 );
        PlayerPrefs.SetInt( "Screenmanager Fullscreen mode", (int)FullScreenMode.Windowed );
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
