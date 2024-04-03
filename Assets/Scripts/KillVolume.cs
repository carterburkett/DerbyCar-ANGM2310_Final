using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;

public class KillVolume : MonoBehaviour
{
    //@TODO add volume slider

    [SerializeField]
    private string deadText = "Game Over...";
    private UIController uiController;
    
    [InspectorLabel("Sound on Death")]
    public AudioSource deadSoundPrefab;

    //[HideInInspector] public AudioClip deathClip = null;
    [HideInInspector] public bool changeAudio = false;
    [HideInInspector] public List<AudioClip> deadSounds = new List<AudioClip>();

    private void Awake() {
        uiController = FindObjectOfType<UIController>();

    }

    private void OnTriggerEnter(Collider other) {
        CarController carController = other.attachedRigidbody.gameObject.GetComponent<CarController>(); 
        if (carController != null ){
            carController.Die();
            if (uiController != null) {
                uiController.ShowWinText(deadText);
            }
        }

        other.gameObject.SetActive(false);

        

        if (deadSoundPrefab != null) {
            if(changeAudio){ 
                if(deadSounds[0] != null){ 
                   int randSelection = Random.Range(0, deadSounds.Count);
                    
                    deadSoundPrefab.clip = deadSounds[randSelection];
                }

            }
            SoundPlayer.Instance.PlaySFX(deadSoundPrefab, transform.position);
        }

    }
}

#region Editor
#if UNITY_EDITOR

[CustomEditor(typeof(KillVolume))]
[CanEditMultipleObjects]
public class KillVolumeEditor : Editor {
    public override void OnInspectorGUI() {
        serializedObject.Update();
        base.OnInspectorGUI();

        KillVolume guiChoice = (KillVolume)target;
        
        
        List<AudioClip> clipList = guiChoice.deadSounds;

        GUIContent AudioTip = new GUIContent("Randomize?", "Randomize clips that can be played?");
        GUIContent ClipTip = new GUIContent("Clip", "Drag Audio Clip Here");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
        guiChoice.changeAudio = EditorGUILayout.Toggle(AudioTip, guiChoice.changeAudio);

        EditorGUI.indentLevel++;

        if (guiChoice.changeAudio) {
            int size = Mathf.Max(0, EditorGUILayout.IntField("Amount of Clips", clipList.Count));

            while (size > clipList.Count) { clipList.Add(null); }
            while (size < clipList.Count) { clipList.RemoveAt(clipList.Count - 1); }

            for (int i = 0; i < clipList.Count; i++) {
                clipList[i] = EditorGUILayout.ObjectField("Clip" + i, clipList[i], typeof(AudioClip), false) as AudioClip;

            }
        }

        EditorGUI.indentLevel--;

        EditorUtility.SetDirty(guiChoice);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
    #endregion
