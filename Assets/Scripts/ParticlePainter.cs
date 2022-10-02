using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePainter : MonoBehaviour
{
    public FlexCollider CollisionMachine;
    public Color32[] NewColours;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PaintParticle(int ParticleId)
    {
        CollisionMachine.Container.ParticleColours[ParticleId] = NewColours[Random.Range(0, NewColours.Length)];
    }
}
