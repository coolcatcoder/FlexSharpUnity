using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class ParticleLauncher : MonoBehaviour
{
    public FlexCollider CollisionMachine;
    public float3 VelocityChange;
    public bool RelativeTransformForward = false;
    public int BatchSize = 256;

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
        if (!RelativeTransformForward)
        {
            CollisionMachine.Container.PBuf.Velocities.data[ParticleId] += (Vector3)VelocityChange;
        }
        else
        {
            //BurstLaunch LaunchJob = new BurstLaunch();
            //LaunchJob.ParticleId = ParticleId;
            //LaunchJob.ParticleArray = (float3*)CollisionMachine.Container.PBuf.Velocities.data;
            //LaunchJob.Direction = transform.forward;
            //LaunchJob.Power = VelocityChange;
            //LaunchJob.Run();
            //LaunchHandle.Complete();
            CollisionMachine.Container.PBuf.Velocities.data[ParticleId] += (Vector3)((float3)transform.forward * VelocityChange);
        }
    }

    public unsafe void LaunchParticle(NativeList<int> ParticleIds)
    {
        if (!RelativeTransformForward)
        {
            BurstLaunch LaunchJob = new BurstLaunch();
            LaunchJob.ParticleIds = ParticleIds;
            LaunchJob.ParticleArray = (float3*)CollisionMachine.Container.PBuf.Velocities.data;
            LaunchJob.VelocityChange = VelocityChange;
            var LaunchHandle = LaunchJob.Schedule(ParticleIds.Length, BatchSize);
            LaunchHandle.Complete();

            //CollisionMachine.Container.PBuf.Velocities.data[ParticleId] += (Vector3)VelocityChange;
        }
        else
        {
            BurstLaunch LaunchJob = new BurstLaunch();
            LaunchJob.ParticleIds = ParticleIds;
            LaunchJob.ParticleArray = (float3*)CollisionMachine.Container.PBuf.Velocities.data;
            LaunchJob.VelocityChange = transform.forward * VelocityChange;
            var LaunchHandle = LaunchJob.Schedule(ParticleIds.Length, BatchSize);
            LaunchHandle.Complete();

            //CollisionMachine.Container.PBuf.Velocities.data[ParticleId] += (Vector3)((float3)transform.forward * VelocityChange);
        }
    }

    [BurstCompile]
    public unsafe struct BurstLaunch : IJobParallelFor
    {
        [ReadOnly]
        public NativeList<int> ParticleIds;

        [NativeDisableUnsafePtrRestriction]
        public float3* ParticleArray;

        [ReadOnly]
        public float3 VelocityChange;

        public void Execute(int i)
        {
            ParticleArray[ParticleIds[i]] += VelocityChange;
        }
    }
}
