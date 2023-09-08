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

    [SerializeField] private UIDocument UIDoc;

    private VisualElement root;
    private Button resumeButton;
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
        quitButton = UIDoc.rootVisualElement.Q<Button>("QuitButton");

        //resumeButton.clicked += () => sceneManager.SwitchToSceneName(sceneManager.gameplaySceneName); //unpause the game
        resumeButton.clicked += () => { gameManager.ResumeGame(); };
        mainMenuButton.clicked += MainMenuClicked;
        quitButton.clicked += () => Application.Quit();

        gameManager.OnGamePause.AddListener(ShowHidePauseMenu);//see comments prefacing method declaration for explanation(?) on why I'm doing it like this
        gameManager.OnGameResume.AddListener(ShowHidePauseMenu);

        //root.SetEnabled(false); root.visible = false;

        root.style.visibility = Visibility.Hidden;
        //root.SetEnabled(false);
        //root.visible = false;
    }

    private void OnEnable()
    {

    }

    private void MainMenuClicked()
    {
        sceneManager.GoToMainMenu();
      
    }


    //I'm not sure if this is the best way to do this, Dunagan advises to separate functionality wherever possible, but this allows a quick check
    //to see whether this method is getting called in menus (which it shouldn't). 
    private void ShowHidePauseMenu()
    {
        if (gameManager.gameState == GameState.paused)
        {
            root.style.visibility = Visibility.Visible;
            //root.SetEnabled(true);
            //root.visible = true;
        } else if(gameManager.gameState == GameState.gameplay)
        {
            root.style.visibility = Visibility.Hidden;

            //root.SetEnabled(false);
            //root.visible = false;
        } else
        {
            throw new Exception("Pausemenu.TogglePause is somehow being called in an unpredicted gamestate");
        }
        
    }
}
