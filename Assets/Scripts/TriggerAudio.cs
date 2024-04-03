using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudio : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField][Tooltip("Audio to be Played on Trigger")]
    public AudioSource audioSound;
    
    void OnTriggerEnter(Collider other) {
        audioSound.Play();
    }
}
