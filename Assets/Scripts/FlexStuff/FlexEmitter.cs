using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlexSharp;

public class FlexEmitter : MonoBehaviour
{
    public FlexContainer Container;
    public int ParticlesPerTime = 1;
    public float SecondsBetween = 1;
    public float SecondsTillStop = 10;
    public bool emit = false;
    public float VelocityX = 0;
    public float VelocityY = 0;
    public float VelocityZ = 0;
    public float Spread = 3;
    public float InverseMass = 1;
    public Color32[] Colours;
    public bool debug = false;

    float SecondsUntilNext = 0;

    string TestMessage = "woah";

    // Start is called before the first frame update
    void Start()
    {
        Container.MappingQueue += TestMethod;
    }

    // Update is called once per frame
    void Update()
    {
        SecondsUntilNext -= Time.deltaTime;
        if (SecondsUntilNext <= 0 &! (SecondsTillStop <= 0))
        {
            SecondsTillStop -= Time.deltaTime;
            Container.InbetweenQueue += EmitParticles;
            SecondsUntilNext = SecondsBetween;
            if (debug)
            {
                Debug.Log("Particles Emitted!");
            }
        }
    }

    void TestMethod()
    {
        Debug.Log(TestMessage);
        Container.MappingQueue -= TestMethod;
    }

    void EmitParticles()
    {
        Container.InbetweenQueue -= EmitParticles;
        unsafe
        {
            //int ActiveCount = Methods.NvFlexGetActiveCount(solver);

            for (int i = 0; i < ParticlesPerTime; i++)
            {
                Container.GBuffers.Positions.data[Container.CurrentSlot] = new Vector4(transform.position.x + Random.Range(-Spread, Spread), transform.position.y + Random.Range(-Spread, Spread), transform.position.z + Random.Range(-Spread, Spread), InverseMass);
                Container.GBuffers.Velocities.data[Container.CurrentSlot] = new Vector3(VelocityX, VelocityY, VelocityZ);
                Container.GBuffers.Phases.data[Container.CurrentSlot] = Methods.NvFlexMakePhaseWithChannels(0, (int)NvFlexPhase.eNvFlexPhaseSelfCollide | (int)NvFlexPhase.eNvFlexPhaseFluid, (int)NvFlexPhase.eNvFlexPhaseShapeChannel0);
                Container.ParticleColours[Container.CurrentSlot] = Colours[Random.Range(0,Colours.Length)];

                Container.SlotsUsed++;
                Mathf.Clamp(Container.SlotsUsed, 0, Container.MaxParticles);
                Container.CurrentSlot++;
                if (Container.CurrentSlot >= Container.MaxParticles - 1)
                {
                    Container.CurrentSlot = 0;
                }
            }
        }
    }
}
