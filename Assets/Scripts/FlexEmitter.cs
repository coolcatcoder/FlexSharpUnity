using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexEmitter : MonoBehaviour
{
    public int ParticlesPerTime = 1;
    public float SecondsBetween = 1;
    public float SecondsTillStop = 10;
    public bool emit = false;
    public float VelocityX = 0;
    public float VelocityY = 0;
    public float VelocityZ = 0;
    public float Spread = 3;
    public float InverseMass = 1;
    public bool debug = false;

    float SecondsUntilNext = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SecondsUntilNext -= Time.deltaTime;
        if (SecondsUntilNext <= 0 &! (SecondsTillStop <= 0))
        {
            SecondsTillStop -= Time.deltaTime;
            emit = true;
            SecondsUntilNext = SecondsBetween;
            if (debug)
            {
                Debug.Log("Particles Emitted!");
            }
        }
    }
}
