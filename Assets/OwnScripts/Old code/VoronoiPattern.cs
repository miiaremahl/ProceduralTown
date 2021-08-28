using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * OLD CODE: attempted to write voronoi pattern but realised it's quite hard.. Satrted using library.
 * Generating Voronoi pattern.
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 20.2.2020
 * 
 * References:
 * 1.UpGames - Voronoi diagram tutorial in Unity3D(C#) https://www.youtube.com/watch?v=EDv69onIETk :
 * Used this to get an idea how to greate Voronoi pattern (tutorial in 2D -> transfrerred and edited to 3D)
 */

namespace OldCode
{
    public class VoronoiPattern : MonoBehaviour
    {
        public Vector3Int AreaDimensions; //dimensions of our area

        public Vector3Int StartCordinates; //where to start calculating voronoi pattern

        public int RegionAmount; //how many regions we have
        private int SpawnedRegions; //how many regions were we able to place

        Vector3Int[] CenterPoints; //center points of regions

        public int TimesToTryPlacing = 30; //how many times new point is tried to be placed

        Dictionary<Vector3Int, List<Vector3Int>> RegionData = new Dictionary<Vector3Int, List<Vector3Int>>(); //data for each region (which places it contains)


        void Start()
        {
            CreateRegions();
        }

        //Create the Voronoi pattern (regions)
        void CreateRegions()
        {
            SetCenterPoints();

            CalculateRegionData();
        }

        //Calculates cordinate data for each region
        void CalculateRegionData()
        {
            for (int x = 0; x < Mathf.Abs(StartCordinates.x) + AreaDimensions.x; x++)
            {
                for (int z = 0; z < Mathf.Abs(StartCordinates.z) + AreaDimensions.z; z++)
                {
                    Vector3Int cordinate = new Vector3Int(StartCordinates.x + x, 0, StartCordinates.z + z);
                    Vector3Int key = CenterPoints[GetClosesPoint(cordinate, CenterPoints)];
                    RegionData[key].Add(cordinate);
                }
            }
        }

        //returns the index of the closest point on the region list
        int GetClosesPoint(Vector3Int pos, Vector3Int[] pointPositions)
        {
            float smallestDistance = float.MaxValue;
            int smallestIndex = 0;

            for (int i = 0; i < pointPositions.Length; i++)
            {
                if (Vector3.Distance(pos, pointPositions[i]) < smallestDistance)
                {
                    smallestDistance = Vector3.Distance(pos, pointPositions[i]);
                    smallestIndex = i;
                }
            }
            return smallestIndex;
        }

        //Set the center point of each region
        void SetCenterPoints()
        {
            CenterPoints = new Vector3Int[RegionAmount];

            for (int i = 0; i < RegionAmount; i++)
            {
                bool placeNotFound = true;
                int timesTried = 0;
                while (placeNotFound)
                {
                    Vector3Int newPoinT = new Vector3Int(Random.Range(StartCordinates.x, AreaDimensions.x), 0, Random.Range(StartCordinates.z, AreaDimensions.z)); //random point in given dimensions
                    if (!RegionData.ContainsKey(newPoinT))
                    {
                        placeNotFound = false;
                        CenterPoints[i] = newPoinT;
                        RegionData.Add(CenterPoints[i], new List<Vector3Int>());
                        SpawnedRegions++;
                    }
                    else if (timesTried > TimesToTryPlacing)
                    {
                        placeNotFound = false;
                    }
                    else
                    {
                        timesTried++;
                    }
                }
            }
        }
    }

}
