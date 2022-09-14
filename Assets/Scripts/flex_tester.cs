using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static NvFlex;
//using FlexSharp;

public class flex_tester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(NvFlexGetVersion());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        Debug.Log("stuff!");
    }
}
