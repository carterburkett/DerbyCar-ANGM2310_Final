using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void RestartLevel(){
        SceneManager.LoadScene(2);
    }

    public void ReturnToGame(){ 
        //@TODO make script to return to scene without resetting it
    }

    public void QuitGame(){
        Debug.Log("Game was Quit!");
        Application.Quit();
    }
}
