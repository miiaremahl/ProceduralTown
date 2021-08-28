using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Camera view
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 10.3.2021
 * Refs:
 * 1. Brackeys - FIRST PERSON MOVEMENT in Unity - FPS Controller : took ideas how to change the camera view.
 * 
 */
public class CameraView : MonoBehaviour
{
    //mouse sensitivity
    public float sensitivity = 100f;

    //rotation
    float xRotation = 0f;

    //Can we move
    bool movementOn = false;

    void Update()
    {
        if (movementOn)
        {
            //Mouse input
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            //rotate
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90, 90);

            //change the view vertical view
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
     
    }

    //enable camera movement
    public void EnableMovement()
    {
        movementOn = true;
    }

    //disable camera movement
    public void DisableMovement()
    {
        movementOn = false;
    }
}
