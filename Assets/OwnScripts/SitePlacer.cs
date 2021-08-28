using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using System.Linq;

/*
 * Placement helper for one site. For house sites.
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 6.3.2020
 * 
 * References:
 * 1.PouletFrit - Delaunay - Voronoi Diagram library for Unity : https://forum.unity.com/threads/delaunay-voronoi-diagram-library-for-unity.248962/,
 *  Took ideas from him how to use the library he had made and modified the ideas to fit this project.
 * 2. Sunny Valley Studio - Procedural town : https://www.youtube.com/watch?v=umedtEzrpvU&list=PLcRSafycjWFcbaI8Dzab9sTy5cAQzLHoy&index=1,
 * took some ideas for the sctructure and how to place houses next to road
 * 3. Alan Zucconi -Procedural Content Generation (Part 1: Randomness) : https://learn.gold.ac.uk/course/view.php?id=16027#section-6
 */

public class SitePlacer
{
    //ref to house spawner
    private ObjectSpawner ObjectSpawner;

    //all the building that could be spawned to house site
    private Structure[] BuildingData; 

    //coordinates for edges
    private List<Vector3> EdgeCoordinates; 

    //Blocked positions 
    private List<Vector3> BlockedPositions;

    //Area limits
    private AreaLimits AreaLimits;

    //Objects that can be spawned
    private Structure[] ObjectList;

    //characters
    private Character[] CharacterList;

    //Sitetype
    SiteType TypeOfSite;

    //Data about site
    Site SiteData;

    //character spawn distance
    float CharacterDist=50;

    //Mars mask
    LayerMask MarsMask;

    //Population in this site
    int Population;

    //rocket area
    Structure RocketSpace;

    //Construction site
    Structure Construction;

    //Dictionary for the free spots
    private Dictionary<Vector3, Direction> FreeHouseSpots;

    //For setting buildingData
    public void SetObjectData(Structure[] buildingData, Character[] characterList, ObjectSpawner objectSpawner, LayerMask marsMask, int population, float characterDist)
    {
        BuildingData = buildingData;
        CharacterList = characterList;
        ObjectSpawner = objectSpawner;
        MarsMask = marsMask;
        Population = population;
        CharacterDist = characterDist;
    }

    //Set area limits 
    public void SetAreaLimits(AreaLimits areaLimits)
    {
        AreaLimits = areaLimits;
    }

    //Set type of the site
    public void SetType(SiteType typeOfSite, Structure[] objectList)
    {
        TypeOfSite = typeOfSite;
        ObjectList = objectList;
    }

    //Set rocket launhing space
    public void SetRocket(Structure rocket)
    {
        RocketSpace = rocket;
    }

    //set construction site
    public void SetConstruction(Structure construction)
    {
        Construction = construction;
    }

    //Place elements to site
    public void PlaceSite(Site siteData, List<Vector2f> sites, RoadHelper roadHelper)
    {
        SiteData = siteData;
        EdgeCoordinates = new List<Vector3>(); //new list for edge coordinates

        foreach (Edge edge in siteData.Edges) //go through edges
        {
            if (edge.ClippedEnds == null) //not inside bounds -> skip
            {
                continue;
            }

            CountEdgeCoords(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT]);
        }

        //set blocked list to road positions
        BlockedPositions = roadHelper.GetRoadPositions();
        FreeHouseSpots = FindFreeSpots(EdgeCoordinates);


        //Go through the free spots
        foreach (var key in FreeHouseSpots.Keys)
        {
            //spot is blocked by other building
            if (BlockedPositions.Contains(key))
            {
                continue;
            }

            //belongs to other cell
            if (!IsInRegion(new Vector2(siteData.x, siteData.y), sites, new Vector2(key.x,key.z))) 
            {
                continue;
            }

            //Check that is within the area
            if (!IsInArea(key)) //is not -> Skip
            {
                continue;
            }

            //get direction and rotation
            Direction direction = FreeHouseSpots[key];
            Quaternion rotation = GetRotation(direction);

            //Go through every building data to see what you could spawn
            for (int i = 0; i < BuildingData.Length; i++)
            {
                //Check if buildingdata has buildings left, if not -> go to next
                if (BuildingData[i].HasStructures())
                {
                    //Check the size
                    if (BuildingData[i].HorizontalSize > 1 || BuildingData[i].VerticalSize > 1)
                    {
                        //temporary list of possibly blocked spots
                        List<Vector3> tempBlocked = new List<Vector3>();

                        //Will the building fit the free spot
                        if (VerifyObjectFits(BuildingData[i].HorizontalSize, BuildingData[i].VerticalSize, key, direction, ref tempBlocked))
                        {
                            //building fits -> spawn and add to the blocked list
                            ObjectSpawner.SpawnObject(key, rotation, BuildingData[i].GetPrefab());
                            BlockedPositions.AddRange(tempBlocked);
                            break;
                        }

                    }
                    else
                    {
                        ObjectSpawner.SpawnObject(key, rotation, BuildingData[i].GetPrefab());
                        break;
                    }
                }
            }
        }
        PlaceOtherSiteStructures();
        SpawnPeople();
    }

    //Places people in the scene
    void SpawnPeople()
    {
        int amount = Population;
        for (int i = 0; i < amount; i++)
        {
            Character prefab = CharacterList[Random.Range(0, CharacterList.Length)];
            Vector3 position = FindFreePosition();
            Quaternion rotation = Quaternion.Euler(0f,Random.Range(0f, 360f), 0f);

            ObjectSpawner.SpawnObject(position, rotation, prefab.GetPreFab());
        }
    }


    //Find free position for characters, refs: 3. Alan Zucconi : idea of how to place the characters to random location
    public Vector3 FindFreePosition()
    {
        int i = 0;
        Vector2 circle = Random.insideUnitCircle;
        const int MaxTries = 50;
        Vector3 position;
        do
        {
            position = new Vector3(SiteData.x, 0, SiteData.y)+
                new Vector3(Random.Range(-CharacterDist, +CharacterDist),
                0, 
                Random.Range(-CharacterDist, +CharacterDist));
            i++;

            if (i >= MaxTries)
            {
                break;
            }
        } while (IsObjectAt(position));

        return position;
    }

    //check is place is taken
    bool IsObjectAt(Vector3 position)
    {
        List<Vector3> tempBlocked = new List<Vector3>();
        if (VerifyObjectFits(2, 2, position, Direction.Right, ref tempBlocked))
        {
            BlockedPositions.AddRange(tempBlocked);
            return false;
        }
        return true;
    }


    //Place other structures on site
    void PlaceOtherSiteStructures()
    {
        if (TypeOfSite == SiteType.Terrain) //terrain
        {
            PlaceTerrainStructures();
        }

        if (TypeOfSite == SiteType.LaunchingSpace) //rocket area
        {
            PlaceLaunchingSpace();
            PlaceTerrainStructures();
        }

        if (TypeOfSite == SiteType.Construction) //rocket area
        {
            PlaceConstruction();
            PlaceTerrainStructures();
        }
    }


    //Placement of the construction
    void PlaceConstruction()
    {
        //Let's try to place the construction area
        Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        bool positionFound = false;

        //spawning distance from center
        float distance = 10f;

        //position
        Vector3 position = GetObjectPosition(distance, Construction, 1, ref positionFound);

        if (positionFound)
        {
            ObjectSpawner.SpawnObject(position, rotation, Construction.GetPrefab());
        }
    }


    //Placement of the launching area
    void PlaceLaunchingSpace()
    {
        //Let's try to place the main rocket area
        Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        bool positionFound = false;

        //spawning distance from center
        float distance = 10f;

        //position
        Vector3 position = GetObjectPosition(distance, RocketSpace, 1, ref positionFound);

        if (positionFound)
        {
            ObjectSpawner.SpawnObject(position, rotation, RocketSpace.GetPrefab());
        }
    }

    //terrain placement logic
    private void PlaceTerrainStructures()
    {
        //make a new terrainspawner class to help 
        TerrainSpawner terrainSpawner = new TerrainSpawner();

        //set the prefab list
        terrainSpawner.SetStructureList(ObjectList);

        //how many to spawn
        int amount = terrainSpawner.GetSpawnAmount();

        float distance = terrainSpawner.GetDistance();

        //spawn certain amount of terrain objects
        for (int i = 0; i < amount; i++)
        {
            Structure structure = terrainSpawner.GetStructure();
            Quaternion rotation = terrainSpawner.GetRotation();
            bool positionFound = false;

            //random changes to size
            //ChangeStructureSize(ref structure);
            int multiplier = Random.Range(1, 4);

            //position
            Vector3 position = GetObjectPosition(distance, structure, multiplier, ref positionFound);

            if (positionFound)
            {
                ObjectSpawner.SpawnStructure(position, rotation, structure.GetPrefab(), multiplier);
            }
        }
    }

    //return position for a structure
    //refs: 3. Alan Zucconi, used the "tree spawning logic" and modified it for this use, also used RandomGaussian
    public Vector3 GetObjectPosition(float distance, Structure structure,int multiplier, ref bool positionFound)
    {
        Vector3 position = new Vector3(SiteData.x, 0, SiteData.y);
        int i = 0;
        const int maxTries = 50;
        bool posFound = false;

        while (!posFound)
        {
            //get random position using random gaussian
            position = new Vector3((int)SiteData.x, 0, (int)SiteData.y) + new Vector3((int)
               RandomGaussian.Range(-distance, +distance),
               0,
               (int)RandomGaussian.Range(-distance, +distance)
            );

            //does the piece fit
            if (ObjectFits(position, structure, multiplier))
            {
                posFound = true;
                positionFound = true;
            }
            if (i >= maxTries)
            {
                break;
            }

            i++;
        }

        return position;
    }

    //does the nature piece fit
    bool ObjectFits(Vector3 position, Structure structure,int multiplier)
    {
        List<Vector3> tempBlocked = new List<Vector3>();
        if(VerifyObjectFits(structure.HorizontalSize * multiplier, structure.VerticalSize* multiplier, position, Direction.Right, ref tempBlocked)){
            BlockedPositions.AddRange(tempBlocked);
            return true;
        }
        return false;
    }


    //Check that point is in area
    private bool IsInArea(Vector3 key)
    {
        //goes outside the area limits
        if (AreaLimits.zMaxlimit <= key.z || AreaLimits.xMaxLimit <= key.x
            || AreaLimits.xMinLimit >= key.x || AreaLimits.zMinlimit >= key.z)
        {
            return false;
        }
        return true;
    }


    //Does the object fit the place
    private bool VerifyObjectFits(
           int horizontal,
           int vertical,
           Vector3 key,
           Direction dir,
           ref List<Vector3> tempBlocked)
    {
        //Checking HORIZONTAL fitting

        //Count the center of the item (horizontal)
        int horizontalHalf = Mathf.FloorToInt(horizontal / 2.0f);

        Vector3 direction = Vector3.zero;
        Vector3 verticalDirection = Vector3.zero; //vertical direction

        //determine the direction that building would be spawned
        if (dir == Direction.Down || dir == Direction.Up)
        {
            direction = Vector3.right;
        }
        else
        {
            direction = new Vector3(0, 0, 1);
        }

        for (int i = 1; i <= horizontalHalf; i++)
        {
            //pos right and left
            var pos1 = key + direction * i;
            var pos2 = key - direction * i;

            //part goes to blocked posions -> does not fit
            if (BlockedPositions.Contains(pos1) || BlockedPositions.Contains(pos2))
            {
                return false;
            }
            //hits the road (edges) -> does not fit
            if (EdgeCoordinates.Contains(pos1) || EdgeCoordinates.Contains(pos2))
            {
                return false;
            }
            //goes outside area -> does not fit
            if (!IsInArea(pos1) || !IsInArea(pos2))
            {
                return false;
            }

            //add positions to the temporarely blocked
            tempBlocked.Add(pos1);
            tempBlocked.Add(pos2);


            //Checking VERTICAL fitting
            switch (dir)
            {
                case Direction.Up:
                    verticalDirection = new Vector3(0, 0, -1);
                    break;
                case Direction.Down:
                    verticalDirection = new Vector3(0, 0, 1);
                    break;
                case Direction.Left:
                    verticalDirection = Vector3.right;
                    break;
                default:
                    verticalDirection = Vector3.left;
                    break;
            }

            //Check vertical direction fitting
            for (int j = 1; j <= vertical; j++)
            {
                var pos3 = pos1 + verticalDirection * i;
                var pos4 = pos2 + verticalDirection * i;


                //part goes to blocked posions -> does not fit
                if (BlockedPositions.Contains(pos3) || BlockedPositions.Contains(pos4))
                {
                    return false;
                }

                //hits the road (edges) -> does not fit
                if (EdgeCoordinates.Contains(pos3) || EdgeCoordinates.Contains(pos4))
                {
                    return false;
                }

                //goes outside area -> does not fit
                if (!IsInArea(pos3) || !IsInArea(pos4))
                {
                    return false;
                }

                //add positions to the temporarely blocked
                tempBlocked.Add(pos3);
                tempBlocked.Add(pos4);
            }
        }
        return true;
    }


    //get the right rotation for the house
    private Quaternion GetRotation(Direction freeSpot)
    {
        switch (freeSpot)
        {
            case Direction.Right:
                return Quaternion.Euler(0, -90, 0);
            case Direction.Left:
                return Quaternion.Euler(0, 90, 0);
            case Direction.Up:
                return Quaternion.Euler(0, 180, 0);
            default:
                return Quaternion.identity;
        }
    }

    //Finding free spots where the houses could be spawned, ref:2, Sunny Valley Studio, took inspiration for the code
    private Dictionary<Vector3, Direction> FindFreeSpots(List<Vector3> edges)
    {
        //make the new dictionary for free spaces
        Dictionary<Vector3, Direction> freeSpaces = new Dictionary<Vector3, Direction>();

        //go through the edge positions
        foreach (var pos in edges)
        {
            //get taken positions for the position
            List<Direction> taken = PlacementLogic.FindTaken(pos, edges);

            //loops through the Direction enum
            foreach (Direction direction in System.Enum.GetValues(typeof(Direction)))
            {
                //direction not taken -> free space
                if (!taken.Contains(direction))
                {
                    //*3 because we need to consider road width
                    Vector3 newPosition = pos + (PlacementLogic.GetOffsetFromDirection(direction)*3);

                    //is there all ready a house here
                    if (freeSpaces.ContainsKey(newPosition))
                    {
                        continue;
                    }

                    //add to the freespaces
                    freeSpaces.Add(newPosition, PlacementLogic.GetReversedDirection(direction));
                }
            }
        }
        return freeSpaces;
    }

    //stores the edge cordinates (where the roads are)
    //Ref: 1. I took the implementation for the math from PouletFrit
    void CountEdgeCoords(Vector2f p1, Vector2f p2)
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
            EdgeCoordinates.Add(new Vector3(x0, 0, z0)); //add condinates to list

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

    //Checks if the point is closest to the given areas center (compared to other centers)
    bool IsInRegion(Vector2 site, List<Vector2f> sites, Vector2 coord)
    {
        float smallestDistance = float.MaxValue;
        Vector2 closest=new Vector2(0,0);

        for (int i = 0; i < sites.Count; i++) //checks every site
        {
            Vector2 temp= new Vector2(sites[i].x, sites[i].y);
            if (Vector2.Distance(coord, temp) < smallestDistance)
            {
                smallestDistance = Vector2.Distance(coord, temp);
                closest = temp;
            }
        }
        return closest == site; 
    }
}
