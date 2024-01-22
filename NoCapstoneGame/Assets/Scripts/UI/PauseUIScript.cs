using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseUIScript : MonoBehaviour
{
    private GameManager gameManager;
    private SceneManager sceneManager;
    private SFXManager sfxManager;
    
    [SerializeField] private OptionsManager optionsManager;

    [SerializeField] AudioReference pauseReference; // BOBBY CHECK HERE -  likely an unset reference

    [SerializeField] private UIDocument UIDoc;

    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource ambientBuzz;
    [SerializeField] private AudioSource pauseAppear;


    private VisualElement root;
    private Button resumeButton;
    private Button restartButton;
    private Button optionsButton;
    private Button mainMenuButton;
    Button quitButton;



     // Start is called before the first frame update
    void Start()
    {

        sceneManager = SceneManager.Instance;
        gameManager = GameManager.Instance;

        root = UIDoc.rootVisualElement;

        resumeButton = UIDoc.rootVisualElement.Q<Button>("ResumeButton");
        mainMenuButton = UIDoc.rootVisualElement.Q<Button>("MainMenuButton");
        restartButton = UIDoc.rootVisualElement.Q<Button>("RestartButton");
        optionsButton = UIDoc.rootVisualElement.Q<Button>("OptionsButton");
        quitButton = UIDoc.rootVisualElement.Q<Button>("QuitButton");

        //resumeButton.clicked += () => sceneManager.SwitchToSceneName(sceneManager.gameplaySceneName); //unpause the game
        resumeButton.clicked += () => {
            buttonClick.Play(); gameManager.ResumeGame(); 
        };
        mainMenuButton.clicked += MainMenuClicked;
        restartButton.clicked += RestartClicked;
        optionsButton.clicked += OptionsClicked;
        quitButton.clicked += () => Application.Quit();


        gameManager.OnGamePause.AddListener(ShowHidePauseMenu);//see comments prefacing method declaration for explanation(?) on why I'm doing it like this
        gameManager.OnGameResume.AddListener(ShowHidePauseMenu);

        //root.SetEnabled(false); root.visible = false;

        root.style.visibility = Visibility.Hidden;
        //root.SetEnabled(false);
        //root.visible = false;
        
        sfxManager = SFXManager.Instance;
    }

    private void OptionsClicked()
    {
        buttonClick.Play();
        optionsManager.ShowOptionsMenu();
    }
    private void RestartClicked()
    {
        buttonClick.Play();
        sceneManager.SwitchToScene(sceneManager.gameplaySceneName);
    }

    private void MainMenuClicked()
    {
        buttonClick.Play();
        sceneManager.GoToMainMenu();    
    }


    //I'm not sure if this is the best way to do this, Dunagan advises to separate functionality wherever possible, but this allows a quick check
    //to see whether this method is getting called in menus (which it shouldn't). 
    private void ShowHidePauseMenu()
    {
        pauseAppear.Play();
        /*
        if(pauseReference != null)
        {

            if(sfxManager != null)
            {
                sfxManager.Play(pauseReference.GetClip());
            } else
            {
                Debug.LogWarning("SFXManager INVALID");

            }

        } else
        {
            Debug.LogWarning("PAUSEREFERENCE INVALID");

        }*/
        
        if (gameManager.gameState == GameState.paused)
        {
            root.style.visibility = Visibility.Visible;
            ambientBuzz.Play();
            //root.SetEnabled(true);
            //root.visible = true;
        } 
        else if(gameManager.gameState == GameState.gameplay)
        {
            root.style.visibility = Visibility.Hidden;
            optionsManager.HideOptionsMenu();
            ambientBuzz.Stop();
            //root.SetEnabled(false);
            //root.visible = false;
        } else
        {
            throw new Exception("Pausemenu.TogglePause() is somehow being called in an unpredicted gamestate");
        }

    }
}
