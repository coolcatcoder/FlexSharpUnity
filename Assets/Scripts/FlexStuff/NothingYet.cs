using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NothingYet : MonoBehaviour
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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, Size);
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(NothingYet))]
public class FlexTriggerEditor : Editor
{
    public void OnSceneGUI()
    {
        var LinkedObject = target as NothingYet;

        Handles.color = Color.blue;
        LinkedObject.Size = Handles.ScaleHandle(LinkedObject.Size, LinkedObject.transform.position, LinkedObject.transform.rotation, HandleUtility.GetHandleSize(LinkedObject.transform.position)*1.5f);
    }
}
#endif