using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Audio class for menu.
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 8.3.2020
 */

public class MenuAudio : MonoBehaviour
{

    public AudioSource StartEffect; //start effect sound

    //Played when user click explore
    public void PlayStartingMusic()
    {
        StartEffect.Play();
    }


}
