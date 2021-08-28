using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
 * Handling UI interface in game scene.
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 6.3.2021
 */
public class Interface : MonoBehaviour
{
    //refrence to object spawner
    public ObjectSpawner ObjectSpawner;

    //Reference to voronoi pattern
    public VoronoiPattern VoronoiPattern;

    //reference to placement helper
    public PlacementHelper PlacementHelper;

    //camera movement ref and can we move freely w camera
    public TopViewBehaviour TopViewBehaviour;
    private bool FreeMovementOn = false;
    public GameObject ExploreText;
    public GameObject MovementText;
    public GameObject ExploreButton;
    public GameObject Panel;

    private int SiteAmount; //how many sites

    [Header("Sliders")]
    public Slider SiteSlider;  //Slider for the site amount
    public TextMeshProUGUI SiteVal; //Site amount as text

    void Update()
    {
        if (FreeMovementOn)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                DisableCameraMovement();
            }
        }
    }


    //enables free movement w camera
    public void EnableCameraMovement()
    {
        MovementText.SetActive(true);
        ExploreButton.SetActive(false);
        Panel.SetActive(false);
        FreeMovementOn = true;
        TopViewBehaviour.EnableMovement();
        ExploreText.SetActive(true);
        StartCoroutine(WaitAnimation());
    }

    //disable free movement w camera
    public void DisableCameraMovement()
    {
        MovementText.SetActive(false);
        Panel.SetActive(true);
        ExploreButton.SetActive(true);
        FreeMovementOn = false;
        TopViewBehaviour.DisableMovement();
    }

    //Update slider values (and text)
    public void UpdateSliders()
    {
        //Update site slider / text
        SiteVal.text = SiteAmount.ToString(); 
        SiteSlider.value = SiteAmount;
    }

    //Sets original values for the UI elements
    public void SetInitialValues(int siteAmount)
    {
        SiteAmount = siteAmount;

        UpdateSliders(); //update the values to UI
    }

    //Update different attribute values
    public void UpdateAttributes(string attributeName)
    {
        switch (attributeName)
        {
            case "siteAmount":
                SiteAmount = int.Parse(SiteSlider.value.ToString());
                SiteVal.text = SiteAmount.ToString();
            break;
        }
    }

    //Creates a voronoi pattern w random values
    public void CreateRandomEmpire()
    {
        ObjectSpawner.ClearScene(); //remove prefabs
        PlacementHelper.ResetBuildingData();//Reset building data

        //site amount
        int randomSite = Random.Range(0,30);
        SiteAmount = randomSite;
        SiteVal.text = randomSite.ToString();

        VoronoiPattern.SetValues(randomSite); // set values from UI to Voronoi pattern
        VoronoiPattern.CreateNewPattern(); //Create new Voronoi pattern
    }


    //Creates new voronoi pattern and deletes old prefabs
    public void CreateNewEmpire()
    {
        ObjectSpawner.ClearScene(); //remove prefabs
        PlacementHelper.ResetBuildingData();//Reset building data
        VoronoiPattern.SetValues(SiteAmount); // set values from UI to Voronoi pattern
        VoronoiPattern.CreateNewPattern(); //Create new Voronoi pattern
    }


    //waits explore animation to finnish
    private IEnumerator WaitAnimation()
    {
        yield return new WaitForSeconds(2.5f);
        ExploreText.SetActive(false);
    }
}
