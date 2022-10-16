using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextStuff : MonoBehaviour
{
    public TextMeshProUGUI FPSText;
    public float HudRefreshRate = 1f;

    public FlexContainer Container;
    public TextMeshProUGUI ParticleAmountText;

    private float FPSTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ParticleAmountText.text = "Particles: " + Container.SlotsUsed;

        if (Time.unscaledTime > FPSTimer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            FPSText.text = "FPS: " + fps;
            FPSTimer = Time.unscaledTime + HudRefreshRate;
        }
    }
}
