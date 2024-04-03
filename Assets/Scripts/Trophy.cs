using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trophy : MonoBehaviour
{
    [Header("Audio/Visual")]
    [SerializeField]
    [Tooltip("Sound effect played when collected")]
    private AudioSource collectSound = null;
    [SerializeField]
    [Tooltip("Music Played when picked up")]
    private AudioClip chosenClip = null;
    [SerializeField]
    [Tooltip("Particle spawned when collected")]
    private ParticleSystem collectParticlePrefab = null;

    public GameObject jumpScare = null;
    bool gitWrekt;


    AudioSource bgReplacement;
    private AudioSource instancedSFX;

    private void OnTriggerEnter(Collider other) {
        //Debug.Log(other.gameObject.name);
        bgReplacement = GameObject.FindGameObjectWithTag("trophyMusic").GetComponent<AudioSource>();
        PlayerInventory inventory =
            other.attachedRigidbody.GetComponent<PlayerInventory>();

        if (inventory != null) {
            
            //@TODO add trophy bar that shows icons for trophies collected maybe do like 3 or 4 of them total. Use an enmu to select the trophy type and allow for specific behaviors

            PlayFX();
            //if (gitWrekt) { Destroy(gameObject); }
            Destroy(gameObject);
        }
    }

    //for the guitar trophy
    void PlayFX() {

        if (collectParticlePrefab != null) {
            ParticleSystem newParticle = Instantiate(collectParticlePrefab,
                transform.position, Quaternion.identity);
            newParticle.Play();
        }

        if (chosenClip != null) {
            bgReplacement.clip = chosenClip;
            bgReplacement.Play();        
        }

        
        if (collectSound != null) { 
            SoundPlayer.Instance.PlaySFX(collectSound, transform.position);
        }
           
        if (jumpScare != null) {
            Debug.Log("SCARE IS NOT NULL");
            gitWrekt = true; 
        }

    }

    void JumpScare(){
        if (gitWrekt == true) {
            Debug.Log("JS SCARE CHECK 1");
            instancedSFX = GameObject.FindGameObjectWithTag("trophySFX").GetComponent<AudioSource>();
            if (instancedSFX.time > 7.0f) {
                Debug.Log("JS SCARE CHECK 2");

                jumpScare.SetActive(true);
            }

            if (!instancedSFX.isPlaying || instancedSFX == null) {
                Debug.Log("JS SCARE CHECK 3");
                jumpScare.SetActive(false);
                gitWrekt = false;
            }

                
        //else { Destroy(gameObject); }
        }
    }

    void FixedUpdate() {
        if (gitWrekt) {
            JumpScare();
        }
    }
}
