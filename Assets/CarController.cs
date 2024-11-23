using System;
using UnityEngine;
using UnityEngine.Timeline;

public class CarController : MonoBehaviour
{
    private Rigidbody carRB;
    private float slipAngle;
    private float speed;
    private float speedClamped;

    public bool sirenIsOn;
    private float sirenHalfInterval = 0.5f;
    private float sirenTimer;
    private bool sirenIsRedActive;

    public WheelColliders wheelColliders;
    public WheelMeshes wheelMeshes;
    public Sirens sirens;
   

   


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
        ApplyWheelPositions();
        ApplySiren();

    }

    void CheckInputs()
    {
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        // control siren activation
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!sirenIsOn)
                sirens.SirenSound.Play();
            else
                sirens.SirenSound.Stop();

            sirenIsOn = !sirenIsOn;
        }



    }

    void ApplySiren()
    {
        if (sirenIsOn)
        {
            sirenTimer += Time.deltaTime;

            if (sirenTimer > sirenHalfInterval) // siren half interval :
            {
                if (sirenIsRedActive) // turn on blue if red was on
                {
                    sirens.TurnRedOff();
                    sirens.TurnBlueOn();
                    sirenIsRedActive = false;
                }
                else // turn on red if blue was on
                {
                    sirens.TurnBlueOff();
                    sirens.TurnRedOn();
                    sirenIsRedActive = true;
                }

                sirenTimer -= sirenHalfInterval;
            }
        }
        else
        {
            sirenTimer = 0;
            sirens.SetOff();
        }
    }

    void ApplyAcceleration()
    {
        if (isEngineRunning > 1)
        {
            if (Mathf.Abs(speed) < maxSpeed) // apply acceleration to rear wheels if speed is less than max
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

    public float GetSpeedRatio()
    {

        // avoid extremely low speed ratios when the gas input is very small
        var gas = Mathf.Clamp(Mathf.Abs(gasInput), 0.5f, 1f);

        return speedClamped * gas / maxSpeed;
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



[Serializable]
public class Sirens
{
    public MeshRenderer RedMesh;
    public MeshRenderer BlueMesh;
    public Light RedLight;
    public Light BlueLight;

    public Color RedOn;
    public Color RedOff;

    public Color BlueOn;
    public Color BlueOff;

    public AudioSource SirenSound;




    public void SetOff()
    {
        RedLight.enabled = false;
        BlueLight.enabled = false;

        RedMesh.material.color = RedOff;
        BlueMesh.material.color = BlueOff;
    }

    public void TurnRedOn()
    {
        RedLight.enabled = true;
        RedMesh.material.color = RedOn;
    }

    public void TurnBlueOn()
    {
        BlueLight.enabled = true;
        BlueMesh.material.color = BlueOn;
    }

    public void TurnRedOff()
    {
        RedLight.enabled = false;
        RedMesh.material.color = RedOff;
    }

    public void TurnBlueOff()
    {
        BlueLight.enabled = false;
        BlueMesh.material.color = BlueOff;
    }
}


