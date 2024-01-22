using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
//using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class OptionsSceneScript : MonoBehaviour
{
    private SceneManager sceneManager;

    public AudioMixer audioMixer;

    //values for our slider
    Slider masterVolumeSlider;
    Slider musicVolumeSlider;
    Slider soundEffectsVolumeSlider;

    float masterValue;
    float musicValue;
    float soundEffectsValue;



    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;


        //Slider masterVolumeSlider = root.Q<UnityEngine.UI.Slider>("MasterVolSlider");
        masterVolumeSlider = root.Q<Slider>("MasterVolSlider");
        musicVolumeSlider = root.Q<Slider>("MusicVolSlider");
        soundEffectsVolumeSlider = root.Q<Slider>("SFXVolSlider");

        masterValue = masterVolumeSlider.value;
        musicValue = musicVolumeSlider.value;
        soundEffectsValue = soundEffectsVolumeSlider.value;

        
        //masterVolumeSlider.OnValueChanged += () => Application.Quit();
        //masterVolumeSlider.OnValueChanged += () => ChangeAudio("MasterVolParam", masterVolumeSlider.normalizedValue;


    }

    // Start is called before the first frame update
    void Start()
    {
        sceneManager = SceneManager.Instance;
    }

    private void Update()
    { //horrid idea, but I couldn't find a version of slider that would listen to events and could be referenced OnEnable
        if(masterValue != masterVolumeSlider.value)
        {
            masterValue = masterVolumeSlider.value;
            ChangeAudio("MasterVolParam", masterValue);
        }
        if (musicValue != musicVolumeSlider.value)
        {
            musicValue = musicVolumeSlider.value;
            ChangeAudio("MusicVolParam", musicValue);
        }
        if(soundEffectsValue != soundEffectsVolumeSlider.value)
        {
            soundEffectsValue = soundEffectsVolumeSlider.value;
            ChangeAudio("SFXVolParam", soundEffectsValue);
        }
    }

    private void ChangeAudio(string channelName, float givenValue) //given value might not be given in the method description
    {
        Debug.Log("changing " + channelName + " to " + givenValue);
        audioMixer.SetFloat(channelName, givenValue);
    }
}
