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

    private bool canPlayAudio;

    private void Awake()
    {
        canPlayAudio = true;

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

        canPlayAudio = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Play(AudioReference audio)
    {
        //check whether a clip can be played, main thing to check is whether something is already playing I believe. but consider other edge cases
        if (canPlayAudio)
        {
            m_AudioSource.clip = audio.GetClip();
            m_AudioSource.Play();
            RestrictAudio();
        }
    }

    public void Play(AudioClip clip)
    {
        Debug.Log("play trip 1");
        if(canPlayAudio)
        {
            Debug.Log("play trip 2");
            m_AudioSource.clip = clip;
            m_AudioSource.Play();
            StartCoroutine(RestrictAudio());
            Debug.Log("play trip 2.5");
        }

    }

    //consider passing an object to destroy
    private IEnumerator RestrictAudio()
    {
        Debug.Log("play trip 3");
        canPlayAudio = false;
        while (m_AudioSource.isPlaying)
        {
            yield return null;
        }
        Debug.Log("play trip 4");
        canPlayAudio = true;
    }
}
