using System;
using UnityEngine;
using UnityEngine.Timeline;

public class CarController : MonoBehaviour
{
    private Rigidbody carRB;
   
    
   
    
    public WheelColliders wheelColliders;
    public WheelMeshes wheelMeshes;


    private float speed;
    private float speedClamped;

    public float enginePower;
    public float brakePower;
    public float maxSpeed;

    public float gasInput;
    public float brakeInput;
    public float steeringInput;
    
    //int : 0 off, 1 starting, 1> running
    public int isEngineRunning;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        carRB = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = wheelColliders.RR.rpm * wheelColliders.RR.radius * 2f * Mathf.PI / 10f;
        speedClamped = Mathf.Lerp(speedClamped, speed, Time.deltaTime);
        CheckInputs();
        
        ApplyAcceleration();
       
        
        
    }

    void CheckInputs()
    {
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");
        
    }

    
    
    void ApplyAcceleration()
    {
        if (isEngineRunning > 1)
        {
            if (Mathf.Abs( speed )< maxSpeed) // apply acceleration to rear wheels if speed is less than max
            {
                wheelColliders.RR.motorTorque = enginePower * gasInput;
                wheelColliders.RL.motorTorque = enginePower * gasInput;
            }
            else
            {
                wheelColliders.RR.motorTorque = 0;
                wheelColliders.RL.motorTorque = 0;
            }
        }
    }

   

    void ApplyWheelPositions()
    {
        ApplyWheelPosition(wheelColliders.FR, wheelMeshes.FR);
        ApplyWheelPosition(wheelColliders.FL, wheelMeshes.FL);
        ApplyWheelPosition(wheelColliders.RR, wheelMeshes.RR);
        ApplyWheelPosition(wheelColliders.RL, wheelMeshes.RL);
    }

    void ApplyWheelPosition(WheelCollider wheelColl, MeshRenderer wheelMesh)
    {
        wheelColl.GetWorldPose(out Vector3 position, out Quaternion rotation);
        
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = rotation;
    }
    
   
}


[Serializable]
public class WheelColliders
{
    public WheelCollider FR;
    public WheelCollider FL;
    public WheelCollider RR;
    public WheelCollider RL;
}


[Serializable]
public class WheelMeshes
{
    public MeshRenderer FR;
    public MeshRenderer FL;
    public MeshRenderer RR;
    public MeshRenderer RL;
}

