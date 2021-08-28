using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Camera movement
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 10.3.2021
 * 
 * * 1. Brackeys - FIRST PERSON MOVEMENT in Unity - FPS Controller : took ideas how to move the camera.
 */

public class TopViewBehaviour : MonoBehaviour
{
    //movement horzontal and vertical
    public float Horizontal;
    public float Vertical;

    //Speed for camera movement
    float Speed = 10f;

    //camera ref
    public CameraView CameraView;

    //rigidbody
    public Rigidbody Rb;

    //can we move
    bool MovementOn = false;

    void Update()
    {
        if(MovementOn)
        {
            //Get Input
            Horizontal = Input.GetAxis("Horizontal");
            Vertical = Input.GetAxis("Vertical");
        }
    }

    //enable camera movement
    public void EnableMovement()
    {
        MovementOn = true;
        Cursor.lockState = CursorLockMode.Locked;
        CameraView.EnableMovement();
    }

    //disable camera movement
    public void DisableMovement()
    {
        MovementOn = false;
        Cursor.lockState = CursorLockMode.None;
        CameraView.DisableMovement();
    }

    private void FixedUpdate()
    {
        if (MovementOn)
        {
            //movement
            Rb.MovePosition(transform.position + Time.deltaTime * Speed * transform.TransformDirection(Horizontal, y: 0f, Vertical));
            
            //rotation
            Rb.rotation = Quaternion.Euler(Rb.rotation.eulerAngles + new Vector3(0f, 2 * Input.GetAxis("Mouse X"), 0f));
        }
    }
}
