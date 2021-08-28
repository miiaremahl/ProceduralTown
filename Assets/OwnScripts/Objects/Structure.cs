using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/*
 * Class for structures.
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 8.3.2021
 * 
 * References:
 * 1. Sunny Valley Studio - Procedural town : https://www.youtube.com/watch?v=umedtEzrpvU&list=PLcRSafycjWFcbaI8Dzab9sTy5cAQzLHoy&index=1,
 *  took some ideas of how to make easily spawnable class and tried to modify it a bit.
 */

[Serializable]
public class Structure
{
    //prefab for the structure
    [SerializeField]
    private GameObject Prefab;

    //Horizontal size 
    public int HorizontalSize;

    //Vertical size 
    public int VerticalSize;

    //scaling size
    public int sizeMultiplied;

    //How many can be placed to scene
    public int Amount;

    //how many placed already
    public int NumOfPlaced;

    //Returns the prefab for the structure
    public GameObject GetPrefab()
    {
        NumOfPlaced++;
        return Prefab;
    }

    //are all structures placed
    public bool HasStructures()
    {
        return NumOfPlaced < Amount;
    }

    //reset the placed amount
    public void ResetAmount()
    {
        NumOfPlaced = 0;
    }

}
