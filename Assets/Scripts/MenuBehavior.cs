using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour {

    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject tutorialUI;

    void Update() {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Level") {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                if (gameIsPaused) {
                    Resume();
                } else {
                    Pause();
                }
            }
        }
    }
    public void StartGame() {
        SceneManager.LoadScene("Level");
    }

    public void Quit() {
        Application.Quit();
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        gameIsPaused = false;
    }

    public void Pause() {
        pauseMenuUI.SetActive(true);
        gameIsPaused = true;
    }

    public void ShowTutorial(bool show) {
        if (show) {
            tutorialUI.SetActive(true);
        } else {
            tutorialUI.SetActive(false);
        }
    }
}
