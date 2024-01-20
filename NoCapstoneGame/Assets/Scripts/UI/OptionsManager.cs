using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
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

    private Slider mouseSensitivitySlider;

    private Button backButton;
    private Toggle muteGameToggle;
    private Toggle showTutorialToggle;

    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;

    [Tooltip("passed a value 0 to 1, return range subject to change ")]
    [SerializeField] private AnimationCurve mouseSensitivityCurve;
    [Tooltip("zero to one")]
    public float mouseSensitivity;
    private InputAction mouseMoveAction;

    public bool showTutorial;

    [Tooltip("scale of 0 to 1")]
    [SerializeField] private float defaultVolume;

    [Tooltip("enter a value 0 to 1, this will be lerped between the min and max sensitivity values")]
    [SerializeField] private float defaultMouseSensitivity;

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
        //PlayerController playerController = GameManager.Instance.playerController;
        if (GameManager.Instance != null) {
            mouseMoveAction = GameManager.Instance.playerController.playerInput.actions.FindAction("Move");
        }

        root = UIDoc.rootVisualElement;
        masterVolSlider = root.Q<Slider>("MasterVolSlider");
        musicVolSlider = root.Q<Slider>("MusicVolSlider");
        sfxVolSlider = root.Q<Slider>("SFXVolSlider");

        mouseSensitivitySlider = root.Q<Slider>("MouseSensitivitySlider");

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
        if (!PlayerPrefs.HasKey("mouseSensitivity"))
        {
            PlayerPrefs.SetFloat("mouseSensitivity", defaultMouseSensitivity);
        }

        masterVolSlider.RegisterValueChangedCallback(OnMasterSliderValueChange);
        musicVolSlider.RegisterValueChangedCallback(OnMusicSliderValueChange);
        sfxVolSlider.RegisterValueChangedCallback(OnSfxSliderValueChange);

        mouseSensitivitySlider.RegisterValueChangedCallback(OnMouseSensitivitySliderValueChange);

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

    public void OnMuteToggleValueChange(ChangeEvent<bool> evt)
    {
        SFXManager.Instance.isMuted = evt.newValue;
        CheckMute();
    }

    public void OnTutorialToggleValueChange(ChangeEvent<bool> evt)
    {
        showTutorial = evt.newValue;
        PlayerPrefs.SetInt("ShowTutorial", evt.newValue ? 1:0);
    }

    public void OnMouseSensitivitySliderValueChange(ChangeEvent<float> evt)
    {

        mouseSensitivity = evt.newValue;
            
        PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivity);

        Debug.Log("value: " + mouseSensitivity);

        if (mouseMoveAction != null)
        {
            mouseMoveAction.ApplyParameterOverride("scaleVector2:x", mouseSensitivityCurve.Evaluate(mouseSensitivity));
            mouseMoveAction.ApplyParameterOverride("scaleVector2:y", mouseSensitivityCurve.Evaluate(mouseSensitivity));
        }
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
        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("mouseSensitivity");
        showTutorialToggle.value = PlayerPrefs.GetInt("ShowTutorial") == 1? true: false;

        masterVolume = PlayerPrefs.GetFloat("masterVolume");
        musicVolume = PlayerPrefs.GetFloat("musicVolume");
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity");
        showTutorial = PlayerPrefs.GetInt("ShowTutorial") == 1 ? true : false;

        masterMixerGroup.audioMixer.SetFloat("MasterVolParam", Mathf.Log10(masterVolSlider.value) * 20);
        musicMixerGroup.audioMixer.SetFloat("MusicVolParam", Mathf.Log10(musicVolSlider.value) * 20);
        sfxMixerGroup.audioMixer.SetFloat("SFXVolParam", Mathf.Log10(sfxVolSlider.value) * 20);

        if(mouseMoveAction != null) {
            mouseMoveAction.ApplyParameterOverride("scaleVector2:x", mouseSensitivityCurve.Evaluate(mouseSensitivity));
            mouseMoveAction.ApplyParameterOverride("scaleVector2:y", mouseSensitivityCurve.Evaluate(mouseSensitivity));
        }
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("mouseSensitivity", mouseSensitivity);
        PlayerPrefs.SetInt("ShowTutorial", showTutorial ? 1 : 0);

    }
}
