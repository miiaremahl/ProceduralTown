using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/*
 * Class for characters.
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 10.3.2021
 * 
 */
[Serializable]
public class Character 
{
    //prefab for the character
    [SerializeField]
    private GameObject Prefab;

    //returns the prefab
    public GameObject GetPreFab()
    {
        return Prefab;
    }
}
