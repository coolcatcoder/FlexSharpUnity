using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlexSharp;

public class FlexCollider : MonoBehaviour
{
    public FlexContainer Container;

    public NvFlexCollisionShapeType Shape;
    public NvFlexPhase PhaseSettings;

    public float ShapeMysteryPower;

    public bool Dynamic;
    public bool Trigger;

    public Mesh TriMesh;

    [System.NonSerialized]
    public uint MeshId;

    [System.NonSerialized]
    unsafe public NvFlexBuffer* Vertices;

    [System.NonSerialized]
    unsafe public NvFlexBuffer* Indices;

    int ShapeIndex;

    // Start is called before the first frame update
    void Start()
    {
        ShapeIndex = Container.SBuf.AddShape();

        Container.InbetweenQueue += DealWithShape;
    }

    // Update is called once per frame
    void Update()
    {
        
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
                        //var RWIndices = (int*)Methods.NvFlexMap(Shapes[i].Indices, (int)NvFlexMapFlags.eNvFlexMapWait);
                        //var RWVertices = (Vector3*)Methods.NvFlexMap(Shapes[i].Vertices, (int)NvFlexMapFlags.eNvFlexMapWait);


                        //for (int k = 0; k < Shapes[i].TriMesh.vertices.Length; k++)
                        //{
                        //    RWVertices[k] = Shapes[i].TriMesh.vertices[k];
                        //}

                        //for (int k = 0; k < Shapes[i].TriMesh.triangles.Length; k++)
                        //{
                        //    RWIndices[k] = Shapes[i].TriMesh.triangles[k];

                        //    Debug.DrawLine(RWVertices[RWIndices[k]], RWVertices[RWIndices[k + 1]], Color.blue, float.PositiveInfinity);
                        //}

                        //for (int k = 0; k < Shapes[i].TriMesh.triangles.Length; k += 3)
                        //{
                        //    Debug.DrawLine(RWVertices[RWIndices[k]], RWVertices[RWIndices[k + 1]], Color.blue, float.PositiveInfinity);
                        //    Debug.DrawLine(RWVertices[RWIndices[k + 1]], RWVertices[RWIndices[k + 2]], Color.blue, float.PositiveInfinity);
                        //    Debug.DrawLine(RWVertices[RWIndices[k + 2]], RWVertices[RWIndices[k]], Color.blue, float.PositiveInfinity);
                        //}

                        //Methods.NvFlexUnmap(Shapes[i].Indices);
                        //Methods.NvFlexUnmap(Shapes[i].Vertices);

                        //var min = Shapes[i].TriMesh.bounds.min * 2;
                        //var LowerBoundsPtr = &min;

                        //var max = Shapes[i].TriMesh.bounds.max * 2;
                        //var UpperBoundsPtr = &max;

                        //Methods.NvFlexUpdateTriangleMesh(Library, Shapes[i].MeshId, Shapes[i].Vertices, Shapes[i].Indices, Shapes[i].TriMesh.vertices.Length, Shapes[i].TriMesh.triangles.Length / 3, (float*)LowerBoundsPtr, (float*)UpperBoundsPtr);
                        ////Methods.NvFlexUpdateTriangleMesh(library, Shapes[i].MeshId, Shapes[i].Vertices, Shapes[i].Indices, Shapes[i].TriMesh.vertices.Length, Shapes[i].TriMesh.triangles.Length / 3, null, null);

                        //Container.SBuf.Geometry.data[i].triMesh.mesh = Shapes[i].MeshId;
                        //Container.SBuf.Geometry.data[i].triMesh.scale[0] = Shapes[i].transform.lossyScale.x;
                        //Container.SBuf.Geometry.data[i].triMesh.scale[1] = Shapes[i].transform.lossyScale.y;
                        //Container.SBuf.Geometry.data[i].triMesh.scale[2] = Shapes[i].transform.lossyScale.z;

                        break;
                }
            }
        }
    }
}
