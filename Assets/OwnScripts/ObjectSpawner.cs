using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Object spawning class.
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 6.3.2021
 */
public class ObjectSpawner : MonoBehaviour
{
    //All the spawned prefabs
    private List<GameObject> SpawnedObjects;


    void Start()
    {
        SpawnedObjects = new List<GameObject>(); //create new list
    }

    //Spawn object
    public void SpawnObject(Vector3Int pos, Quaternion rotation, GameObject prefab)
    {
        SpawnedObjects.Add(Instantiate(prefab, pos, rotation, transform));
    }

    //Spawn (Vector3 version)
    public void SpawnObject(Vector3 pos, Quaternion rotation, GameObject prefab)
    {
        SpawnedObjects.Add(Instantiate(prefab, pos, rotation, transform));
    }

    //spawn a structure (with scale multiplier)
    public void SpawnStructure(Vector3 pos, Quaternion rotation, GameObject prefab, int multiplier){
        GameObject spawned = Instantiate(prefab, pos, rotation, transform);
        SpawnedObjects.Add(spawned);
        spawned.transform.localScale = new Vector3(spawned.transform.localScale.x* multiplier, spawned.transform.localScale.y* multiplier, spawned.transform.localScale.z* multiplier);
    }

    //Removes all the spawned prefabs
    public void ClearScene()
    {
        foreach (GameObject Prefab in SpawnedObjects)
        {
            Destroy(Prefab);
        }
        SpawnedObjects.Clear();
    }
}
