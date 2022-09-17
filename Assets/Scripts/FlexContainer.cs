using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlexSharp;
using FlexSharpExt;
using System;
using Random = UnityEngine.Random;
using Methods = FlexSharp.Methods;
using ExtMethods = FlexSharpExt.Methods;
using System.Runtime.InteropServices;

public class FlexContainer : MonoBehaviour
{
    byte bfalse = Convert.ToByte(false);
    byte btrue = Convert.ToByte(true);

    public int MaxParticles = 1000;
    public int MaxDiffuseParticles = 0;
    public List<FlexEmitter> emitters;
    public ParticleSystem FluidRenderer;
    public Color32 ParticleColour;
    public Color32 TrackingColour;
    //public float InverseParticleMass = 1;
    public float RParticleRadius = 0.15f;
    public int Substeps = 1;

    [Header("Plane 1\n(remember planes are ax + by + cz + d = 0)")]
    public float Plane0Index0;
    public float Plane0Index1;
    public float Plane0Index2;
    public float Plane0Index3;
    public bool Plane0Wave;

    [Header("Plane 2")]
    public float Plane1Index0;
    public float Plane1Index1;
    public float Plane1Index2;
    public float Plane1Index3;
    public bool Plane1Wave;

    [Header("Plane 3")]
    public float Plane2Index0;
    public float Plane2Index1;
    public float Plane2Index2;
    public float Plane2Index3;
    public bool Plane2Wave;

    [Header("Plane 4")]
    public float Plane3Index0;
    public float Plane3Index1;
    public float Plane3Index2;
    public float Plane3Index3;
    public bool Plane3Wave;

    [Header("Plane 5")]
    public float Plane4Index0;
    public float Plane4Index1;
    public float Plane4Index2;
    public float Plane4Index3;
    public bool Plane4Wave;

    [Header("Plane 6")]
    public float Plane5Index0;
    public float Plane5Index1;
    public float Plane5Index2;
    public float Plane5Index3;
    public bool Plane5Wave;

    [Header("Plane 7")]
    public float Plane6Index0;
    public float Plane6Index1;
    public float Plane6Index2;
    public float Plane6Index3;
    public bool Plane6Wave;

    [Header("Plane 8")]
    public float Plane7Index0;
    public float Plane7Index1;
    public float Plane7Index2;
    public float Plane7Index3;
    public bool Plane7Wave;

    unsafe NvFlexLibrary* library;
    NvFlexSolverDesc solverDesc;
    unsafe NvFlexSolver* solver;
    public NvFlexParams SolverParams;

    unsafe NvFlexBuffer* particleBuffer;
    unsafe NvFlexBuffer* velocityBuffer;
    unsafe NvFlexBuffer* phaseBuffer;
    unsafe NvFlexBuffer* geometryBuffer;
    unsafe NvFlexBuffer* positionsBuffer;
    unsafe NvFlexBuffer* rotationsBuffer;
    unsafe NvFlexBuffer* flagsBuffer;

    unsafe Vector4* particles;
    unsafe Vector3* velocities;
    unsafe int* phases;
    unsafe NvFlexCollisionGeometry* geometry;
    unsafe Vector4* positions;
    unsafe XQuat<float>* rotations;
    unsafe int* flags;

    int SlotsUsed;
    int CurrentSlot;

    bool ShapesChanged = true;
    FlexCollider[] Shapes;

    [StructLayout(LayoutKind.Sequential)]
    public struct XQuat<T>
    {
        public T x, y, z, w;
    }

    unsafe public struct FVector<T>
        where T : unmanaged
    {
        public NvFlexLibrary* lib;
        public int size;
        public NvFlexBufferType type;
        public NvFlexBuffer* buffer;
        public T* data;

        public FVector(NvFlexLibrary* lib, int size, NvFlexBuffer* buffer = default, T* data = default, NvFlexBufferType type = NvFlexBufferType.eNvFlexBufferHost)
        {
            this.lib = lib;
            this.size = size;
            this.type = type;
            this.buffer = buffer;
            this.data = data;
        }

        public void InitVec()
        {
            buffer = Methods.NvFlexAllocBuffer(lib, size, sizeof(T), type);
        }

        public void MapVec()
        {
            data = (T*)Methods.NvFlexMap(buffer, (int)NvFlexMapFlags.eNvFlexMapWait);
        }

        public void UnmapVec()
        {
            Methods.NvFlexUnmap(buffer);
        }

        public void Destroy()
        {
            Methods.NvFlexFreeBuffer(buffer);
        }
    }

    public struct SimBuffers //simulation buffers
    {
        unsafe public NvFlexSolver* solver;

        public FVector<Vector4> Positions;
        public FVector<Vector3> Velocities;
        public FVector<int> Phases;

        //public FVector<NvFlexCollisionGeometry> ShapeGeometry;
        //public FVector<Vector4> ShapePositions;
        //public FVector<XQuat<float>> ShapeRotations;
        //public FVector<int> ShapeFlags;

        public void InitVectors()
        {
            Positions.InitVec();
            Velocities.InitVec();
            Phases.InitVec();
        }

        public void MapVectors()
        {
            Positions.MapVec();
            Velocities.MapVec();
            Phases.MapVec();
        }

        public void UnmapVectors()
        {
            Positions.UnmapVec();
            Velocities.UnmapVec();
            Phases.UnmapVec();
        }

        unsafe public void SendBuffers()
        {
            Methods.NvFlexSetParticles(solver, Positions.buffer, null);
            Methods.NvFlexSetVelocities(solver, Velocities.buffer, null);
            Methods.NvFlexSetPhases(solver, Phases.buffer, null);
        }

        unsafe public void GetBuffers()
        {
            Methods.NvFlexGetParticles(solver, Positions.buffer, null);
            Methods.NvFlexGetVelocities(solver, Velocities.buffer, null);
            Methods.NvFlexGetPhases(solver, Phases.buffer, null);
        }

        public void DestroyVectors()
        {
            Positions.Destroy();
            Velocities.Destroy();
            Phases.Destroy();
        }
    }

    public struct int3
    {
        public int x, y, z;
    }

    SimBuffers GBuffers = new SimBuffers();

    // Start is called before the first frame update
    void Start()
    {

        Shapes = (FlexCollider[])FindObjectsOfType(typeof(FlexCollider));

        unsafe
        {
            library = Methods.NvFlexInit();
            fixed(NvFlexSolverDesc* solverDescPtr = &solverDesc) { Methods.NvFlexSetSolverDescDefaults(solverDescPtr); }
            solverDesc.maxParticles = MaxParticles;
            solverDesc.maxDiffuseParticles = MaxDiffuseParticles;
            //solverDesc.featureMode = NvFlexFeatureMode.eNvFlexFeatureModeSimpleFluids;

            fixed (NvFlexSolverDesc* solverDescPtr = &solverDesc) { solver = Methods.NvFlexCreateSolver(library, solverDescPtr); }

            AssignPlanes();
            fixed (NvFlexParams* SolverParamsPtr = &SolverParams) { Methods.NvFlexSetParams(solver, SolverParamsPtr); }
            Debug.Log(SolverParams.gravity[1]);

            GBuffers.solver = solver;

            GBuffers.Positions = new FVector<Vector4>(library, MaxParticles);
            GBuffers.Velocities = new FVector<Vector3>(library, MaxParticles);
            GBuffers.Phases = new FVector<int>(library, MaxParticles);

            GBuffers.InitVectors();

            //particleBuffer = Methods.NvFlexAllocBuffer(library, MaxParticles, 16, NvFlexBufferType.eNvFlexBufferHost);
            //velocityBuffer = Methods.NvFlexAllocBuffer(library, MaxParticles, 16, NvFlexBufferType.eNvFlexBufferHost);
            //phaseBuffer = Methods.NvFlexAllocBuffer(library, MaxParticles, 4, NvFlexBufferType.eNvFlexBufferHost);

            geometryBuffer = Methods.NvFlexAllocBuffer(library, Shapes.Length, sizeof(NvFlexCollisionGeometry), NvFlexBufferType.eNvFlexBufferHost);
            positionsBuffer = Methods.NvFlexAllocBuffer(library, Shapes.Length, sizeof(float) * 4, NvFlexBufferType.eNvFlexBufferHost);
            rotationsBuffer = Methods.NvFlexAllocBuffer(library, Shapes.Length, sizeof(XQuat<float>), NvFlexBufferType.eNvFlexBufferHost);
            flagsBuffer = Methods.NvFlexAllocBuffer(library, Shapes.Length, sizeof(int), NvFlexBufferType.eNvFlexBufferHost);
        }

        InitWeirdShapes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        unsafe
        {
            GBuffers.MapVectors();

            //particles = (Vector4*)Methods.NvFlexMap(particleBuffer, (int)NvFlexMapFlags.eNvFlexMapWait);
            //velocities = (Vector3*)Methods.NvFlexMap(velocityBuffer, (int)NvFlexMapFlags.eNvFlexMapWait);
            //phases = (int*)Methods.NvFlexMap(phaseBuffer, (int)NvFlexMapFlags.eNvFlexMapWait);

            geometry = (NvFlexCollisionGeometry*)Methods.NvFlexMap(geometryBuffer, (int)NvFlexMapFlags.eNvFlexMapWait);
            positions = (Vector4*)Methods.NvFlexMap(positionsBuffer, (int)NvFlexMapFlags.eNvFlexMapWait);
            rotations = (XQuat<float>*)Methods.NvFlexMap(rotationsBuffer, (int)NvFlexMapFlags.eNvFlexMapWait);
            flags = (int*)Methods.NvFlexMap(flagsBuffer, (int)NvFlexMapFlags.eNvFlexMapWait);

            EmitParticles();
            RenderParticles();
            DealWithShapes();
            WavePlanes();
            fixed (NvFlexParams* SolverParamsPtr = &SolverParams) { Methods.NvFlexSetParams(solver, SolverParamsPtr); }

            GBuffers.UnmapVectors();

            //Methods.NvFlexUnmap(particleBuffer);
            //Methods.NvFlexUnmap(velocityBuffer);
            //Methods.NvFlexUnmap(phaseBuffer);

            Methods.NvFlexUnmap(geometryBuffer);
            Methods.NvFlexUnmap(positionsBuffer);
            Methods.NvFlexUnmap(rotationsBuffer);
            Methods.NvFlexUnmap(flagsBuffer);

            GBuffers.SendBuffers();

            //Methods.NvFlexSetParticles(solver, particleBuffer, null);
            //Methods.NvFlexSetVelocities(solver, velocityBuffer, null);
            //Methods.NvFlexSetPhases(solver, phaseBuffer, null);

            if (ShapesChanged)
            {
                Methods.NvFlexSetShapes(solver, geometryBuffer, positionsBuffer, rotationsBuffer, null, null, flagsBuffer, Shapes.Length);
            }
            

            Methods.NvFlexSetActiveCount(solver, SlotsUsed);

            Methods.NvFlexUpdateSolver(solver, Time.deltaTime, Substeps, bfalse);

            GBuffers.GetBuffers();

            //Methods.NvFlexGetParticles(solver, particleBuffer, null);
            //Methods.NvFlexGetVelocities(solver, velocityBuffer, null);
            //Methods.NvFlexGetPhases(solver, phaseBuffer, null);
        }
    }

    void OnDisable()
    {
        Debug.Log("attempting shutdown");
        unsafe { Debug.Log(Methods.NvFlexGetActiveCount(solver)); }
        unsafe
        {
            DestroyWeirdShapes();

            GBuffers.DestroyVectors();
            //Methods.NvFlexFreeBuffer(particleBuffer);
            //Methods.NvFlexFreeBuffer(velocityBuffer);
            //Methods.NvFlexFreeBuffer(phaseBuffer);

            Methods.NvFlexFreeBuffer(geometryBuffer);
            Methods.NvFlexFreeBuffer(positionsBuffer);
            Methods.NvFlexFreeBuffer(rotationsBuffer);
            Methods.NvFlexFreeBuffer(flagsBuffer);

            Methods.NvFlexDestroySolver(solver);
            Methods.NvFlexShutdown(library);
        }
    }

    void EmitParticles()
    {
        foreach (FlexEmitter emitter in emitters)
        {
            if (emitter.emit)
            {
                emitter.emit = false;
                unsafe
                {
                    //int ActiveCount = Methods.NvFlexGetActiveCount(solver);

                    for (int i = 0; i < emitter.ParticlesPerTime; i++)
                    {
                        GBuffers.Positions.data[CurrentSlot] = new Vector4(emitter.transform.position.x + Random.Range(-emitter.Spread, emitter.Spread), emitter.transform.position.y + Random.Range(-emitter.Spread, emitter.Spread), emitter.transform.position.z + Random.Range(-emitter.Spread, emitter.Spread), emitter.InverseMass);
                        GBuffers.Velocities.data[CurrentSlot] = new Vector3(emitter.VelocityX, emitter.VelocityY, emitter.VelocityZ);
                        GBuffers.Phases.data[CurrentSlot] = Methods.NvFlexMakePhaseWithChannels(0, (int)NvFlexPhase.eNvFlexPhaseSelfCollide | (int)NvFlexPhase.eNvFlexPhaseFluid, (int)NvFlexPhase.eNvFlexPhaseShapeChannel0);
                        
                        SlotsUsed++;
                        Mathf.Clamp(SlotsUsed, 0, MaxParticles);
                        CurrentSlot++;
                        if (CurrentSlot >= MaxParticles)
                        {
                            CurrentSlot = 0;
                        }
                    }
                }
            }
        }
    }

    void RenderParticles()
    {
        var RParticles = new ParticleSystem.Particle[SlotsUsed];
        var FluidRendererMain = FluidRenderer.main;
        FluidRendererMain.maxParticles = MaxParticles;

        unsafe
        {
            for (int i = 0; i < RParticles.Length; i++)
            {
                RParticles[i].position = GBuffers.Positions.data[i];
                RParticles[i].startSize = RParticleRadius;
                if (i == 2)
                {
                    RParticles[i].startColor = TrackingColour;
                }
                else
                {
                    RParticles[i].startColor = ParticleColour;
                }

            }
        }
        FluidRenderer.SetParticles(RParticles);
    }

    void AssignPlanes()
    {
        unsafe
        {
            //plane stuff

            SolverParams.planes[4 * 0 + 0] = Plane0Index0;
            SolverParams.planes[4 * 0 + 1] = Plane0Index1;
            SolverParams.planes[4 * 0 + 2] = Plane0Index2;
            SolverParams.planes[4 * 0 + 3] = Plane0Index3;

            SolverParams.planes[4 * 1 + 0] = Plane1Index0;
            SolverParams.planes[4 * 1 + 1] = Plane1Index1;
            SolverParams.planes[4 * 1 + 2] = Plane1Index2;
            SolverParams.planes[4 * 1 + 3] = Plane1Index3;

            SolverParams.planes[4 * 2 + 0] = Plane2Index0;
            SolverParams.planes[4 * 2 + 1] = Plane2Index1;
            SolverParams.planes[4 * 2 + 2] = Plane2Index2;
            SolverParams.planes[4 * 2 + 3] = Plane2Index3;

            SolverParams.planes[4 * 3 + 0] = Plane3Index0;
            SolverParams.planes[4 * 3 + 1] = Plane3Index1;
            SolverParams.planes[4 * 3 + 2] = Plane3Index2;
            SolverParams.planes[4 * 3 + 3] = Plane3Index3;

            SolverParams.planes[4 * 4 + 0] = Plane4Index0;
            SolverParams.planes[4 * 4 + 1] = Plane4Index1;
            SolverParams.planes[4 * 4 + 2] = Plane4Index2;
            SolverParams.planes[4 * 4 + 3] = Plane4Index3;
        }
    }

    void InitWeirdShapes()
    {
        unsafe
        {
            for (int i = 0; i < Shapes.Length; i++)
            {
                switch (Shapes[i].Shape)
                {
                    case NvFlexCollisionShapeType.eNvFlexShapeBox:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeCapsule:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeConvexMesh:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeSDF:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeSphere:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeTriangleMesh:
                        Shapes[i].MeshId = Methods.NvFlexCreateTriangleMesh(library);

                        Shapes[i].Vertices = Methods.NvFlexAllocBuffer(library, Shapes[i].TriMesh.vertices.Length, sizeof(Vector3), NvFlexBufferType.eNvFlexBufferHost);
                        Shapes[i].Indices = Methods.NvFlexAllocBuffer(library, Shapes[i].TriMesh.triangles.Length, sizeof(int), NvFlexBufferType.eNvFlexBufferHost);
                        break;
                }
            }
        }
    }

    void DealWithShapes()
    {
        unsafe
        {
            for (int i = 0; i < Shapes.Length; i++)
            {
                if (Shapes[i].transform.hasChanged)
                {
                    //Debug.Log("Shapes have changed.");
                    ShapesChanged = true;
                    Shapes[i].transform.hasChanged = false;

                    var rotation = new XQuat<float>();
                    rotation.w = Shapes[i].transform.rotation.w;
                    rotation.x = Shapes[i].transform.rotation.x;
                    rotation.y = Shapes[i].transform.rotation.y;
                    rotation.z = Shapes[i].transform.rotation.z;
                    rotations[i] = rotation;

                    positions[i] = new Vector4(Shapes[i].transform.position.x, Shapes[i].transform.position.y, Shapes[i].transform.position.z, Shapes[i].ShapeMysteryPower);

                    flags[i] = Methods.NvFlexMakeShapeFlagsWithChannels(Shapes[i].Shape, Shapes[i].Dynamic, (int)Shapes[i].PhaseSettings);
                    if (Shapes[i].Trigger) { flags[i] |= (int)NvFlexCollisionShapeFlags.eNvFlexShapeFlagTrigger; }

                    switch (Shapes[i].Shape)
                    {
                        case NvFlexCollisionShapeType.eNvFlexShapeBox:
                            geometry[i].box.halfExtents[0] = Shapes[i].transform.lossyScale.x / 2;
                            geometry[i].box.halfExtents[1] = Shapes[i].transform.lossyScale.y / 2;
                            geometry[i].box.halfExtents[2] = Shapes[i].transform.lossyScale.z / 2;
                            break;
                        case NvFlexCollisionShapeType.eNvFlexShapeCapsule:
                            break;
                        case NvFlexCollisionShapeType.eNvFlexShapeConvexMesh:
                            break;
                        case NvFlexCollisionShapeType.eNvFlexShapeSDF:
                            break;
                        case NvFlexCollisionShapeType.eNvFlexShapeSphere:
                            geometry[i].sphere.radius = Shapes[i].transform.lossyScale.x / 2;
                            break;
                        case NvFlexCollisionShapeType.eNvFlexShapeTriangleMesh:
                            var RWIndices = (int*)Methods.NvFlexMap(Shapes[i].Indices, (int)NvFlexMapFlags.eNvFlexMapWait);
                            var RWVertices = (Vector3*)Methods.NvFlexMap(Shapes[i].Vertices, (int)NvFlexMapFlags.eNvFlexMapWait);

                            //RWVertices = (Vector3*)Shapes[i].TriMesh.vertices.;
                            //Array.Copy(Shapes[i].TriMesh.vertices, (System.Array)RWVertices, Shapes[i].TriMesh.vertices.Length);

                            for (int k = 0; k < Shapes[i].TriMesh.vertices.Length; k++)
                            {
                                RWVertices[k] = Shapes[i].TriMesh.vertices[i];
                            }

                            for (int k = 0; k < Shapes[i].TriMesh.triangles.Length; k++)
                            {
                                RWIndices[k] = Shapes[i].TriMesh.triangles[i];
                            }

                            Methods.NvFlexUnmap(Shapes[i].Indices);
                            Methods.NvFlexUnmap(Shapes[i].Vertices);

                            var min = Shapes[i].TriMesh.bounds.min;
                            var LowerBoundsPtr = &min;

                            var max = Shapes[i].TriMesh.bounds.max;
                            var UpperBoundsPtr = &max;

                            Methods.NvFlexUpdateTriangleMesh(library, Shapes[i].MeshId, Shapes[i].Vertices, Shapes[i].Indices, Shapes[i].TriMesh.vertices.Length, Shapes[i].TriMesh.triangles.Length/3, (float*)LowerBoundsPtr, (float*)UpperBoundsPtr);

                            geometry[i].triMesh.mesh = Shapes[i].MeshId;
                            geometry[i].triMesh.scale[0] = Shapes[i].transform.lossyScale.x;
                            geometry[i].triMesh.scale[1] = Shapes[i].transform.lossyScale.y;
                            geometry[i].triMesh.scale[2] = Shapes[i].transform.lossyScale.z;

                            break;
                    }
                }
            }
        }
    }

    void DestroyWeirdShapes()
    {
        unsafe
        {
            for (int i = 0; i < Shapes.Length; i++)
            {
                switch (Shapes[i].Shape)
                {
                    case NvFlexCollisionShapeType.eNvFlexShapeBox:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeCapsule:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeConvexMesh:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeSDF:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeSphere:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeTriangleMesh:
                        Methods.NvFlexDestroyTriangleMesh(library, Shapes[i].MeshId);

                        Methods.NvFlexFreeBuffer(Shapes[i].Vertices);
                        Methods.NvFlexFreeBuffer(Shapes[i].Indices);
                        break;
                }
            }
        }
    }

    void WavePlanes()
    {
        if (Plane0Wave) { WavePlane(0); }
        if (Plane1Wave) { WavePlane(1); }
        if (Plane2Wave) { WavePlane(2); }
        if (Plane3Wave) { WavePlane(3); }
        if (Plane4Wave) { WavePlane(4); }
        if (Plane5Wave) { WavePlane(5); }
        if (Plane6Wave) { WavePlane(6); }
        if (Plane7Wave) { WavePlane(7); }
    }

    void WavePlane(int plane)
    {
        unsafe
        {
            SolverParams.planes[4 * plane + 3] = Mathf.Sin(Time.fixedTime) * 3;
        }
    }
}
