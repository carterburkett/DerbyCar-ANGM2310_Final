using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POVCamera : MonoBehaviour
{
    //Wrote this Script so there would be an option to follow the car behind.
    private GameObject lookedAt = null;
    private GameObject target = null;
    
    
    private Vector3 vel = Vector3.zero;
    private float currentVel = 0;
    private float lastFrameVel = 0;
    private float gForce;
    //Vector3 cameraPos;
    Vector2 cameraPos;
    Quaternion cameraRot;
    float accelFX = 0;


    //private float followPos;
    //private float followAngle;w
    private CarController pc;
    public float followSpeed;
    //public float rotationSpeed;
    private float followDistance;
    public float forwardDistance;
    public float angleMultiplier = 1;
    private Rigidbody rb;

    void Start()
    {
        lookedAt = GameObject.FindGameObjectWithTag("Player");
        //cameraPos = Camera.main.transform.position;
        cameraPos = new Vector2(2,0);
        cameraRot = Camera.main.transform.rotation;
        target = GameObject.FindGameObjectWithTag("CameraTarget");
        pc = FindObjectOfType<CarController>();

    }

    private void FixedUpdate() {
        rb = pc.GetComponent<Rigidbody>();
        
        currentVel = rb.velocity.magnitude;
        gForce = (currentVel - lastFrameVel) / (Time.deltaTime * (Physics.gravity.magnitude * pc.gravityMultiplier));
        lastFrameVel = currentVel;

        Vector3 updatedPos = target.transform.position - (target.transform.forward * cameraPos.x) + (target.transform.up * cameraPos.y);
        
        accelFX = Mathf.Lerp(accelFX, gForce * 3.5f, 2 * Time.deltaTime);
        this.transform.position = Vector3.Lerp(Camera.main.transform.position, target.transform.GetChild(0).transform.position, followSpeed * Time.deltaTime);
        followDistance = Mathf.Pow(Vector3.Distance(this.transform.position, updatedPos), forwardDistance);
    

        this.transform.position = Vector3.MoveTowards(this.transform.position, updatedPos, followDistance * Time.deltaTime);
        Camera.main.transform.localRotation = Quaternion.Lerp(Camera.main.transform.localRotation, Quaternion.Euler(-accelFX, 0,0), angleMultiplier * Time.deltaTime);
        this.transform.LookAt(target.transform);
        

        ////@TODO make Angle smoother after lerping - figure out how to fix the behavior...?

        //this.transform.LookAt(lookedAt.transform.position);
        
        //followPos = Mathf.Abs(Vector3.Distance(this.transform.position, target.transform.position) * followSpeed);
        //this.transform.position = Vector3.SmoothDamp(this.transform.position, target.transform.position, ref vel, followPos * Time.deltaTime);

        //followAngle = Mathf.Abs(Quaternion.Angle(this.transform.rotation, target.transform.rotation) * followSpeed);
        ////this.transform.rotation = Quaternion.LerpUnclamped(this.transform.rotation, target.transform.rotation, followAngle * Time.deltaTime);
        ////this.transform.rotation = SmoothDampQuaternion(this.transform.rotation, target.transform.rotation, ref vel, followAngle * Time.deltaTime);
        //this.transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(this.transform.rotation.eulerAngles, target.transform.eulerAngles, ref vel, followAngle * Time.deltaTime));
    }

    public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime) {
        Vector3 c = current.eulerAngles;
        Vector3 t = target.eulerAngles;
        return Quaternion.Euler(
          Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
          Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
          Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
        );
    }

}
