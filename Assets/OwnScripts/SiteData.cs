using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/*
 * Class for site info.
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 10.3.2021
 */

[Serializable]
public class SiteData
{
    //How many times site can be placed
    public int MaxToPlace;

    //how many placed already
    public int NumOfPlaced;

    //Sitetype
    public SiteType siteType;

    //get type of site
    public SiteType GetSiteType()
    {
        NumOfPlaced++;
        return siteType;
    }

    //can we place more of this site
    public bool hasSitesLeft()
    {
        return NumOfPlaced < MaxToPlace;
    }

    //reset amount
    public void ResetAmount()
    {
        NumOfPlaced = 0;
    }

}
