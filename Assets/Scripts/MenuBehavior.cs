using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour {

    public static bool gameIsPaused = false;

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

    void Resume() {

    }

    void Pause() {

    }
}
