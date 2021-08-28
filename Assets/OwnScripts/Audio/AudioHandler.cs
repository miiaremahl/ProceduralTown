using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class for music in scene.
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 8.3.2021
 */

public class AudioHandler : MonoBehaviour
{
    public AudioSource Ambient; //ambient music

    void Start()
    {
        Ambient.Play(); // play the music when scene starts
    }

}
