using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FlexTrigger : MonoBehaviour
{
    public Vector3 Size;
    public FlexContainer Container;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, Size);
    }
}

[CustomEditor(typeof(FlexTrigger))]
public class FlexTriggerEditor : Editor
{
    public void OnSceneGUI()
    {
        var LinkedObject = target as FlexTrigger;

        Handles.color = Color.blue;
        LinkedObject.Size = Handles.ScaleHandle(LinkedObject.Size, LinkedObject.transform.position, LinkedObject.transform.rotation, HandleUtility.GetHandleSize(LinkedObject.transform.position)*1.5f);
    }
}