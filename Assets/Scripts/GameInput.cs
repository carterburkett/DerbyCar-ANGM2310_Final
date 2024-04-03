using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInput : MonoBehaviour{
    
    void Update(){


        //game reload
        if (Input.GetKeyDown(KeyCode.Backspace)) {

            Debug.Log("Game was Restarted via Backspace!");
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentLevelIndex);

        }

        //@IMPORTANT!
            //Game Exit is handled by PreserveLevel.cs (which is terribly named at the moment)


    }
}
