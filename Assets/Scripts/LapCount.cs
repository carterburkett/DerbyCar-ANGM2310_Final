using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapCount : MonoBehaviour{
    private CarController carController;
    public bool useFinishLine = false;
    private PlayerInventory inventory;
    [HideInInspector] public bool readyLapAdd = false;


    private void OnTriggerEnter(Collider other) {
        inventory = other.attachedRigidbody.GetComponent<PlayerInventory>();
        
        carController = other.attachedRigidbody.GetComponent<CarController>();
        
        if (inventory != null) {
            
            if(useFinishLine) {
                readyLapAdd = true;
            }
            else{
                inventory.AddLap();
                Debug.Log("Player completed a lap. Finish line not used");
                //readyLapAdd = false;
            }
        }
    }
}

    
