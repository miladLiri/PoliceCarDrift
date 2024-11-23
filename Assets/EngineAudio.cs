using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineAudio : MonoBehaviour
{
    public AudioSource runningSound;
    public float runningMaxVolume;
    public float runningMaxPitch;
    
    public AudioSource idleSound;
    public float idleMaxVolume;
    
    public float speedRatio;
    
    public bool isEngineRunning = false;

    public AudioSource startingSound;
    
    private CarController carController;
    
    // Start is called before the first frame update
    void Start()
    {
        carController = GetComponent<CarController>();
        idleSound.volume = 0;
        runningSound.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float speedSign=0;
        if (carController)
        {
            speedSign = Mathf.Sign(carController.GetSpeedRatio());
            speedRatio = Mathf.Abs(carController.GetSpeedRatio());
        }
        if (isEngineRunning)
        {
            // Set the volume of the idle sound based on speed ratio, between minimum and maximum values
            idleSound.volume = Mathf.Lerp(0.1f, idleMaxVolume, speedRatio);
            
            if (speedSign > 0)
            {
                // Set the running sound volume based on the speed ratio
                runningSound.volume = Mathf.Lerp(0.3f, runningMaxVolume, speedRatio);
                runningSound.pitch = Mathf.Lerp(runningSound.pitch, Mathf.Lerp(0.3f, runningMaxPitch, speedRatio), Time.deltaTime);
            }
        }
        else {
            idleSound.volume = 0;
            runningSound.volume = 0;
        }
    }
    
    public IEnumerator StartEngine()
    {
        startingSound.Play();
        carController.isEngineRunning = 1;
        yield return new WaitForSeconds(0.6f);
        isEngineRunning = true;
        yield return new WaitForSeconds(0.4f);
        carController.isEngineRunning = 2;
    }
}