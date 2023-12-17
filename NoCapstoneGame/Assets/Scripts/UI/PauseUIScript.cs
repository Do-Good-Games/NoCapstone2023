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

    [SerializeField] AudioReference pauseReference; // BOBBY CHECK HERE -  likely an unset reference

    [SerializeField] private UIDocument UIDoc;

    [SerializeField] private StyleSheet normalStyleSheet;
    [SerializeField] private StyleSheet hoverStyleSheet;
    [SerializeField] private StyleSheet clickedStyleSheet;


    private VisualElement root;
    private Button resumeButton;
    private Button restartButton;
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
        quitButton = UIDoc.rootVisualElement.Q<Button>("QuitButton");

        //resumeButton.clicked += () => sceneManager.SwitchToSceneName(sceneManager.gameplaySceneName); //unpause the game
        resumeButton.clicked += () => { gameManager.ResumeGame(); };
        mainMenuButton.clicked += MainMenuClicked;
        restartButton.clicked += RestartClicked;
        quitButton.clicked += () => Application.Quit();

        //https://forum.unity.com/threads/question-how-do-i-detect-if-the-mouse-is-over-any-ui-element-ui-elements.829575/
        //mainMenuButton.RegisterCallback<MouseEnterEvent> 
        mainMenuButton.RegisterCallback<MouseDownEvent>(MainMenuDown);
        mainMenuButton.RegisterCallback<MouseEnterEvent> (MainMenuHovered);
        mainMenuButton.RegisterCallback<MouseLeaveEvent>(MainMenuLeft);

        restartButton.RegisterCallback<MouseDownEvent>(RestartDown);
        restartButton.RegisterCallback<MouseEnterEvent>(RestartHovered);
        restartButton.RegisterCallback<MouseLeaveEvent>(RestartLeft);

        resumeButton.RegisterCallback<MouseDownEvent>(ResumeDown);
        resumeButton.RegisterCallback<MouseEnterEvent>(ResumeHovered);
        resumeButton.RegisterCallback<MouseLeaveEvent>(ResumeLeft);

        gameManager.OnGamePause.AddListener(ShowHidePauseMenu);//see comments prefacing method declaration for explanation(?) on why I'm doing it like this
        gameManager.OnGameResume.AddListener(ShowHidePauseMenu);

        //root.SetEnabled(false); root.visible = false;

        root.style.visibility = Visibility.Hidden;
        //root.SetEnabled(false);
        //root.visible = false;
        
        sfxManager = SFXManager.Instance;
    }


    private void MainMenuDown(MouseDownEvent evt)
    {
        //https://forum.unity.com/threads/how-would-i-change-a-property-from-a-stylesheet-selector-by-script.1385697/
        Debug.Log("down");
        mainMenuButton.AddToClassList("buttonPressed");
        mainMenuButton.RemoveFromClassList("buttonHover");
        mainMenuButton.RemoveFromClassList("button");
    }

    private void MainMenuHovered(MouseEnterEvent evt)
    {
        mainMenuButton.AddToClassList("buttonHover");
        mainMenuButton.RemoveFromClassList("button");
        mainMenuButton.RemoveFromClassList("buttonPressed");
    }

    private void MainMenuLeft(MouseLeaveEvent evt)
    {
        mainMenuButton.AddToClassList("button");
        mainMenuButton.RemoveFromClassList("buttonHover");
        mainMenuButton.RemoveFromClassList("buttonPressed");
    }


    private void RestartDown(MouseDownEvent evt)
    {
        restartButton.AddToClassList("buttonPressed");
        restartButton.RemoveFromClassList("buttonHover");
        restartButton.RemoveFromClassList("button");
    }

    private void RestartHovered(MouseEnterEvent evt)
    {
        restartButton.AddToClassList("buttonHover");
        restartButton.RemoveFromClassList("button");
        restartButton.RemoveFromClassList("buttonPressed");
    }

    private void RestartLeft(MouseLeaveEvent evt)
    {
        restartButton.AddToClassList("button");
        restartButton.RemoveFromClassList("buttonHover");
        restartButton.RemoveFromClassList("buttonPressed");
    }

    private void ResumeDown(MouseDownEvent evt)
    {
        resumeButton.AddToClassList("buttonPressed");
        resumeButton.RemoveFromClassList("buttonHover");
        resumeButton.RemoveFromClassList("button");
    }

    private void ResumeHovered(MouseEnterEvent evt)
    {
        resumeButton.AddToClassList("buttonHover");
        resumeButton.RemoveFromClassList("button");
        resumeButton.RemoveFromClassList("buttonPressed");
    }

    private void ResumeLeft(MouseLeaveEvent evt)
    {
        resumeButton.AddToClassList("button");
        resumeButton.RemoveFromClassList("buttonHover");
        resumeButton.RemoveFromClassList("buttonPressed");
    }


    private void OnEnable()
    {

    }

    private void RestartClicked()
    {
        sceneManager.SwitchToScene(sceneManager.gameplaySceneName);
    }

    private void MainMenuClicked()
    {
        sceneManager.GoToMainMenu();    
    }


    //I'm not sure if this is the best way to do this, Dunagan advises to separate functionality wherever possible, but this allows a quick check
    //to see whether this method is getting called in menus (which it shouldn't). 
    private void ShowHidePauseMenu()
    {
    
        Debug.Log("check 6");
        if(pauseReference != null)
        {
            Debug.Log("check 6.1");
            Debug.Log("successfully instanced");
            //sfxManager.Play(pauseReference.GetClip());

        } else
        {
            Debug.Log("check 6.2");
            Debug.LogWarning("PAUSEREFERENCE INVALID");

        }
        
        if (gameManager.gameState == GameState.paused)
        {
            Debug.Log("check 6.3");
            root.style.visibility = Visibility.Visible;
            //root.SetEnabled(true);
            //root.visible = true;
        } 
        else if(gameManager.gameState == GameState.gameplay)
        {
            Debug.Log("check 6.4");
            root.style.visibility = Visibility.Hidden;

            //root.SetEnabled(false);
            //root.visible = false;
        } else
        {
            throw new Exception("Pausemenu.TogglePause is somehow being called in an unpredicted gamestate");
        }
        Debug.Log("check 7");

    }
}
