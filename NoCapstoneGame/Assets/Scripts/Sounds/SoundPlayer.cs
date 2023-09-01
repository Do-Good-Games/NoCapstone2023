using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
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

    public bool RequestPlay()
    {
        sfxManager.Play(clip);

        return true;
    }

}
