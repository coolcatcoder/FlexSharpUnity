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
using UnityEditor;

public class FlexContainer : MonoBehaviour
{
    byte bfalse = Convert.ToByte(false);
    byte btrue = Convert.ToByte(true);

    public int MaxParticles = 1000;
    public int MaxDiffuseParticles = 0;
    [System.NonSerialized]
    public Color32[] ParticleColours;
    [System.NonSerialized]
    public int[] ExtraData;
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

    unsafe public NvFlexLibrary* Library;
    NvFlexSolverDesc SolverDesc;
    unsafe NvFlexSolver* Solver;
    public NvFlexParams SolverParams;

    public int SlotsUsed;
    public int CurrentSlot;

    //bool ShapesChanged = true;
    //FlexCollider[] Shapes;

    bool FirstTime = true;

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

    public struct ParticleBuffers
    {
        unsafe public NvFlexSolver* Solver;

        public FVector<Vector4> Positions;
        public FVector<Vector3> Velocities;
        public FVector<int> Phases;

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
            Methods.NvFlexSetParticles(Solver, Positions.buffer, null);
            Methods.NvFlexSetVelocities(Solver, Velocities.buffer, null);
            Methods.NvFlexSetPhases(Solver, Phases.buffer, null);
        }

        unsafe public void GetBuffers()
        {
            Methods.NvFlexGetParticles(Solver, Positions.buffer, null);
            Methods.NvFlexGetVelocities(Solver, Velocities.buffer, null);
            Methods.NvFlexGetPhases(Solver, Phases.buffer, null);
        }

        public void DestroyVectors()
        {
            Positions.Destroy();
            Velocities.Destroy();
            Phases.Destroy();
        }
    }

    public struct ShapeBuffers
    {
        unsafe public NvFlexSolver* Solver;

        public FVector<NvFlexCollisionGeometry> Geometry;
        public FVector<Vector4> Positions;
        public FVector<XQuat<float>> Rotations;
        public FVector<Vector4> PrevPositions;
        public FVector<XQuat<float>> PrevRotations;
        public FVector<int> Flags;

        public FVector<Vector4> ContactPlanes;
        public FVector<Vector4> ContactVelocities;
        public FVector<int> ContactIndices;
        public FVector<uint> ContactCounts;

        public bool ShapesChanged;
        public int NumShapes;

        public void InitVectors()
        {
            Geometry.InitVec();
            Positions.InitVec();
            Rotations.InitVec();
            PrevPositions.InitVec();
            PrevRotations.InitVec();
            Flags.InitVec();

            ContactPlanes.InitVec();
            ContactVelocities.InitVec();
            ContactIndices.InitVec();
            ContactCounts.InitVec();
        }

        public void MapVectors()
        {
            Geometry.MapVec();
            Positions.MapVec();
            Rotations.MapVec();
            PrevPositions.MapVec();
            PrevRotations.MapVec();
            Flags.MapVec();

            ContactPlanes.MapVec();
            ContactVelocities.MapVec();
            ContactIndices.MapVec();
            ContactCounts.MapVec();
        }

        public void UnmapVectors()
        {
            Geometry.UnmapVec();
            Positions.UnmapVec();
            Rotations.UnmapVec();
            PrevPositions.UnmapVec();
            PrevRotations.UnmapVec();
            Flags.UnmapVec();

            ContactPlanes.UnmapVec();
            ContactVelocities.UnmapVec();
            ContactIndices.UnmapVec();
            ContactCounts.UnmapVec();
        }

        unsafe public void SendBuffers()
        {
            if (ShapesChanged)
            {
                Methods.NvFlexSetShapes(Solver, Geometry.buffer, Positions.buffer, Rotations.buffer, PrevPositions.buffer, PrevRotations.buffer, Flags.buffer, NumShapes);
            }
            
        }

        unsafe public void GetBuffers()
        {
            //Debug.Log("?????? YOU CANT GET SHAPES, THIS FUNCTION SHOULD NEVER RUN!!!!!!!!!!!!!!!!!!!"); Removed cause you can get contacts, so GetBuffers has a purpose now...
            Methods.NvFlexGetContacts(Solver, ContactPlanes.buffer, ContactVelocities.buffer, ContactIndices.buffer, ContactCounts.buffer);
        }

        public void DestroyVectors()
        {
            Geometry.Destroy();
            Positions.Destroy();
            Rotations.Destroy();
            PrevPositions.Destroy();
            PrevRotations.Destroy();
            Flags.Destroy();

            ContactPlanes.Destroy();
            ContactVelocities.Destroy();
            ContactIndices.Destroy();
            ContactCounts.Destroy();
        }

        public int AddShape()
        {
            NumShapes += 1;
            return NumShapes - 1;
        }
    }

    public ParticleBuffers PBuf = new ParticleBuffers();
    public ShapeBuffers SBuf = new ShapeBuffers();

    public Action MappingQueue;
    public Action InbetweenQueue;
    public Action UnmappingQueue;
    public Action BeforeSolverTickQueue;
    public Action AfterSolverTickQueue;
    public Action DestroyQueue;

    bool FixedDone = true;

    // Start is called before the first frame update
    void Start()
    {
        ExtraData = new int[MaxParticles];
        ParticleColours = new Color32[MaxParticles];
        unsafe
        {
            Library = Methods.NvFlexInit();
            fixed(NvFlexSolverDesc* SolverDescPtr = &SolverDesc) { Methods.NvFlexSetSolverDescDefaults(SolverDescPtr); }
            SolverDesc.maxParticles = MaxParticles;
            SolverDesc.maxDiffuseParticles = MaxDiffuseParticles;

            fixed (NvFlexSolverDesc* solverDescPtr = &SolverDesc) { Solver = Methods.NvFlexCreateSolver(Library, solverDescPtr); }

            AssignPlanes();
            fixed (NvFlexParams* SolverParamsPtr = &SolverParams) { Methods.NvFlexSetParams(Solver, SolverParamsPtr); }
            Debug.Log(SolverParams.gravity[1]);

            PBuf.Solver = Solver;

            PBuf.Positions = new FVector<Vector4>(Library, MaxParticles);
            PBuf.Velocities = new FVector<Vector3>(Library, MaxParticles);
            PBuf.Phases = new FVector<int>(Library, MaxParticles);

            PBuf.InitVectors();
        }
    }

    void FixedUpdate()
    {
        if (!FixedDone)
        {
            Debug.Log("Warning, fixed updates are colliding!");
        }

        FixedDone = false;

        unsafe
        {
            if (FirstTime && (SBuf.NumShapes!=0))
            {
                FirstTime = false;

                SBuf.Solver = Solver;

                SBuf.Geometry = new FVector<NvFlexCollisionGeometry>(Library, SBuf.NumShapes);
                SBuf.Positions = new FVector<Vector4>(Library, SBuf.NumShapes);
                SBuf.Rotations = new FVector<XQuat<float>>(Library, SBuf.NumShapes);
                SBuf.PrevPositions = new FVector<Vector4>(Library, SBuf.NumShapes);
                SBuf.PrevRotations = new FVector<XQuat<float>>(Library, SBuf.NumShapes);
                SBuf.Flags = new FVector<int>(Library, SBuf.NumShapes);

                SBuf.ContactPlanes = new FVector<Vector4>(Library, MaxParticles * 6); // 6 appears to be the max amount of contacts a particle is allowed to have according to triggervolume.h in the demo app
                SBuf.ContactVelocities = new FVector<Vector4>(Library, MaxParticles * 6);
                SBuf.ContactIndices = new FVector<int>(Library, MaxParticles);
                SBuf.ContactCounts = new FVector<uint>(Library, MaxParticles);

                SBuf.InitVectors();
            }



            PBuf.MapVectors();
            SBuf.MapVectors();

            MappingQueue?.Invoke();

            InbetweenQueue?.Invoke();

            WavePlanes();
            fixed (NvFlexParams* SolverParamsPtr = &SolverParams) { Methods.NvFlexSetParams(Solver, SolverParamsPtr); }

            UnmappingQueue?.Invoke();

            PBuf.UnmapVectors();
            SBuf.UnmapVectors();

            PBuf.SendBuffers();
            SBuf.SendBuffers();

            Methods.NvFlexSetActiveCount(Solver, SlotsUsed);

            BeforeSolverTickQueue?.Invoke();

            Methods.NvFlexUpdateSolver(Solver, Time.deltaTime, Substeps, bfalse);

            AfterSolverTickQueue?.Invoke();

            PBuf.GetBuffers();
            SBuf.GetBuffers();
        }

        FixedDone = true;
    }

    void OnDisable()
    {
        Debug.Log("attempting shutdown");
        unsafe { Debug.Log(Methods.NvFlexGetActiveCount(Solver)); }
        unsafe
        {
            PBuf.DestroyVectors();
            SBuf.DestroyVectors();

            DestroyQueue?.Invoke();

            Methods.NvFlexDestroySolver(Solver);
            Methods.NvFlexShutdown(Library);
        }
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

    //void InitWeirdShapes()
    //{
    //    unsafe
    //    {
    //        for (int i = 0; i < Shapes.Length; i++)
    //        {
    //            switch (Shapes[i].Shape)
    //            {
    //                case NvFlexCollisionShapeType.eNvFlexShapeBox:
    //                    break;
    //                case NvFlexCollisionShapeType.eNvFlexShapeCapsule:
    //                    break;
    //                case NvFlexCollisionShapeType.eNvFlexShapeConvexMesh:
    //                    break;
    //                case NvFlexCollisionShapeType.eNvFlexShapeSDF:
    //                    break;
    //                case NvFlexCollisionShapeType.eNvFlexShapeSphere:
    //                    break;
    //                case NvFlexCollisionShapeType.eNvFlexShapeTriangleMesh:
    //                    Shapes[i].MeshId = Methods.NvFlexCreateTriangleMesh(Library);
    //                    Debug.Log("meshid_success");

    //                    Shapes[i].Vertices = Methods.NvFlexAllocBuffer(Library, Shapes[i].TriMesh.vertices.Length, sizeof(Vector3), NvFlexBufferType.eNvFlexBufferHost);
    //                    Debug.Log("vert_success");

    //                    Shapes[i].Indices = Methods.NvFlexAllocBuffer(Library, Shapes[i].TriMesh.triangles.Length, sizeof(int), NvFlexBufferType.eNvFlexBufferHost);
    //                    Debug.Log("indices_success");

    //                    break;
    //            }
    //        }
    //    }
    //}

    //void DestroyWeirdShapes()
    //{
    //    unsafe
    //    {
    //        for (int i = 0; i < Shapes.Length; i++)
    //        {
    //            switch (Shapes[i].Shape)
    //            {
    //                case NvFlexCollisionShapeType.eNvFlexShapeBox:
    //                    break;
    //                case NvFlexCollisionShapeType.eNvFlexShapeCapsule:
    //                    break;
    //                case NvFlexCollisionShapeType.eNvFlexShapeConvexMesh:
    //                    break;
    //                case NvFlexCollisionShapeType.eNvFlexShapeSDF:
    //                    break;
    //                case NvFlexCollisionShapeType.eNvFlexShapeSphere:
    //                    break;
    //                case NvFlexCollisionShapeType.eNvFlexShapeTriangleMesh:
    //                    Methods.NvFlexDestroyTriangleMesh(Library, Shapes[i].MeshId);

    //                    Methods.NvFlexFreeBuffer(Shapes[i].Vertices);
    //                    Methods.NvFlexFreeBuffer(Shapes[i].Indices);
    //                    break;
    //            }
    //        }
    //    }
    //}

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

#if UNITY_EDITOR
    [MenuItem("GameObject/Flex/Flex Container", false, 10)]
    static unsafe void CreateContainer(MenuCommand menuCommand)
    {
        GameObject go = new GameObject("Fluid Container");
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        var con = go.AddComponent<FlexContainer>();

        con.MaxParticles = 100000;
        con.MaxDiffuseParticles = 0;
        con.RParticleRadius = 0.5f;
        con.Substeps = 2;

        con.SolverParams.numIterations = 2;
        con.SolverParams.gravity[0] = 0;
        con.SolverParams.gravity[1] = -9.8f;
        con.SolverParams.gravity[2] = 0;
        con.SolverParams.radius = 0.5f;
        con.SolverParams.solidRestDistance = 0.25f;
        con.SolverParams.fluidRestDistance = 0.25f;
        con.SolverParams.maxSpeed = float.MaxValue;
        con.SolverParams.maxAcceleration = 100;
        con.SolverParams.surfaceTension = 0.04f;
        con.SolverParams.vorticityConfinement = 80;
        con.SolverParams.solidPressure = 1;
        con.SolverParams.buoyancy = 1;
        con.SolverParams.collisionDistance = 0.125f;
        con.SolverParams.particleCollisionMargin = 0.0625f;
        con.SolverParams.shapeCollisionMargin = 0.0625f;
        con.SolverParams.relaxationMode = NvFlexRelaxationMode.eNvFlexRelaxationLocal;
        con.SolverParams.relaxationFactor = 1;

        Selection.activeObject = go;
    }
#endif

}
