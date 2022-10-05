using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLauncher : MonoBehaviour
{
    public FlexCollider CollisionMachine;
    public Vector3 VelocityChange;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public unsafe void LaunchParticle(int ParticleId)
    {
        CollisionMachine.Container.PBuf.Velocities.data[ParticleId] += VelocityChange;
    }
}
