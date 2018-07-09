using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuBehavior : MonoBehaviour {

    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject tutorialUI;

    public AudioSource buttonClick;
    public AudioSource backgroundMusic;
    public GameObject ToggleBackgroundMusic; 

    private static bool _playBackMusic = true;
    private static bool _init = false;

    void Start() {
        if(!_init) {
            if (_playBackMusic) {
            ToggleBackgroundMusic.GetComponent<Toggle>().isOn = true;
            } else {
                ToggleBackgroundMusic.GetComponent<Toggle>().isOn = false;
            }
            _init = true;
        } else {
            if (!_playBackMusic) {
                ToggleBackgroundMusic.GetComponent<Toggle>().isOn = true;
                PlayBackroundMusic();
            }
        }
    }

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

    public void LoadMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void PlayAudio() {
        buttonClick.Play();
    }

    public void PlayBackroundMusic() {
        if (_playBackMusic) {
            backgroundMusic.Play();
        } else {
            backgroundMusic.Stop();
        }
            _playBackMusic = !_playBackMusic;
    }
}
