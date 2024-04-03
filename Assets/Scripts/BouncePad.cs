using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif

using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;


public class BouncePad : MonoBehaviour{
    //@TODO this might be better if there was an option for trigger stay as well. so that you could set a delay before the player is launched. (NOT NEEDED NOW)

    [HideInInspector] public enum bounceOrElevator { bounce, elevator };
    [HideInInspector] public bounceOrElevator trampBehavior;
    [HideInInspector] public GameObject trampoline;
    [HideInInspector] public Vector3 outDir, destination;
    [HideInInspector] public float speed, force;
    [HideInInspector] public AudioSource audioSound;
    [HideInInspector] public bool playAudio;
    

    private bool startElevator = false;
    private Collision recievedCol = null;

    private float duration = 0.0f;
    private float countTime = 0.0f;

    private Vector3 startPosition;
    private Vector3 center;
    private Vector3 startRelCenter;
    private Vector3 destRelCenter;


    void Start() {
        trampoline = this.GameObject();
        force *= 200;
    }

    void Update() {
        if (startElevator) {
            MoveToFixedPos(recievedCol);
        }
    }

    void OnCollisionEnter(Collision hitObj) {
        Rigidbody hitRB = hitObj.gameObject.GetComponent<Rigidbody>();
        if (hitRB != null) {

            if(playAudio) {
                //@TODO add support for multiple audio files and set it up so it plays them by randomly selecting an index value from an array
                Debug.Log("Audio Started");
                audioSound.Play();
            }
            //else{ Debug.Log("Boolean Assignment Error"); }
            
            if(trampBehavior == bounceOrElevator.bounce){
                Debug.Log("Player Collided with Trampoline");
                Bounce(hitObj);
            }
            else if (trampBehavior == bounceOrElevator.elevator){
                Debug.Log("Elevator Activated");
                startPosition = hitRB.position;
                recievedCol = hitObj;
                startElevator = true;
            }

            

        }
    }

    void Bounce(Collision col) {
        //@TODO make this better and more consistent
        //@DEBUG colliding with the wheel colliders on the car can cause weird behavior sometimes. Fix this so it works better with the car and future prefrabs...

        outDir = col.transform.position - trampoline.transform.position;
        col.rigidbody.AddForce(outDir * force, ForceMode.Impulse);
    }

    void MoveToFixedPos(Collision col) {
        //@TODO change variable names so they make more sense i.e. "speed" to "duration" and vice versa
        //@TODO add menu option for rotational destination of the bounced object and use the input here as well
            //Currently the object just kind of ends up in a random position which causes problems
            //@DEBUG for the reasons above
        //@TODO this is a slerper script :: move it to it's own script so you can use it for other game objects.

        //@INFO This isn't really the best implementation of this script but it works for what I'm trying to accomplish in the mean time. Might want to look into Raytracing
            //Not having any control of the car when it's in air feels like shit. I don't know if this is an issue with prof's car script or if it's something i can fix with a different script
        
        Quaternion finishSpin = Quaternion.Euler(0, 166, 0); //@TODO FIX THIS
       
        Rigidbody hitRB = col.gameObject.GetComponent<Rigidbody>();
        hitRB.velocity = new Vector3(0,0,0);
        FindRelCenter(Vector3.up);

        //Debug.Log("MTFP Started");
        if (duration <= 1.0f) {
            //Debug.Log("Car's position is " + hitRB.position);
           // Debug.Log("Destination is " + destination);

            countTime += Time.deltaTime;
            duration = countTime / speed;
            duration = Mathf.SmoothStep(0, 1, duration);

            col.transform.position = Vector3.Slerp(startRelCenter, destRelCenter, duration);
            col.transform.position += center;

            col.transform.rotation = Quaternion.Lerp(hitRB.rotation, finishSpin, duration);
            
            if(duration >= 1.0f){
                //resets values so that the class can be called again on next collision
                startElevator = false;
                countTime = 0.0f;
                duration = 0.0f;

            }

            if(hitRB.position == destination){ startElevator = false; }     // Debug.Log("ERROR: time not met"); }
                //This is a failsafe to make sure it doesn't get stuck in a loop spinning in the air if duration fails somehow...
        }

    }

    public void FindRelCenter(Vector3 direction) {
        center = (startPosition + destination) / 2f;
        center -= direction;
        startRelCenter = startPosition - center;
        destRelCenter = destination - center;
    }


}

//============================================================================
//========================Trampoline Behavior Editor==========================
//============================================================================
#region Editor
#if UNITY_EDITOR

[CustomEditor(typeof(BouncePad))]
[CanEditMultipleObjects]
public class bounceOptionEditor : Editor {
    //@TODO add undo and redo functionality
    public override void OnInspectorGUI() {
        serializedObject.Update();
        base.OnInspectorGUI();

        BouncePad guiChoice = (BouncePad)target;
        
        GUIContent choiceTip = new GUIContent("Behavior Options", "Select whether the Trampoline sends the object to a set destination or bounces based off trajectory \n *Note: Bounce is currently a little buggy*");
        GUIContent AudioTip = new GUIContent("Play Audio?", "Audio can be played on impact with the bouncepad");
        GUIContent AudioFileTip = new GUIContent("Audio File", "Reference to the Audio File that will be played on collision");


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(choiceTip, EditorStyles.boldLabel);
        guiChoice.trampBehavior = (BouncePad.bounceOrElevator)EditorGUILayout.EnumPopup(guiChoice.trampBehavior);
        EditorGUILayout.EndHorizontal();




        if (guiChoice.trampBehavior == BouncePad.bounceOrElevator.bounce) {
            DrawBounceDetails(guiChoice);
        }
        else if (guiChoice.trampBehavior == BouncePad.bounceOrElevator.elevator) {
            DrawElevatorDetails(guiChoice);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Sound Effect", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUI.indentLevel++;
        guiChoice.playAudio = EditorGUILayout.Toggle(AudioTip, guiChoice.playAudio);
        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(!guiChoice.playAudio);
        guiChoice.audioSound = EditorGUILayout.ObjectField(AudioFileTip, guiChoice.audioSound, typeof(AudioSource), true) as AudioSource;
        EditorGUI.EndDisabledGroup();


        EditorUtility.SetDirty(guiChoice);
        serializedObject.ApplyModifiedProperties();
    }

    static void DrawElevatorDetails(BouncePad guiChoice) {
        //@TODO clamp the duration value so that it cannot be 0;

        GUIContent destToolTip = new GUIContent("Destination", "Ending/landing destination for the bounced object");
        GUIContent speedToolTip = new GUIContent("Duration", "How long before the bounced object reaches it's destination");

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Elevator Options", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        guiChoice.destination = EditorGUILayout.Vector3Field(destToolTip, guiChoice.destination);
            EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(speedToolTip);
        guiChoice.speed = EditorGUILayout.FloatField(guiChoice.speed);
            EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
    }

    static void DrawBounceDetails(BouncePad guiChoice) {
        //@TODO clamp the fouce value so it cannot be 0;

        GUIContent forceToolTip = new GUIContent("Bounce Force", "How Bouncy the Trampoline is on Impact");
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Bounce Options", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(forceToolTip);
        guiChoice.force = EditorGUILayout.FloatField(guiChoice.force, GUILayout.MaxWidth(200));
            EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel--;
    }
}
#endif
#endregion
