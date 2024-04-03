using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GravityModTrigger : MonoBehaviour
{
    //@TODO make a menu
    //@INFO this script could have been made better, right now it's mostly designed to make long jumps off of ramps or make them shorter. 
    public enum TriggerType { enter, exit, stay };
    
    [InspectorLabel("Behavior")]
    [Tooltip("Decides whether the trigger activates on entry or exit")]
    public TriggerType behavior = TriggerType.exit;
    
    [SerializeField][Range(0.0f, 10f)]
    [Tooltip("Increasing falls faster, decreasing falls slower")]
     private float temporaryGravityMultiplier;

    [InspectorLabel("Change Speed?")]
    [Tooltip("Checking yes will alter the speed of the car while gravity is altered")]
    public bool alterSpeed = false;


    [SerializeField]
    [Range(0.0f, 100f)]
    [Tooltip("Increasing increases velocity, decreasing decreases velocity")]
    private float temporarySpeed;



    private bool flyingHigh = false;
    private CarController playerCar = null;
    private float gravityStorage;
    private float velocityStorage;

    void Update() {
        if (flyingHigh && playerCar.CheckIfGrounded()) {

            Debug.Log("Player has landed. Gravity reset to " + gravityStorage + "\n Speed reset to: " + velocityStorage); ;
            playerCar.gravityMultiplier = gravityStorage;
            velocityStorage = playerCar.CurrentSpeed;

            playerCar = null;
            flyingHigh = false;

            //@TODO ADD SOUND FOR LANDING
        }
    }

    private void OnTriggerExit(Collider player) {
        if (behavior == TriggerType.exit) {
            Debug.Log("Player Exited");
            ChangeGravityAndSpeed(player);
        }
    }

    private void OnTriggerEnter(Collider player) {
        if(behavior == TriggerType.enter){
            Debug.Log("Player Entered;");
            ChangeGravityAndSpeed(player);
        }
    }

    private void OnTriggerStay(Collider player) {
        if (behavior == TriggerType.stay) {
            Debug.Log("Player Lingering");
            if (alterSpeed) {
                playerCar.CurrentSpeed = temporarySpeed;
            }
            WriteDebug();
            playerCar.gravityMultiplier = temporaryGravityMultiplier;
            playerCar.CurrentSpeed = velocityStorage;
            flyingHigh = true;

        }
    }

    private void ChangeGravityAndSpeed(Collider player){
        playerCar = player.GetComponent<CarController>();
        gravityStorage = playerCar.gravityMultiplier;
        velocityStorage = playerCar.CurrentSpeed;

        if (playerCar != null && !flyingHigh) {

            if(alterSpeed){
            playerCar.CurrentSpeed = temporarySpeed;
            }
            
            if (!playerCar.IsGrounded) {
                WriteDebug();

                playerCar.gravityMultiplier = temporaryGravityMultiplier;
                playerCar.CurrentSpeed = velocityStorage;
                flyingHigh = true;
            }
        }
    }

    private void WriteDebug() {
        if (temporaryGravityMultiplier < gravityStorage) {
            Debug.Log("Gravity Decreased!");
        }
        else if (temporaryGravityMultiplier > gravityStorage) {
            Debug.Log("Gravity Increased!");
        }
        else { Debug.Log("Gravity was not changed."); }


        if (temporarySpeed < velocityStorage) {
            Debug.Log("Speed Decreased!");
        }
        else if (temporarySpeed > velocityStorage) {
            Debug.Log("Speed Increased!");
        }
        else { Debug.Log("Speed was not changed."); }

    }
}
