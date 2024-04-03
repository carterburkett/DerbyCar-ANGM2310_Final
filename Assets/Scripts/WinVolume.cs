using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinVolume : MonoBehaviour
{
    [SerializeField]
    private string winText = "Good Job!";
    private UIController uiController;
    private AudioSource winSoundPrefab;

    private void Awake ( ) {
        uiController = FindObjectOfType<UIController>();
    }
    

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);

        if( uiController != null ) {
            uiController.ShowWinText(winText);
        }

        if(winSoundPrefab != null ) {
            SoundPlayer.Instance.PlaySFX(winSoundPrefab, transform.position);
        }
    }
}
