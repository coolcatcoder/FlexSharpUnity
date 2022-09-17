using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlexSharp;

public class FlexCollider : MonoBehaviour
{
    //public FlexContainer Container;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
