using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeCoupling : MonoBehaviour
{
    public FlexCollider CollisionMachine;
    public Rigidbody CollisionPhysics;
    public float AdditionalFloatiness = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public unsafe void AffectRigidBody(int ParticleId)
    {
        //CollisionMachine.Container.PBuf.Velocities.data[ParticleId] += (Vector3)VelocityChange;
        Vector3 direction = CollisionPhysics.transform.position - (Vector3)CollisionMachine.Container.PBuf.Positions.data[ParticleId];
        Vector3 force = direction.normalized * CollisionMachine.Container.PBuf.Velocities.data[ParticleId].magnitude;
        force.y += AdditionalFloatiness;
        CollisionPhysics.AddForceAtPosition(force, CollisionMachine.Container.PBuf.Positions.data[ParticleId]);
    }
}
