using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

public class ParticleTeleporter : MonoBehaviour
{
    public FlexCollider CollisionMachine;
    public Vector3 MinNewPosition;
    public Vector3 MaxNewPosition;
    public Vector3 NewVelocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public unsafe void TeleportParticle(int ParticleId)
    {
        var NewPosition = new Vector3(Random.Range(MinNewPosition.x, MaxNewPosition.x), Random.Range(MinNewPosition.y, MaxNewPosition.y), Random.Range(MinNewPosition.z, MaxNewPosition.z));

        CollisionMachine.Container.PBuf.Positions.data[ParticleId] = new Vector4(NewPosition.x, NewPosition.y, NewPosition.z, CollisionMachine.Container.PBuf.Positions.data[ParticleId].w);
        CollisionMachine.Container.PBuf.Velocities.data[ParticleId] = NewVelocity;
        //Debug.Log("working?");
    }
}
