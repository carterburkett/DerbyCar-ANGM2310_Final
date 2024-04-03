using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCannon : MonoBehaviour
{
    //@TODO MAKE CUSTOM MENU THIS ONE IS CROWDED
    public enum TriggerType { enter, exit };
    public enum TravelType { trajectory, lerp };

    [InspectorLabel("Triger Behavior")]
    [Tooltip("Decides whether the trigger activates on entry or exit")]
    public TriggerType behavior = TriggerType.exit;

    [InspectorLabel("Travel Behavior")]
    [Tooltip("DO NOT USE \n Decides whether the Exiting Object's Trajectory uses Force or Interpolation")]
    public TravelType flightType = TravelType.lerp;


    public bool playAudio;
    public AudioSource cannonSound = null;
    public bool useParticles;
    public ParticleSystem smokeEffect;

    private bool launch = false;
    CarController carController = null;
    Rigidbody cannonBall = null;
    
    private Vector3 startPos;
    public Vector3 destination;
    private Quaternion startRot;

    private float count = 0.0f;
    private float duration = 0.0f;
    public float flightTime;
    

    private Vector3 center;
    private Vector3 startRelCenter;
    private Vector3 destRelCenter;

    void Update() {
        if (launch && flightType == TravelType.lerp) { CannonLerp(); }
    }

    private void OnTriggerEnter(Collider player) {
        //Launch Player to destination
        //@INFO this is being designed with a cannon shooting straight or down in mind. If you're going to use this for an upwards shot, make sure the landing zone is sloped so you don't clip through it...
        //@TODO make a menu that allows me to choose between a the pos of a GameObject, or choose coordinates
        
        if(behavior == TriggerType.enter){
            CannonEffects();
            cannonBall = player.gameObject.GetComponent<Rigidbody>();
            carController = player.gameObject.GetComponent<CarController>();
            startPos = cannonBall.position;
            startRot = cannonBall.rotation;
            launch = true;
            if (launch && flightType == TravelType.trajectory) { TrajectoryCannon(); }
        }
    }

    private void OnTriggerExit(Collider player) { 
        if(behavior == TriggerType.exit) {
            CannonEffects();
            cannonBall = player.gameObject.GetComponent<Rigidbody>();
            carController = player.gameObject.GetComponent<CarController>();
            startPos = cannonBall.position;
            startRot = cannonBall.rotation;
            launch = true;
            if (launch && flightType == TravelType.trajectory) { TrajectoryCannon(); }
        }
    }

    void CannonLerp() {
        //@TODO make the cannonball spin like a bullet ? :o
        
        FindRelCenter(Vector3.up);
        cannonBall.velocity = Vector3.zero;
        float zRotation;
        
        
        if (duration <= 1.0f) {
            GameObject cameraPos = GameObject.FindGameObjectWithTag("MainCamera");

            count += Time.deltaTime;
            float rotDuration = count / flightTime;
            duration = count / flightTime;

            zRotation = Mathf.LerpAngle(startRot.eulerAngles.z, -360f, duration);
            //transform.LookAt(destination);
            
            duration = Mathf.Pow( Mathf.Pow(duration, .5f), duration +1);
            cannonBall.transform.position = Vector3.Slerp(startRelCenter, destRelCenter, duration);
            cannonBall.transform.position += center;

            //cannonBall.transform.localEulerAngles = Vector3.Slerp(startRot.eulerAngles, new Vector3 (0, 0, zRotation), rotDuration);
            cannonBall.transform.Rotate(0,0,zRotation, Space.Self);
            cameraPos.transform.rotation = Quaternion.FromToRotation(Vector3.forward, transform.up);
            Debug.Log("Cannon Fired");

            if(duration >= 1.0f){
                launch = false;
                count = 0.0f;
                duration = 0.0f;
                rotDuration = 0.0f;
                cannonBall.transform.rotation = new Quaternion(0,0,0,0);
            }
        }
    }

    void TrajectoryCannon() {
        Debug.Log("gravity is currently" + Physics.gravity);
        
        Vector3 forceApplied = Calculate3DTrajectory(startPos, destination);
        cannonBall.velocity = forceApplied;
    }

    public void FindRelCenter(Vector3 direction) {
        center = (startPos + destination) / .75f;
        center -= direction;
        startRelCenter = startPos - center;
        destRelCenter = destination - center;
    }

    private void CannonEffects() {
        if (smokeEffect != null && useParticles) {
            Debug.Log("Cannon Particle Played");
            smokeEffect.Play();
        }
        if (cannonSound != null && playAudio) {
            Debug.Log("Cannon Sound Played");
            cannonSound.Play();
        }
    }

    private void CannonExpansion(){ 
        //@TODO use this to make the cannon 'expand' when it fires for a second or so
    }

    public Vector3 Calculate3DTrajectory(Vector3 startPos, Vector3 destination) {
        Vector3 launchVelocity;

        Vector3 trajectory = destination - startPos;

        //trajectory.x = Mathf.Abs(startPos.x - destination.x);
        //trajectory.y = Mathf.Abs(startPos.y - destination.y);
        //trajectory.z = Mathf.Abs(startPos.z - destination.z);

        float angle = Vector3.Angle(startPos, destination);
        float radAng = angle;
        radAng = Mathf.Rad2Deg * angle;

        float dy = trajectory.y;
        trajectory.y = 0.0f;
        float dxdz = trajectory.magnitude;


        trajectory.y = dxdz * Mathf.Tan(radAng);
        dxdz += dy/ Mathf.Tan(radAng);


        float velocity = Mathf.Sqrt(dxdz * Physics.gravity.magnitude / Mathf.Sin(2 * radAng));


        launchVelocity = velocity * trajectory.normalized;



        return launchVelocity;
    }

}
