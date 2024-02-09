using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private AudioSource mainLoop;
    [SerializeField] private AudioSource boostLoop;
    [SerializeField] private AudioSource boostStart;
    [SerializeField] private AudioSource gameOverLoop;

    //webGL builds have a glitch where pausing doesn't properly keep clips at the same time stamp
    private float mainLoopTime;
    private float boostLoopTime;
    private float boostStartTime;

    private bool inBoost;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        inBoost = false;

        gameManager.OnBoostStart.AddListener(StartBoostMusic);
        gameManager.OnBoostEnd.AddListener(StopBoostMusic);

        gameManager.OnGamePause.AddListener(PauseMusic);
        gameManager.OnGameResume.AddListener(ResumeMusic);

        gameManager.OnPlayerDeath.AddListener(startGameOverMusic);
    }

    private void StartBoostMusic()
    {
        inBoost = true;
        // Debug.Log("event received");
        mainLoop.Stop();
        boostLoop.Play();
        boostStart.Play();
    }

    private void StopBoostMusic()
    {
        inBoost = false;
        // Debug.Log("event received");
        mainLoop.Play();
        boostLoop.Stop();
        boostStart.Stop();
    }

    private void PauseMusic()
    {
        if (inBoost)
        {
            boostLoopTime = boostLoop.time;
            boostLoop.Pause();
            if (boostLoop.isPlaying)
            {
                boostStartTime = boostLoop.time;
                boostStart.Play();
            } else
            {
                boostStartTime = -1;
            }
        } else
        {
            mainLoopTime = mainLoop.time;
            mainLoop.Pause();
        }
    }

    private void startGameOverMusic(){
        boostLoop.Stop();
        mainLoop.Stop();

        gameOverLoop.Play();

    }

    private void ResumeMusic()
    {

        if (inBoost)
        {
            boostLoop.time = boostLoopTime;
            boostLoop.Play();
            if (boostStartTime != -1 )
            {
                boostStart.time = boostStartTime;
                boostStart.Play();
            }
        }
        else
        {
            mainLoop.time = mainLoopTime;
            mainLoop.Play();
        }
    }
}
