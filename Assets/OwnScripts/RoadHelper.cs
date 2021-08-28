using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

/*
 * Road helper class.
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 8.3.2021
 * 
 * References:
 * 1.PouletFrit - csDelaunay library : https://github.com/PouletFrit/csDelaunay, used his library to make the pattern (using csDelaunay)
 * 2.PouletFrit - Delaunay - Voronoi Diagram library for Unity : https://forum.unity.com/threads/delaunay-voronoi-diagram-library-for-unity.248962/,
 *  Took ideas from him how to use the library he had made and modified the ideas to fit this project.
 * 3. Sunny Valley Studio - Procedural town : https://www.youtube.com/watch?v=umedtEzrpvU&list=PLcRSafycjWFcbaI8Dzab9sTy5cAQzLHoy&index=1, ideas for the finding free spots
 */


public class RoadHelper : MonoBehaviour
{
    public GameObject prefab; //Road prefab

    public ObjectSpawner ObjectSpawner; //ref to object spawner

    private List<Vector3> RoadCordinates; //stores for all road spots available

    public GameObject mainRoad;


    //Returns road positions
    public List<Vector3> GetRoadPositions()
    {
        return RoadCordinates;
    }

    //Places the roads according to the pattern edges
    //refs:1. took an idea from his code how the clippedend work
    public void PlaceRoads(List<Edge> edges)
    {
        RoadCordinates = new List<Vector3>();

        foreach (Edge edge in edges)
        {
            if (edge.ClippedEnds == null) //not inside bounds -> skip
            {
                continue; 
            }

            SetMidRoadCoordinates(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT]);
        }

        //Add free spots near road
        RoadCordinates.AddRange(FindFreeSpots(RoadCordinates)); 

        //place sandy road
        PlaceRoadPieces();

    }
    
    //Places road pieces to every road spot
    private void PlaceRoadPieces()
    {
        foreach (Vector3 cordinate in RoadCordinates) //for each road spot add road prefab
        {
            SpawnRoad(cordinate);
        }
    }

    //Finding free spots where the road can be refs: 3. Sunny Valley Studio - took ideas on how to find free spots
    private List<Vector3> FindFreeSpots(List<Vector3> roads)
    {
        //make the new dictionary for free spaces
        List<Vector3> freeSpaces = new List<Vector3>();

        //go through the edge positions
        foreach (var pos in roads)
        {
            //get taken positions for the position
            List<Direction> taken = PlacementLogic.FindTaken(pos, roads);

            //loops through the Direction enum
            foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
            {
                //direction not taken -> free space
                if (!taken.Contains(direction))
                {
                    Vector3 newPosition = pos + PlacementLogic.GetOffsetFromDirection(direction);

                    //is there all ready a road here
                    if (freeSpaces.Contains(newPosition))
                    {
                        continue;
                    }

                    //add to the freespaces
                    freeSpaces.Add(newPosition);
                }
            }
        }
        return freeSpaces;
    }


    //spawn one piece of road
    private void SpawnRoad(Vector3 pos){
        var rotation = Quaternion.identity;
        ObjectSpawner.SpawnObject(pos, rotation, prefab);
    }


    // Places one road piece: uses Bresenham line algorithm
    //Ref: 2. I took the implementation for the math from PouletFrit
    private void SetMidRoadCoordinates(Vector2f p1, Vector2f p2)
    {
        //point 1 vals
        int x0 = (int)p1.x;
        int x1 = (int)p2.x;

        //point 2 vals
        int z0 = (int)p1.y;
        int z1 = (int)p2.y;

        int difX = Mathf.Abs(x1 - x0); //difference between points
        int difY = Mathf.Abs(z1 - z0);
        int difBetw = difX - difY;

        int factorX = x0 < x1 ? 1 : -1; 
        int factorY = z0 < z1 ? 1 : -1;

        while (true)
        {
            RoadCordinates.Add(new Vector3(x0, 0, z0));

            if (x0 == x1 && z0 == z1) break; //if the point 1 reaches point 2 -> stop loop

            //readjust values
            int difD = 2 * difBetw;
            if (difD > -difY)
            {
                difBetw -= difY;
                x0 += factorX;
            }
            if (difD < difX)
            {
                difBetw += difX;
                z0 += factorY;
            }
        }
    }
}
