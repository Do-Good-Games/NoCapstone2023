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
    private VisualElement root;
    private Slider masterVolSlider;
    private Slider musicVolSlider;
    private Slider sfxVolSlider;
    private Button backButton;

    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    [Tooltip("scale of 0 to 100")]
    [SerializeField] private float defaultVolume;

    // Start is called before the first frame update
    void Start()
    {
        root = UIDoc.rootVisualElement;
        masterVolSlider = root.Q<Slider>("MasterVolSlider");
        musicVolSlider = root.Q<Slider>("MusicVolSlider");
        sfxVolSlider = root.Q<Slider>("SFXVolSlider");

        backButton = root.Q<Button>("BackButton");

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

        //if any key hasn't been set, set it to the default
        if (!PlayerPrefs.HasKey("masterVolume"))
        {
            PlayerPrefs.SetFloat("masterVolume", defaultVolume);
        }
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", defaultVolume);

        }
        if (!PlayerPrefs.HasKey("sfxVolume"))
        {
            PlayerPrefs.SetFloat("sfxVolume", defaultVolume);
        }
        if (!PlayerPrefs.HasKey("tutorialDone"))
        {
            PlayerPrefs.SetInt("tutorialDone", 0); //I am setting tutorial done to an int 0=false 1=true because player prefs can't use bools
        }

        masterVolSlider.RegisterValueChangedCallback(OnMasterSliderValueChange);
        musicVolSlider.RegisterValueChangedCallback(OnMusicSliderValueChange);
        sfxVolSlider.RegisterValueChangedCallback(OnSfxSliderValueChange);

        backButton.clicked += HideOptionsMenu;

        Load();

        HideOptionsMenu();
    }

    

    public void ShowOptionsMenu()
    {
        UIDoc.rootVisualElement.style.display = DisplayStyle.Flex;


        root.style.visibility = Visibility.Visible;
    }

    private void HideOptionsMenu()
    {
        UIDoc.rootVisualElement.style.display = DisplayStyle.None;
        root.style.visibility = Visibility.Hidden;
    }


    public void OnMasterSliderValueChange(ChangeEvent<float> evt)
    {
        masterVolume = evt.newValue;
        masterMixerGroup.audioMixer.SetFloat("MasterVolParam", Mathf.Log10(evt.newValue) * 20);
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        CheckMute();
    }

    public void OnMusicSliderValueChange(ChangeEvent<float> evt)
    {
        musicVolume = evt.newValue;
        musicMixerGroup.audioMixer.SetFloat("MusicVolParam", Mathf.Log10(evt.newValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        CheckMute();
    }
    public void OnSfxSliderValueChange(ChangeEvent<float> evt)
    {
        sfxVolume = evt.newValue;
        sfxMixerGroup.audioMixer.SetFloat("SFXVolParam", Mathf.Log10(evt.newValue) * 20);
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
        CheckMute();
    }

    public void CheckMute() //sees if the volume is currently muted, this should be called whenever a volume slider is changed, or the mute button is pushed
    {
        Debug.Log("check mute called");
        if(SFXManager.Instance.isMuted)
        {
            //set all volumes to mute (-80 decibels)
            masterMixerGroup.audioMixer.SetFloat("MasterVolParam", -80);
            musicMixerGroup.audioMixer.SetFloat("MusicVolParam", -80);
            sfxMixerGroup.audioMixer.SetFloat("SFXVolParam", -80);
        }
        else
        {
            //set all volumes to their appropriate values
            masterMixerGroup.audioMixer.SetFloat("MasterVolParam", Mathf.Log10(masterVolume) * 20);
            musicMixerGroup.audioMixer.SetFloat("MusicVolParam", Mathf.Log10(musicVolume) * 20);
            sfxMixerGroup.audioMixer.SetFloat("SFXVolParam", Mathf.Log10(sfxVolume) * 20);
        }
    }

    public void OnDestroy()
    {
        Save();
    }


    public void Load()
    {
        masterVolSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicVolSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxVolSlider.value = PlayerPrefs.GetFloat("sfxVolume");

        masterVolume = PlayerPrefs.GetFloat("masterVolume");
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");

        masterMixerGroup.audioMixer.SetFloat("MasterVolParam", Mathf.Log10(masterVolSlider.value) * 20);
        musicMixerGroup.audioMixer.SetFloat("MusicVolParam", Mathf.Log10(musicVolSlider.value) * 20);
        sfxMixerGroup.audioMixer.SetFloat("SFXVolParam", Mathf.Log10(sfxVolSlider.value) * 20);
        //if we were storing tooltips elsewhere, then we'd set that here as well
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }
}
