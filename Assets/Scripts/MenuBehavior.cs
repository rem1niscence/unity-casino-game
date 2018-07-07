using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour {

    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (gameIsPaused) {
                Resume();
            } else {
                Pause();
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
}
