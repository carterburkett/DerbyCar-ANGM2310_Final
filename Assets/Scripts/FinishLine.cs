using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
#endif
using UnityEngine;
//using static Unity.VisualScripting.FlowStateWidget;


//@TODO add particle FX with that shoot color of lap line on crossing?
//@TODO add a special win sound
public class FinishLine : MonoBehaviour
{
    public GameObject lapVolume;

    [Tooltip("How many laps to win the game?")][Range(1,5)]
    public int lapsToWin = 1;

    [HideInInspector] public bool changeMats = false;
    [HideInInspector] public bool useAudio = false;
    [HideInInspector] public bool useParticles = false;
    [HideInInspector] public AudioSource audioSource = null;
    [HideInInspector] public AudioSource lapAudio = null;
    [HideInInspector] public List<Material> _materials = new List<Material>();
    [HideInInspector] public List<ParticleSystem> _particles = new List<ParticleSystem>();
    [HideInInspector] public List<ParticleSystem> particleEvents = new List<ParticleSystem>();
    [HideInInspector] public List<List<ParticleSystem>> systemsList = new List<List<ParticleSystem>>();


    private WinVolume winVolume;

    private void OnTriggerEnter(Collider other) {
        PlayerInventory inventory = other.attachedRigidbody.GetComponent<PlayerInventory>();
        LapCount laps = lapVolume.GetComponent<LapCount>();
        
        //Debug.Log("Lap Count = " + inventory.lapCount);
        //Debug.Log("Laps to Win = " + lapsToWin);

        if (inventory.lapCount >= lapsToWin - 1 && laps.readyLapAdd) {
            
            this.AddComponent<WinVolume>();

            if (audioSource != null) {
                AudioSource bgAudio = GameObject.FindGameObjectWithTag("bgMusicPlayer").GetComponent<AudioSource>();
                bgAudio.volume *= .75f;
                SoundPlayer.Instance.PlaySFX(audioSource, transform.position);
            }
        }

        if (laps.readyLapAdd && inventory.lapCount < lapsToWin){

            if (useParticles) {
                for(int i = 0;i < _particles.Count;i++){
                    _particles[i].Play();
                }
            }
            if(useAudio && lapAudio != null) {
                lapAudio.Play();
            }

            inventory.AddLap();

            

            Debug.Log("Player completed a lap");
            laps.readyLapAdd = false;
        }

        this.GetComponent<Renderer>().material = _materials[inventory.lapCount];


    }
}


#region Editor
#if UNITY_EDITOR

[CustomEditor(typeof(FinishLine))]
[CanEditMultipleObjects]
public class FinishLineEditor : Editor {
    //@TODO add undo and redo functionality
    //@TODO break the individual draws into separate methods
    public override void OnInspectorGUI() {
        serializedObject.Update();
        base.OnInspectorGUI();

        FinishLine guiChoice = (FinishLine)target;
        List<ParticleSystem> pfxList = guiChoice._particles;
        //List<ParticleSystem> eventList = guiChoice.particleEvents;
        List<List<ParticleSystem>> listsList = guiChoice.systemsList;
        List<Material> matList = guiChoice._materials;


        GUIContent ParticleTip = new GUIContent("Use Particles?", "Use particles when completing a lap?");
        GUIContent AudioTip = new GUIContent("Play Audio?", "Audio can be played when triggering next Lap");
        GUIContent LapSoundTip = new GUIContent("Lap Sound", "Sound that will be played when crossing the finish line");
        GUIContent AudioSourceTip = new GUIContent("Win Sound", "Audio Source for Win Volume (Must be set here if using finish line as win volume)");
        GUIContent MaterialTip = new GUIContent("Change Materials?", "Change Finish Line Material on Laps? \n\nThe material currently equipped to the finish line will be lap '0'.");

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
        guiChoice.useAudio = EditorGUILayout.Toggle(AudioTip, guiChoice.useAudio);

        EditorGUI.indentLevel++;

        if (guiChoice.useAudio) {
            guiChoice.lapAudio = EditorGUILayout.ObjectField(LapSoundTip, guiChoice.lapAudio, typeof(AudioSource), true) as AudioSource;
            guiChoice.audioSource = EditorGUILayout.ObjectField(AudioSourceTip, guiChoice.audioSource, typeof(AudioSource), true) as AudioSource;
        }
        EditorGUI.indentLevel--;

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Particle FX", EditorStyles.boldLabel);
        guiChoice.useParticles = EditorGUILayout.Toggle(ParticleTip, guiChoice.useParticles);
        EditorGUILayout.Space();

        //if (guiChoice.useParticles) {
        //    int size = EditorGUILayout.IntSlider("Lap Systems", pfxList.Count, 1, 4);
            
        //    EditorGUI.indentLevel++;

        //    while (size > pfxList.Count) { pfxList.Add(null); }
        //    while (size < pfxList.Count) { pfxList.RemoveAt(pfxList.Count - 1); }
            
        //    for (int i = 0; i < pfxList.Count; i++) {
        //        GUIContent LapFXTip = new GUIContent("Lap System " + i, "IMPORTANT! \n These are not instanced. You need to use systems from the scene.");
        //        pfxList[i] = EditorGUILayout.ObjectField(LapFXTip, pfxList[i], typeof(ParticleSystem), true) as ParticleSystem;
        //    }

        //    EditorGUI.indentLevel--;
        //    EditorGUILayout.Space();
            
        //    //int eventSize = EditorGUILayout.IntSlider("World Systems", eventList.Count, 1, 20);
        //    EditorGUI.indentLevel++;

        //    while(guiChoice.lapsToWin > listsList.Count) { listsList.Add(null); }
        //    while(guiChoice.lapsToWin < listsList.Count) { listsList.RemoveAt(listsList.Count - 1); }

        //    for (int i = 0; i < guiChoice.lapsToWin; i++){ 
        //        EditorGUILayout.LabelField("Lap " + (i + 1), EditorStyles.boldLabel);
                
        //        EditorGUI.indentLevel++;
        //        int eventSize = EditorGUILayout.IntSlider("Systems for Lap " + (i + 1), listsList[i].Count, 1, 5);

        //        for (int f = 0; f < listsList.Count; f++) {
        //            List<ParticleSystem> eventList;
        //            listsList[i] = guiChoice.particleEvents;
        //            eventList = listsList[i];

        //            //int eventSize = EditorGUILayout.IntSlider("Systems for Lap " + (i + 1), listsList[i].Count, 1, 5);

        //            while (eventSize > listsList[i].Count) { listsList[i].Add(null); }
        //            while (eventSize < listsList[i].Count) { listsList[i].RemoveAt(listsList.Count - 1); }

        //            //for(int k = 0; k < eventSize; k++){
        //            //    EditorGUI.indentLevel++;

        //            //    GUIContent LapFXTip = new GUIContent("System " + (k+1), "IMPORTANT!\nThese are not instanced. You need to use systems from the scene.");
        //            //    eventList[f] = EditorGUILayout.ObjectField(LapFXTip, eventList[f], typeof(ParticleSystem), true) as ParticleSystem;

        //            //    EditorGUI.indentLevel--;
        //            //}
        //        }
        //        EditorGUI.indentLevel--;

        //    }
        //}

        EditorGUI.indentLevel--;

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        guiChoice.changeMats = EditorGUILayout.Toggle(MaterialTip, guiChoice.changeMats);
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel++;


        if (guiChoice.changeMats){

            while (guiChoice.lapsToWin > matList.Count) { matList.Add(null); }
            while (guiChoice.lapsToWin < matList.Count) { matList.RemoveAt(matList.Count - 1); }

            for (int i = 0; i < matList.Count; i++) {
                matList[i] = EditorGUILayout.ObjectField("Lap " + (i+1) , matList[i], typeof(Material), false) as Material;
            }
        }

        EditorUtility.SetDirty(guiChoice);
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
#endregion