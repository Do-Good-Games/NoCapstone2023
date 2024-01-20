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
    private Toggle muteGameToggle;
    private Toggle showTutorialToggle;

    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    public bool showTutorial;

    [Tooltip("scale of 0 to 100")]
    [SerializeField] private float defaultVolume;

    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey("ShowTutorial"))
        {
            PlayerPrefs.SetInt("ShowTutorial", 1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        root = UIDoc.rootVisualElement;
        masterVolSlider = root.Q<Slider>("MasterVolSlider");
        musicVolSlider = root.Q<Slider>("MusicVolSlider");
        sfxVolSlider = root.Q<Slider>("SFXVolSlider");

        backButton = root.Q<Button>("BackButton");

        muteGameToggle = root.Q<Toggle>("MuteGameToggle");
        showTutorialToggle = root.Q<Toggle>("ShowTutorialToggle");

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
        if (!PlayerPrefs.HasKey("isMuted"))
        {
            PlayerPrefs.SetInt("isMuted", 0);
        }
        Debug.Log("mute check " + SFXManager.Instance.isMuted);

        masterVolSlider.RegisterValueChangedCallback(OnMasterSliderValueChange);
        musicVolSlider.RegisterValueChangedCallback(OnMusicSliderValueChange);
        sfxVolSlider.RegisterValueChangedCallback(OnSfxSliderValueChange);

        muteGameToggle.RegisterValueChangedCallback(OnMuteToggleValueChange);
        showTutorialToggle.RegisterValueChangedCallback(OnTutorialToggleValueChange);

        backButton.clicked += HideOptionsMenu;

        Load();

        HideOptionsMenu();
    }

    

    public void ShowOptionsMenu()
    {
        UIDoc.rootVisualElement.style.display = DisplayStyle.Flex;


        root.style.visibility = Visibility.Visible;
    }

    public void HideOptionsMenu()
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

    public void OnMuteToggleValueChange(ChangeEvent<bool> evt)
    {
        SFXManager.Instance.isMuted = evt.newValue;
        if(evt.newValue == true)
        {
            PlayerPrefs.SetInt("isMuted", 1);
        }
        else
        {
            PlayerPrefs.SetInt("isMuted", 0);
        }

        CheckMute();
    }

    public void OnTutorialToggleValueChange(ChangeEvent<bool> evt)
    {
        showTutorial = evt.newValue;
        PlayerPrefs.SetInt("ShowTutorial", evt.newValue ? 1:0);
    }

    public void CheckMute() //sees if the volume is currently muted, this should be called whenever a volume slider is changed, or the mute button is pushed
    {
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
        showTutorialToggle.value = PlayerPrefs.GetInt("ShowTutorial") == 1? true: false;

        masterVolume = PlayerPrefs.GetFloat("masterVolume");
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        showTutorial = PlayerPrefs.GetInt("ShowTutorial") == 1 ? true : false;

        if (PlayerPrefs.GetInt("isMuted") == 0)
        {
            SFXManager.Instance.isMuted = false;         
        }
        else
        {
            SFXManager.Instance.isMuted = true;
        }
        muteGameToggle.value = PlayerPrefs.GetInt("isMuted") == 1 ? true : false;

        /*
        masterMixerGroup.audioMixer.SetFloat("MasterVolParam", Mathf.Log10(masterVolSlider.value) * 20);
        musicMixerGroup.audioMixer.SetFloat("MusicVolParam", Mathf.Log10(musicVolSlider.value) * 20);
        sfxMixerGroup.audioMixer.SetFloat("SFXVolParam", Mathf.Log10(sfxVolSlider.value) * 20);
        */
        CheckMute();

    }

    private void Save()
    {
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetInt("ShowTutorial", showTutorial ? 1 : 0);

        if(SFXManager.Instance.isMuted)
        {
            PlayerPrefs.SetInt("isMuted", 1);
        }
        else
        {
            PlayerPrefs.SetInt("isMuted", 0);
        }


    }
}
