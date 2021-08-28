using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
 * Scene changing logic.
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * last edited: 28.2.2021
 * 
 * References:
 */


public class SceneChanger : MonoBehaviour
{
    public Animator TitleAnimator; //title animator refrence

    public MenuAudio MenuAudio; //menu audio refrence

    //Loads the main game
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    //Called when user pressed explore button
    public void ExploreClick()
    {
        StartCoroutine(SceneChangeTimer());
        MenuAudio.PlayStartingMusic();
        TitleAnimator.SetBool("Started", true);
    }

    //loads the main menu
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    //Coroutine for scene change
    IEnumerator SceneChangeTimer()
    {
        yield return new WaitForSeconds(1.8f); //waits for the animation to stop
        LoadGameScene();
    }
}
