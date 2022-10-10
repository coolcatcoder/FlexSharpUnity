using FlexSharp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexParticlesFromMesh : MonoBehaviour
{
    // Start is called before the first frame update

    public Mesh Verts;
    public FlexContainer Container;
    public float InverseMass;
    public bool Fluid = false;
    public bool Rigid = false;
    public NvFlexPhase CollisionChannel;
    public Color32[] Colours;

    void Start()
    {
        Container.InbetweenQueue += AddParticles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    unsafe void AddParticles()
    {
        Container.InbetweenQueue -= AddParticles;

        int PrePhase = (int)NvFlexPhase.eNvFlexPhaseSelfCollide;
        if (Fluid)
        {
            PrePhase |= (int)NvFlexPhase.eNvFlexPhaseFluid;
        }

        var FinalPhase = Methods.NvFlexMakePhaseWithChannels(0, PrePhase, (int)CollisionChannel);

        for (int i = 0; i < Verts.vertexCount; i++)
        {
            var Pos = transform.TransformPoint(Verts.vertices[i]);

            Container.PBuf.Positions.data[Container.CurrentSlot] = new Vector4(Pos.x, Pos.y, Pos.z, InverseMass);
            //Container.PBuf.Velocities.data[Container.CurrentSlot] = new Vector3(VelocityX, VelocityY, VelocityZ);
            Container.PBuf.Phases.data[Container.CurrentSlot] = FinalPhase;
            Container.ParticleColours[Container.CurrentSlot] = Colours[Random.Range(0, Colours.Length)];//Verts.colors32[i];

            Container.SlotsUsed++;
            Mathf.Clamp(Container.SlotsUsed, 0, Container.MaxParticles);
            Container.CurrentSlot++;
            if (Container.CurrentSlot >= Container.MaxParticles - 1)
            {
                Container.CurrentSlot = 0;
            }
        }
    }
}
