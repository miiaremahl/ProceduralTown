using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using System.Linq;

/*
 * Generating Voronoi pattern.
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 26.2.2020
 * 
 * References:
 * 1.PouletFrit - csDelaunay library : https://github.com/PouletFrit/csDelaunay, used his library to make the pattern (using csDelaunay)
 * 2.PouletFrit - Delaunay - Voronoi Diagram library for Unity : https://forum.unity.com/threads/delaunay-voronoi-diagram-library-for-unity.248962/,
 *  Took ideas from him how to use the library he had made and modified the ideas to fit this project.
 */

public class VoronoiPattern : MonoBehaviour
{
    [Header("Sites")]
    public int SiteAmount = 200; //how many sites we want 
    private int SpawnedSites; //how many regions were we able to place
    public int TimesToTryPlacing = 30; //how many times new point is tried to be placed

    [Header("Dimensions")]
    public Vector2Int AreaDimensions; //dimensions of our area
    public Vector2Int StartCordinates; //where to start calculating voronoi pattern

    [Header("Lloyd relaxation")]
    public bool UseLloyd=false; //do we use lloyd relaxation or not

    [Header("Helpers")]
    public RoadHelper Roadhelper;
    public PlacementHelper PlacementHelper;

    private Dictionary<Vector2f, Site> Sites; //sites of the pattern
    private List<Edge> Edges; //edges of the pattern

    [Header("Refrences")]
    public Interface Interface;//reference to UI

    void Start()
    {
        SetUIValues();
        CreateVoronoiPattern(); //create the pattern
    }


    //Send values to UI
    void SetUIValues()
    {
        Interface.SetInitialValues(SiteAmount);
    }

    //Creates new Voroinoi pattern /empire
    public void CreateNewPattern()
    {
        CreateVoronoiPattern(); //create the pattern
    }


    //Set values for the voronoi pattern
    public void SetValues(int siteAmount)
    {
        SiteAmount = siteAmount;
    }

    public void CreateVoronoiPattern()
    {
        List<Vector2f> points = SetCenterPoints(); //center points

        Rectf bounds = new Rectf(StartCordinates.x, StartCordinates.y, AreaDimensions.x, AreaDimensions.y); //bounds

        Voronoi voronoi = new Voronoi(points, bounds); //create voronoi

        if (UseLloyd) // do lloyd relaxation if wanted
        {
            voronoi.LloydRelaxation(5);
            Sites = voronoi.SitesIndexedByLocation;
        }

        //get edges for voronoi pattern
        Edges = voronoi.Edges;

        Roadhelper.PlaceRoads(Edges); //call road helper to place the roads
        PlacementHelper.PlaceHouses(Sites, Roadhelper); // call placement helper to place houses to scene
    }

    //Returns the center points
    private List<Vector2f> SetCenterPoints()
    {
        List<Vector2f> centerPoints = new List<Vector2f>(); //creating center points

        for (int i = 0; i < SiteAmount; i++)
        {
            bool placeNotFound = true;
            int timesTried = 0;
            while (placeNotFound) //try to find a place where there is no other point
            {
                Vector2f newPoinT = new Vector2f(Random.Range(StartCordinates.x, AreaDimensions.x), Random.Range(StartCordinates.y, AreaDimensions.y)); //random point in given dimensions
                if (!centerPoints.Contains(newPoinT))
                {
                    placeNotFound = false;
                    centerPoints.Add(newPoinT);
                    SpawnedSites++;
                }
                else if (timesTried > TimesToTryPlacing) //if we were unable to find a place stop after x amount of tries
                {
                    placeNotFound = false;
                }
                else
                {
                    timesTried++;
                }
            }
        }
        return centerPoints;
    }

}