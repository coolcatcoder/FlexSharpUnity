using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlexSharp;
using UnityEngine.Events;
using Unity.Jobs;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Burst;

[System.Serializable]
public class ParticleIdEvent : UnityEvent<int>
{

}

public class FlexCollider : MonoBehaviour
{
    public FlexContainer Container;

    public NvFlexCollisionShapeType Shape;
    public NvFlexPhase PhaseSettings;

    public float ShapeMysteryPower;

    public bool Dynamic;
    public bool Trigger;

    public Mesh TriMesh;

    public bool DetectCollision;

    public ParticleIdEvent MethodToRunOnDetectCollision;

    //public WhenToRun WhenToRunMethodOnCollision; //I hate this name, please someone figure out a better naming scheme for everything in this file just so that this variable can have a better name, please!

    //bool Burst = true; //force on, because burst is better, and trying to have support for both burst and non burst is getting tiresome, feel free to add your own support for non burst if you need to

    public int BatchSize = 256;

    public bool debug;

    [System.NonSerialized]
    public uint MeshId;

    [System.NonSerialized]
    unsafe public NvFlexBuffer* Vertices;

    [System.NonSerialized]
    unsafe public NvFlexBuffer* Indices;

    unsafe public Vector4* RWVertices;

    unsafe public int* RWIndices;

    int ShapeIndex;

    public enum WhenToRun
    {
        Update,
        FixedUpdate
    }

    bool RunMethod;

    [System.NonSerialized]
    public int ParticleId; // try to avoid using, this is heavilly deprecated

    [System.NonSerialized]
    public NativeList<int> ParticleIds;

    // Start is called before the first frame update
    void Start()
    {
        ShapeIndex = Container.SBuf.AddShape();

        if (DetectCollision)
        {
            Container.InbetweenQueue += BurstDealWithCollisions;
        }

        Container.InbetweenQueue += DealWithShape;

        if (Shape == NvFlexCollisionShapeType.eNvFlexShapeTriangleMesh)
        {
            Container.MappingQueue += InitTri;
            //Container.MappingQueue += MapTri;
            //Container.UnmappingQueue += UnMapTri;
            Container.DestroyQueue += DestroyTri;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (RunMethod)
        {
            //MethodToRunOnDetectCollision?.Invoke(); deprecated...
            RunMethod = false;
        }
    }

    void DealWithShape()
    {
        unsafe
        {
            if (transform.hasChanged)
            {
                //Debug.Log("Shapes have changed.");
                Container.SBuf.ShapesChanged = true;
                transform.hasChanged = false;

                var rotation = new FlexContainer.XQuat<float>();
                rotation.w = transform.rotation.w;
                rotation.x = transform.rotation.x;
                rotation.y = transform.rotation.y;
                rotation.z = transform.rotation.z;
                Container.SBuf.Rotations.data[ShapeIndex] = rotation;

                Container.SBuf.Positions.data[ShapeIndex] = new Vector4(transform.position.x, transform.position.y, transform.position.z, ShapeMysteryPower);

                Container.SBuf.Flags.data[ShapeIndex] = Methods.NvFlexMakeShapeFlagsWithChannels(Shape, Dynamic, (int)PhaseSettings);
                if (Trigger) { Container.SBuf.Flags.data[ShapeIndex] |= (int)NvFlexCollisionShapeFlags.eNvFlexShapeFlagTrigger; }

                switch (Shape)
                {
                    case NvFlexCollisionShapeType.eNvFlexShapeBox:
                        Container.SBuf.Geometry.data[ShapeIndex].box.halfExtents[0] = transform.lossyScale.x / 2;
                        Container.SBuf.Geometry.data[ShapeIndex].box.halfExtents[1] = transform.lossyScale.y / 2;
                        Container.SBuf.Geometry.data[ShapeIndex].box.halfExtents[2] = transform.lossyScale.z / 2;
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeCapsule:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeConvexMesh:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeSDF:
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeSphere:
                        Container.SBuf.Geometry.data[ShapeIndex].sphere.radius = transform.lossyScale.x / 2;
                        break;
                    case NvFlexCollisionShapeType.eNvFlexShapeTriangleMesh:

                        //Vertices = Methods.NvFlexAllocBuffer(Container.Library, TriMesh.vertexCount, sizeof(Vector4), NvFlexBufferType.eNvFlexBufferHost);
                        //Indices = Methods.NvFlexAllocBuffer(Container.Library, TriMesh.triangles.Length, sizeof(int), NvFlexBufferType.eNvFlexBufferHost);

                        //MeshId = Methods.NvFlexCreateTriangleMesh(Container.Library);

                        RWVertices = (Vector4*)Methods.NvFlexMap(Vertices, (int)NvFlexMapFlags.eNvFlexMapWait);
                        RWIndices = (int*)Methods.NvFlexMap(Indices, (int)NvFlexMapFlags.eNvFlexMapWait);

                        //for (int i = 0; i < TriMesh.vertices.Length; i++)
                        //{
                        //RWVertices[i] = TriMesh.vertices[i];
                        //}

                        VertsLooping VertJob = new VertsLooping();
                        VertJob.RWVerts = RWVertices;

                        NativeArray<Vector3> Verts;

                        using (var dataArray = Mesh.AcquireReadOnlyMeshData(TriMesh))
                        {
                            var data = dataArray[0];
                            Verts = new NativeArray<Vector3>(TriMesh.vertexCount, Allocator.TempJob);
                            data.GetVertices(Verts);
                            VertJob.Verts = Verts;
                        }

                        JobHandle VertHandle = VertJob.Schedule(TriMesh.vertices.Length, BatchSize);

                        //for (int i = 0; i < TriMesh.triangles.Length; i++)
                        //{
                        //RWIndices[i] = TriMesh.triangles[i];

                        //Debug.DrawLine(RWVertices[RWIndices[i]], RWVertices[RWIndices[i + 1]], Color.blue, float.PositiveInfinity);
                        //}

                        //for (int i = 0; i < TriMesh.triangles.Length; i += 3)
                        //{
                        //    Debug.DrawLine(RWVertices[RWIndices[i]], RWVertices[RWIndices[i + 1]], Color.blue, float.PositiveInfinity);
                        //    Debug.DrawLine(RWVertices[RWIndices[i + 1]], RWVertices[RWIndices[i + 2]], Color.blue, float.PositiveInfinity);
                        //    Debug.DrawLine(RWVertices[RWIndices[i + 2]], RWVertices[RWIndices[i]], Color.blue, float.PositiveInfinity);
                        //}

                        IndisLooping IndiJob = new IndisLooping();
                        IndiJob.RWIndis = RWIndices;

                        NativeArray<int> Indis;

                        using (var dataArray = Mesh.AcquireReadOnlyMeshData(TriMesh))
                        {
                            var data = dataArray[0];
                            Indis = new NativeArray<int>(TriMesh.triangles.Length, Allocator.TempJob);
                            data.GetIndices(Indis, 0);
                            IndiJob.Indis = Indis;
                        }

                        JobHandle IndiHandle = IndiJob.Schedule(TriMesh.triangles.Length, BatchSize);

                        var min = TriMesh.bounds.min;
                        var LowerBoundsPtr = &min;

                        var max = TriMesh.bounds.max;
                        var UpperBoundsPtr = &max;

                        Container.SBuf.Geometry.data[ShapeIndex].triMesh.mesh = MeshId;
                        Container.SBuf.Geometry.data[ShapeIndex].triMesh.scale[0] = transform.lossyScale.x;
                        Container.SBuf.Geometry.data[ShapeIndex].triMesh.scale[1] = transform.lossyScale.y;
                        Container.SBuf.Geometry.data[ShapeIndex].triMesh.scale[2] = transform.lossyScale.z;

                        

                        Methods.NvFlexUnmap(Vertices);
                        Methods.NvFlexUnmap(Indices);

                        VertHandle.Complete();
                        IndiHandle.Complete();

                        Verts.Dispose();
                        Indis.Dispose();

                        //Methods.NvFlexUpdateTriangleMesh(Container.Library, MeshId, Vertices, Indices, TriMesh.vertices.Length, TriMesh.triangles.Length / 3, null, null);
                        Methods.NvFlexUpdateTriangleMesh(Container.Library, MeshId, Vertices, Indices, TriMesh.vertices.Length, TriMesh.triangles.Length / 3, (float*)LowerBoundsPtr, (float*)UpperBoundsPtr);
                        
                        break;
                }
            }
        }
    }

    unsafe void InitTri()
    {
        Container.MappingQueue -= InitTri;
        Vertices = Methods.NvFlexAllocBuffer(Container.Library, TriMesh.vertexCount, sizeof(Vector4), NvFlexBufferType.eNvFlexBufferHost);
        Indices = Methods.NvFlexAllocBuffer(Container.Library, TriMesh.triangles.Length, sizeof(int), NvFlexBufferType.eNvFlexBufferHost);

        MeshId = Methods.NvFlexCreateTriangleMesh(Container.Library);

        Debug.Log("attempting to create a tri mesh");
    }

    //unsafe void MapTri()
    //{
    //    RWVertices = (Vector4*)Methods.NvFlexMap(Vertices, (int)NvFlexMapFlags.eNvFlexMapWait);
    //    RWIndices = (int*)Methods.NvFlexMap(Indices, (int)NvFlexMapFlags.eNvFlexMapWait);
    //}

    //unsafe void UnMapTri()
    //{
    //    Methods.NvFlexUnmap(Vertices);
    //    Methods.NvFlexUnmap(Indices);

    //    if (transform.hasChanged)
    //    {
    //        Methods.NvFlexUpdateTriangleMesh(Container.Library, MeshId, Vertices, Indices, TriMesh.vertices.Length, TriMesh.triangles.Length / 3, null, null);
    //        transform.hasChanged = false;
    //    }
    //}

    unsafe void DestroyTri()
    {
        Methods.NvFlexDestroyTriangleMesh(Container.Library, MeshId);
        Methods.NvFlexFreeBuffer(Vertices);
        Methods.NvFlexFreeBuffer(Indices);
    }

    [BurstCompile]
    public unsafe struct VertsLooping : IJobParallelFor
    {
        [WriteOnly]
        [NativeDisableUnsafePtrRestriction]
        public Vector4* RWVerts;

        [ReadOnly]
        public NativeArray<Vector3> Verts;

        public void Execute(int i)
        {
            RWVerts[i] = Verts[i];
        }
    }

    [BurstCompile]
    public unsafe struct IndisLooping : IJobParallelFor
    {
        [WriteOnly]
        [NativeDisableUnsafePtrRestriction]
        public int* RWIndis;

        [ReadOnly]
        public NativeArray<int> Indis;

        public void Execute(int i)
        {
            RWIndis[i] = Indis[i];
        }
    }

    unsafe void DealWithCollisions()
    {
        if (debug)
        {
            Debug.Log("run");
        }

        for (int i = 0; i < Container.SlotsUsed; i++)
        {
            int ContactIndex = Container.SBuf.ContactIndices.data[i];
            uint Count = Container.SBuf.ContactCounts.data[ContactIndex];

            if (debug)
            {
                Debug.Log(ContactIndex);
                Debug.Log(Count);
            }

            for (uint c = 0; c < Count; c++)
            {
                Vector4 Velocity = Container.SBuf.ContactVelocities.data[ContactIndex * 6 + c];

                int ContactShapeId = (int)Velocity.w;

                if (ContactShapeId == ShapeIndex)
                {
                    ParticleId = i;

                    if (debug)
                    {
                        Debug.Log("Collision!");
                    }

                    //if (WhenToRunMethodOnCollision == WhenToRun.FixedUpdate)
                    //{
                        //MethodToRunOnDetectCollision?.Invoke(); deprecate
                    //}
                    //else
                    //{
                        //RunMethod = true;
                    //}
                }
                else if (debug)
                {
                    Debug.Log(ContactShapeId);
                }
            }
        }
    }

    [BurstCompile]
    public unsafe struct CollisionsJob : IJobParallelFor
    {
        //public FlexContainer FContainer;
        //[ReadOnly]
        //public int _SlotsUsed;
        [ReadOnly]
        [NativeDisableUnsafePtrRestriction]
        public int* ContactIndicesData;
        [ReadOnly]
        [NativeDisableUnsafePtrRestriction]
        public uint* ContactCountsData;
        [ReadOnly]
        [NativeDisableUnsafePtrRestriction]
        public Vector4* ContactVelocitiesData;
        [ReadOnly]
        public int _ShapeIndex;
        public NativeList<int>.ParallelWriter _ParticleIds;
        //public WhenToRun _WhenToRunMethodOnCollision;
        //public UnityEvent _MethodToRunOnDetectCollision;
        //public NativeArray<bool> _RunMethod;

        public unsafe void Execute(int i)
        {
            //for (int i = 0; i < _SlotsUsed; i++)
            //{
                int ContactIndex = ContactIndicesData[i];
                uint Count = ContactCountsData[ContactIndex];

                for (uint c = 0; c < Count; c++)
                {
                    Vector4 Velocity = ContactVelocitiesData[ContactIndex * 6 + c];

                    int ContactShapeId = (int)Velocity.w;

                    if (ContactShapeId == _ShapeIndex)
                    {
                        //_ParticleIds.Add(i);
                        _ParticleIds.AddNoResize(i);

                        //Debug.Log("Collision");

                        //if (_WhenToRunMethodOnCollision == WhenToRun.FixedUpdate)
                        //{
                        //    _MethodToRunOnDetectCollision?.Invoke();
                        //}
                        //else
                        //{
                        //    _RunMethod[0] = true;
                        //}
                        // icky, dont like, so goodbye you garbage code, guess you will have to run fixed update, oh well
                    }
                }
            //}
        }
    }

    //[BurstCompile]
    public unsafe void BurstDealWithCollisions()
    {
        ParticleIds = new NativeList<int>(Container.SlotsUsed + 300, Allocator.TempJob); //+300 is not required, just for safety incase something goes wrong

        CollisionsJob CollisionsJobData = new CollisionsJob();

        //CollisionsJobData.FContainer = Container;
        CollisionsJobData.ContactIndicesData = Container.SBuf.ContactIndices.data;
        CollisionsJobData.ContactCountsData = Container.SBuf.ContactCounts.data;
        CollisionsJobData.ContactVelocitiesData = Container.SBuf.ContactVelocities.data;
        CollisionsJobData._ShapeIndex = ShapeIndex;
        CollisionsJobData._ParticleIds = ParticleIds.AsParallelWriter();
        //CollisionsJobData._SlotsUsed = Container.SlotsUsed;
        //CollisionsJobData._MethodToRunOnDetectCollision = MethodToRunOnDetectCollision;

        JobHandle handle = CollisionsJobData.Schedule(Container.SlotsUsed, BatchSize);
        handle.Complete();

        foreach (int PI in ParticleIds)
        {
            MethodToRunOnDetectCollision?.Invoke(PI);
        }

        if (debug)
        {
            Debug.Log(ParticleIds.Length);
        }

        ParticleIds.Dispose();

        
    }
}
