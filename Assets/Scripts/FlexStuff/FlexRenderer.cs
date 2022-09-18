using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlexSharp;

public class FlexRenderer : MonoBehaviour
{
    public FlexContainer Container;
    public ParticleSystem FluidRenderer;

    // Start is called before the first frame update
    void Start()
    {
        var FluidRendererMain = FluidRenderer.main;
        FluidRendererMain.maxParticles = Container.MaxParticles;

        Container.ParticleColours = new Color32[Container.MaxParticles];

        Container.InbetweenQueue += RenderParticles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RenderParticles()
    {
        var RParticles = new ParticleSystem.Particle[Container.SlotsUsed];

        unsafe
        {
            for (int i = 0; i < RParticles.Length; i++)
            {
                RParticles[i].position = Container.GBuffers.Positions.data[i];
                RParticles[i].startSize = Container.RParticleRadius;
                RParticles[i].startColor = Container.ParticleColours[i];

            }
        }
        FluidRenderer.SetParticles(RParticles);
    }
}
