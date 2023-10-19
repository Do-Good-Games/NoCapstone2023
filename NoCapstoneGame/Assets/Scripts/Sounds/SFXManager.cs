using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{

    private static SFXManager _instance;
    public static SFXManager Instance { get { return _instance; } }

    [SerializeField] AudioSource m_AudioSource;
    GameManager gameManager;

    [SerializeField] public AudioClip PauseClip;

    private void Awake()
    {

        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //gameManager= GameManager.Instance;
        //m_AudioSource = gameManager.audioManager.m_AudioSource; m_AudioSource.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(AudioReference audio)
    {
        //check whether a clip can be played, main thing to check is whether something is already playing I believe. but consider other edge cases
        m_AudioSource.clip = audio.GetClip(); 
        m_AudioSource.Play();
    }

    public void Play(AudioClip clip)
    {
        m_AudioSource.clip=clip;
        m_AudioSource.Play();
    }
}
