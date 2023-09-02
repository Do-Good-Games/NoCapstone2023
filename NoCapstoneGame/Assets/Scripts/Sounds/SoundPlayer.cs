using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;
using System;

public class SoundPlayer : MonoBehaviour
{
    private enum whenCalled { onStart, onStop }

    [SerializeField] private AudioClip clip;
    private SFXManager sfxManager;




    // Start is called before the first frame update
    void Start()
    {
        sfxManager = GameManager.Instance.sfxManager;
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RequestPlay()
    {
        sfxManager.Play(clip);

        //return true;
    }


}
