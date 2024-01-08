using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused = false;
    public GameObject pauseMenuUI;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (IsPaused)
            {
                Resume();
            }
            else {
                Pause();
            }
        }
    }
    public void Resume() {
        pauseMenuUI.SetActive(false);
        //should we stop time when paused?
        IsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        //should we stop time when paused?
        IsPaused = true;
    }

    public void LoadMenu() {
        Debug.Log("Loading Menu");
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame() {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
