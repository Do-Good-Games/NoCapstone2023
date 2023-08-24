using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseUIScript : MonoBehaviour
{
    private GameManager gameManager;
    private SceneManager sceneManager;

    [SerializeField] private UIDocument UIDoc;

    private VisualElement root;
    private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
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
        resumeButton.clicked += () => { gameManager.TogglePause(); };
        //I don't think we want to switch to the scene
        //TODO: set this callback to playercontroller.togglePause, or maybe refactor that fucntion to be invoked on the togglePauseEvent
        //update: do this by refactoring playerController.togglePause to be two functions. one invokes the event and one swaps the control scheme
        //mainMenuButton.clicked += () => {Debug.Log("menu button clicked"); sceneManager.SwitchToSceneName("MainMenuScene");  };
        mainMenuButton.clicked += MainMenuClicked;
        quitButton.clicked += () => Application.Quit();

        gameManager.OnGameTogglePause.AddListener(TogglePauseMenu);

        //root.SetEnabled(false); root.visible = false;

        root.style.visibility = Visibility.Hidden;
        //root.SetEnabled(false);
        //root.visible = false;
    }

    private void OnEnable()
    {
        root = UIDoc.rootVisualElement;

        resumeButton = UIDoc.rootVisualElement.Q<Button>("ResumeButton");
        mainMenuButton = UIDoc.rootVisualElement.Q<Button>("MainMenuButton");
        quitButton = UIDoc.rootVisualElement.Q<Button>("QuitButton");

    }

    private void MainMenuClicked()
    {
        Debug.Log("main menu clicked");
    }

    private void TogglePauseMenu()
    {
        Debug.Log("toggle pause menu called");
        if (gameManager.paused)
        {
            root.style.visibility = Visibility.Visible;
            //root.SetEnabled(true);
            //root.visible = true;
        } else
        {
            root.style.visibility = Visibility.Hidden;

            //root.SetEnabled(false);
            //root.visible = false;
        }
    }
}
