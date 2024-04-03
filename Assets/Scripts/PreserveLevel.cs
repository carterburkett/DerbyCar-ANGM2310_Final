using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreserveLevel : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject pauseMenuUI;


    public void Resume() {
        Debug.Log("Game was resumed!");

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;

    }

    public void Pause() {
        Debug.Log("Game was paused!");

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;

    }

    public void QuitGame() {
        Debug.Log("Game was Quit!");

        Application.Quit();
    }

    public void RestartLevel() {
        Debug.Log("Scene Restarted through Menu");

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;

        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentLevelIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GamePaused){Resume();} 
            else {Pause();}
        }
    }
}
