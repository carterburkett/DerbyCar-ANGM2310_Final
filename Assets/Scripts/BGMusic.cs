using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.WSA;

public class BGMusic : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Do you want to transition the clips together? If no, the bg music will mute and the trophy music will play at its full volume")]
    public bool fadeAudio;
    [SerializeField]
    [Tooltip("how long do you want the fade to last?")]
    public float fadeDuration = 0.0f;

    private float startTime = 0.0f;
    private float lerpTime = 0.0f;
    private float bgVolStorage;
    private float trophyVolStorage;
    private bool startCrossFade = false;

    private AudioSource audioSound;
    private AudioSource trophyMusic;

    private enum TrophyAudioFade {fadeIn, fadeOut};
    TrophyAudioFade audioUp = TrophyAudioFade.fadeIn;
    TrophyAudioFade audioDown = TrophyAudioFade.fadeOut;


    private void Awake() {
        audioSound = GameObject.FindGameObjectWithTag("bgMusicPlayer").GetComponent<AudioSource>();
        bgVolStorage = audioSound.volume;
    }

    void Update() {
       if(audioSound != null) {
            trophyMusic = GameObject.FindGameObjectWithTag("trophyMusic").GetComponent<AudioSource>();
            trophyVolStorage = trophyMusic.volume;
            TrophyMusicManager();
        } 
    }

    private void TrophyMusicManager(){

        float timeLeft = trophyMusic.clip.length - fadeDuration;

        if (trophyMusic != null && !trophyMusic.isPlaying) {
            audioSound.mute = false;
        }


        if (trophyMusic != null) {
            if (trophyMusic.isPlaying){
                if(fadeAudio){
                    if(audioSound.volume > 0 && trophyMusic.time <= timeLeft) {
                        TransitionAudio(audioSound, audioDown, bgVolStorage);
                    }
                
                    if(trophyMusic.time >= timeLeft){
                        CrossFadeAudio(audioSound, trophyMusic, bgVolStorage);
                    }
                }
                else{ audioSound.mute = true; }
            }
        }

        

        
    }

    //there's a couple ways i could hangle this.
        //calculate the position of the audio and do it off percentage
        //use the fade duration and start the fade when that amount of time is left
        //just fade the normal music back in after the trophy music is done

            //try the middle option first

    private void TransitionAudio(AudioSource fadingAudio, TrophyAudioFade dir, float volStorage) {
        if (lerpTime <= 1.0f) {
            //startTime += Time.deltaTime;
            startTime += Time.deltaTime;
            lerpTime = startTime / fadeDuration;

            if (dir == TrophyAudioFade.fadeIn) {
                Debug.Log("Fading in: " + fadingAudio.ToString());

                fadingAudio.mute = false;
                fadingAudio.volume = Mathf.Lerp(0, volStorage, lerpTime);
            }
            else if (dir == TrophyAudioFade.fadeOut) {
                Debug.Log("Fading out: " + fadingAudio.ToString());

                fadingAudio.volume = Mathf.Lerp(volStorage, 0, lerpTime);
            }
        }
        else if (lerpTime > 1.0f) {
            startTime = 0.0f;
            lerpTime = 0.0f;
        }
    }

    private void CrossFadeAudio(AudioSource audioUp, AudioSource audioDown, float upTarget){
        if (lerpTime <= 1.0f) {
            startTime += Time.deltaTime;
            lerpTime = startTime / fadeDuration;
            lerpTime = Mathf.Clamp01(lerpTime);

            audioUp.mute = false;
            audioUp.volume = Mathf.Lerp(0, upTarget, lerpTime);
            audioDown.volume = Mathf.Lerp(trophyVolStorage, 0, lerpTime); //@TODO tie this to audioDown instead of tVS
            
        }
        else if (lerpTime > 1.0f) {
            startTime = 0.0f;
            lerpTime = 0.0f;
        }
    }

}
