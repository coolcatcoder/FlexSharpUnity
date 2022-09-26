using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FluidRendererExperimental : MonoBehaviour
{
    // Start is called before the first frame update

    public ComputeShader SphereDrawCS;

    RenderTexture TestTexture;

    public RawImage ScreenDraw;

    public Transform TestTransform;

    //ComputeBuffer PositionsBuffer;

    void Start()
    {
        TestTexture = new RenderTexture(Screen.width, Screen.height, 24);
        TestTexture.enableRandomWrite = true;
        TestTexture.Create();

        ScreenDraw.texture = TestTexture;
        ScreenDraw.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width,Screen.height);

        SphereDrawCS.SetTexture(0, "Result", TestTexture);
        //SphereDrawCS.SetFloat("Resolution", TestTexture.width);
        SphereDrawCS.Dispatch(0, TestTexture.width / 8, TestTexture.height / 8, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
