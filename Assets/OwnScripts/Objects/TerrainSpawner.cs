using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSpawner
{
    //min and max what to spawn
    public int CountMin = 10;
    public int CountMax = 40;

    //list of structures
    private Structure[] StructureList;


    //returns the spawning distance
    public float GetDistance()
    {
        return Random.Range(10, 50);
    }

    //gets random amount of items to spawn
    public int GetSpawnAmount()
    {
        return Random.Range(CountMin, CountMax);
    }

    //set the structure list
    public void SetStructureList(Structure[] ObjectList)
    {
        StructureList = ObjectList;
    }

    //returns random prefab from list
    public Structure GetStructure()
    {
        int randomNum= Random.Range(0, StructureList.Length);
        return StructureList[randomNum];
    }

    //get random rotation for the structure
    public Quaternion GetRotation()
    {
        return Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }



}
