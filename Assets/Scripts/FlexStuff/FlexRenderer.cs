using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlexSharp;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using UnityEngine.ParticleSystemJobs;
using Unity.Collections.LowLevel.Unsafe;

public class FlexRenderer : MonoBehaviour
{
    public FlexContainer Container;
    public ParticleSystem FluidRenderer;

    public bool Render = true;

    public bool BurstRender = false;

    public Vector3 MinRenderPos = new Vector3(-1000, -1000, -1000);
    public Vector3 MaxRenderPos = new Vector3(1000, 1000, 1000);

    bool FirstTime = true;

    UpdateParticlesJob job = new UpdateParticlesJob();
    //ParticleSystem.Particle[] mainThreadParticles;

    ParticleSystem.Particle[] mainThreadParticles;

    // Start is called before the first frame update
    void Start()
    {
        var FluidRendererMain = FluidRenderer.main;
        FluidRendererMain.maxParticles = Container.MaxParticles;

        Container.ParticleColours = new Color32[Container.MaxParticles];

        if (BurstRender)
        {
            Container.InbetweenQueue += RenderParticlesBurst;
            //Container.AfterSolverTickQueue += PlayParticles;
        }

        job.PColours = new NativeArray<Color32>(Container.ParticleColours.Length, Allocator.Persistent);

        //var Hope = GetComponent<ParticleSystemRenderer>();
        //Hope.allowOcclusionWhenDynamic = false;

        mainThreadParticles = new ParticleSystem.Particle[Container.MaxParticles];
        mainThreadParticles[0].position = MinRenderPos;
        mainThreadParticles[1].position = MaxRenderPos;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayParticles()
    {
        if (FirstTime)
        {
            FluidRenderer.Play();
            FirstTime = false;
        }
    }

    void RenderParticlesBurst()
    {
        if (Render)
        {
            unsafe
            {
                job.PosData = Container.PBuf.Positions.data;
                job.RenderRadius = Container.RParticleRadius;
                job.Slots = Container.SlotsUsed;
                job.PColours.CopyFrom(Container.ParticleColours);
            }
        }
    }

    void OnParticleUpdateJobScheduled()
    {
        if (Container.SlotsUsed > 1)
        {
            //mainThreadParticles = new ParticleSystem.Particle[Container.SlotsUsed];
            //mainThreadParticles[0].position = MinRenderPos;
            //mainThreadParticles[1].position = MaxRenderPos;

            FluidRenderer.SetParticles(mainThreadParticles, Container.SlotsUsed);
        }

        if (FluidRenderer.particleCount > 1)
        {
            job.Schedule(FluidRenderer).Complete();
        }
    }

    [BurstCompile]
    struct UpdateParticlesJob : IJobParticleSystem
    {
        [ReadOnly]
        [NativeDisableUnsafePtrRestriction]
        unsafe public Vector4* PosData;
        [ReadOnly]
        public float RenderRadius;
        [ReadOnly]
        public NativeArray<Color32> PColours;
        [ReadOnly]
        public int Slots;

        public void Execute(ParticleSystemJobData particles)
        {
            var PosX = particles.positions.x;
            var PosY = particles.positions.y;
            var PosZ = particles.positions.z;

            var Colours = particles.startColors;
            var SizeX = particles.sizes.x;
            //var SizeY = particles.sizes.y;
            //var SizeZ = particles.sizes.z;

            unsafe
            {
                for (int i = 0; i < Slots; i++)
                {
                    PosX[i] = PosData[i].x;
                    PosY[i] = PosData[i].y;
                    PosZ[i] = PosData[i].z;
                    SizeX[i] = RenderRadius;
                    //SizeY[i] = RenderRadius;
                    //SizeZ[i] = RenderRadius;
                    Colours[i] = PColours[i];
                }
            }
        }
    }

    void OnDisable()
    {
        job.PColours.Dispose();
    }
}
