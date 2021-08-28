using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using System;
using System.Linq;
/*
 * Placement helper  and side placer classes.
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 28.2.2020
 * 
 * References:
 * 1.PouletFrit - csDelaunay library : https://github.com/PouletFrit/csDelaunay, used this library to gert the Voronoi data and build on top of it.
 */

[Serializable]
public class AreaLimits
{
    public float zMaxlimit;
    public float zMinlimit;
    public float xMinLimit;
    public float xMaxLimit;
}

public class PlacementHelper : MonoBehaviour
{

    public ObjectSpawner ObjectSpawner; //ref to house spawner

    private List<SitePlacer> SitePlacers; //list of all the sites

    public Structure[] BuildingData; //all the building that could be spawned to house site

    [Header(header: "Area limits")]
    public AreaLimits AreaLimits;

    //characters
    public Character[] CharacterList;

    // structures for terrain
    public Structure[] TerrainData;

    // Rocket launching space
    public Structure RocketData;

    //Construction space
    public Structure Construction;

    //Sitetypes
    public SiteData[] SiteTypes;

    //layer mask for hitting terrain
    public LayerMask MarsMask;

    //distance for characters 
    public float CharacterDist;

    //over all population
    public int Population;

    //Place the houses to scene
    public void PlaceHouses(Dictionary<Vector2f, Site> sites, RoadHelper roadhelper)
    {
        SitePlacers = new List<SitePlacer>();

        foreach (KeyValuePair<Vector2f, Site> attachStat in sites)
        {
            //chooses a site type
            SiteType chosenType = GetSiteType(); 
            //SiteType chosenType = SiteType.Terrain;

            SitePlacer sitePlacer = new SitePlacer();

            //Place houses 
            sitePlacer.SetObjectData(BuildingData, CharacterList, ObjectSpawner, MarsMask, Population/sites.Count, CharacterDist); //set data
            sitePlacer.SetAreaLimits(AreaLimits); //set area limits

            if (chosenType == SiteType.Terrain)
            {
                sitePlacer.SetType(chosenType, TerrainData); //set type for the site
            }

            if (chosenType == SiteType.LaunchingSpace)
            {
                sitePlacer.SetType(chosenType,TerrainData); //set type for the site
                sitePlacer.SetRocket(RocketData);
            }

            if (chosenType == SiteType.Construction)
            {
                sitePlacer.SetType(chosenType, TerrainData); //set type for the site
                sitePlacer.SetConstruction(Construction);
            }

            List<Vector2f> centerPoints = new List<Vector2f>(sites.Keys); //center of all the sites
            sitePlacer.PlaceSite(attachStat.Value, centerPoints, roadhelper); //place housing to the site
            SitePlacers.Add(sitePlacer); //add to house site list
        }

        //NavMeshUpdater.BakeNavMesh(); //navmesh update for ai's
    }

    //chooses a sitetype for site
    SiteType GetSiteType()
    {
        bool siteNotFound = true;
        SiteType chosenSite = SiteType.Terrain;
        int i = 0;
        while (siteNotFound)
        {
            SiteData site = SiteTypes[i];
            if (site.hasSitesLeft() || site.MaxToPlace == -1)
            {
                siteNotFound = false;
                chosenSite = site.GetSiteType();
            }
            i++;
        }
        return chosenSite;
    }

    //Resets the building data
    public void ResetBuildingData()
    {
        for (int i = 0; i < BuildingData.Length; i++)
        {
            BuildingData[i].ResetAmount(); //reset each building amount
        }

        for (int i = 0; i < SiteTypes.Length; i++)
        {
            SiteTypes[i].ResetAmount(); //reset each building amount
        }


    }
}

