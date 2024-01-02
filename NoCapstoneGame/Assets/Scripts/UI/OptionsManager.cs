using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class OptionsManager : MonoBehaviour
{

    public static OptionsManager _instance;
    public static OptionsManager Instance { get { return _instance; } }

    [SerializeField] AudioMixerGroup masterMixerGroup;
    [SerializeField] AudioMixerGroup musicMixerGroup;
    [SerializeField] AudioMixerGroup sfxMixerGroup;

    [SerializeField] UIDocument UIDoc;
    VisualElement root;

    // Start is called before the first frame update
    void Start()
    {
        //SINGLETON PATTERN - ensures that there only ever exists a single optionsManager

        //is this the first time we've created this singleton
        if (_instance == null)
        {
            //we're the first optionsManager, so assign ourselves to this instance
            _instance = this;

            // don't keep ourselves between levels
        }
        else
        {
            //if there's another one, then destroy this one
            Destroy(this.gameObject);
        }
        if (!PlayerPrefs.HasKey("masterVolume"))
        {
            Debug.Log("uwu1");
            PlayerPrefs.SetFloat("masterVolume", .75f);
        }
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            Debug.Log("uwu1");
            PlayerPrefs.SetFloat("musicVolume", .75f);

        }
        if (!PlayerPrefs.HasKey("sfxVolume"))
        {
            Debug.Log("uwu2");
            PlayerPrefs.SetFloat("sfxVolume", .75f);
        }

        if (!PlayerPrefs.HasKey("showToolTips"))
        {
            PlayerPrefs.SetInt("showToolTips", BoolToInt(true));
        }


        //Load();
        gameObject.SetActive(false);
    }

    private int BoolToInt(bool b)
    {
        return b ? 1 : 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
