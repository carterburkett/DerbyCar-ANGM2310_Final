using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    public Vector3 destination;
    private Vector3 vel = Vector3.zero;
    private Vector3 exitPos;
    private Vector3 startPos;

    private float backDuration = 0.0f;
    public float duration;
    private float startTime = 0.0f;
    
    private bool startMove = false;
    private bool startBack = false;

    private void OnTriggerEnter(Collider other) {
        other.transform.SetParent(transform);
        
        startMove = true;
    }

    private void OnTriggerStay(Collider other) {
        Debug.Log("player staying");
        CarController playerCar = other.GetComponent<CarController>();
        Rigidbody rb = other.GetComponent<Rigidbody>();
        
        //startMove = true;
        startBack = false;
    }

    private void OnTriggerExit(Collider other) {
        other.transform.SetParent(null);
        exitPos = this.transform.position;
        startBack = true;
    }

    private void Start() {
        startPos = this.transform.position;
    }

    private void FixedUpdate() {
        
        if (startMove) {
            if (startTime >= 0.0f) {
                
                startTime += Time.deltaTime;
                float t = startTime / duration;
                t = Mathf.SmoothStep(0,1,t);

                if(t >= 1.0f) {
                    backDuration = startTime;
                    startTime = 0.0f;
                    startMove = false;
                }

                this.transform.position = Vector3.Lerp(startPos, destination, t);

            }
        }


        if(startBack) {
            if (startTime >= 0.0f) {

                startTime += Time.deltaTime;
                float t = startTime / backDuration;
                t = Mathf.SmoothStep(0, 1, t);

                if (t >= 1.0f) {
                    
                }

                this.transform.position = Vector3.Lerp(exitPos, startPos, t);

            }

        }
    }
}
