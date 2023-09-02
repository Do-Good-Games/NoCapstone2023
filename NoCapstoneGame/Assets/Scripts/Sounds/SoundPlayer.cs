using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;
using System;

//[CreateAssetMenu(menuName = "Audio/SoundPlayer", fileName = "Sound Player")]
public class SoundPlayer : MonoBehaviour
{

    [SerializeField] private AudioClip clip;
    private SFXManager sfxManager;


    // Start is called before the first frame update
    void Start()
    {
        if(sfxManager = SFXManager.Instance)
        {
            Debug.Log("instance sfxman");

        } else
        {
            Debug.Log("penis");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RequestPlay()
    {
        if (sfxManager != null)
        {
            sfxManager.Play(clip);
        } else
        {
            sfxManager = SFXManager.Instance;
            sfxManager.Play(clip);
        }

        //return true;
    }


}
