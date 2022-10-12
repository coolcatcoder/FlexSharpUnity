using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class FlexRendererFast : MonoBehaviour
{
    public Mesh ParticleMesh;
    public int SubMeshIndex = 0;
    public Material ParticleMaterial;
    public Bounds RenderBounds;
    public MaterialPropertyBlock MaterialProperties;
    public ShadowCastingMode CastShadows;
    public bool ReceiveShadows;
    public int Layer;
    public Camera Cam;
    public LightProbeUsage LightProbeThing;
    public FlexContainer Container;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //[BurstCompile]
    unsafe struct DealWithMatrixStuff : IJobParallelFor
    {
        public NativeArray<float4x4> Matrices;
        [ReadOnly]
        [NativeDisableUnsafePtrRestriction]
        unsafe public Vector4* PosData;
        [ReadOnly]
        public float3 RenderRadius;

        public void Execute(int i)
        {
            Matrices[i] = float4x4.TRS(new float3(PosData[i].x, PosData[i].y, PosData[i].z), new quaternion(), RenderRadius);
        }
    }
}
