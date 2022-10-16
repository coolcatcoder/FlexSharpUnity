using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Sensitivity;
    public float Speed;
    public float ScrollSensitivity;

    float ViewerX;
    float ViewerY;
    float MoveX;
    float MoveZ;

    Rigidbody rb;

    public bool FreezeZ = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Speed += Input.mouseScrollDelta.y * ScrollSensitivity;

        ViewerX = Input.GetAxis("Mouse X") * Sensitivity;
        ViewerY = Input.GetAxis("Mouse Y") * Sensitivity;
        MoveX = Input.GetAxis("Horizontal") * Speed;
        MoveZ = Input.GetAxis("Vertical") * Speed;

        transform.Rotate(Vector3.up * ViewerX);
        transform.Rotate(Vector3.left * ViewerY);

        if (FreezeZ)
        {
            var ZRot = transform.eulerAngles;
            ZRot.z = 0;
            transform.eulerAngles = ZRot;
        }

        rb.AddRelativeForce(new Vector3(MoveX, 0, MoveZ));

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
