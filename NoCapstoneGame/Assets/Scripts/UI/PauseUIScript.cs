using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PauseUIScript : MonoBehaviour
{
    private SceneManager sceneManager;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button resumeButton = root.Q<Button>("ResumeButton");
        Button mainMenuButton = root.Q<Button>("MainMenuButton");
        Button quitButton = root.Q<Button>("QuitButton");

        //resumeButton.clicked += () => sceneManager.SwitchToSceneName(sceneManager.gameplaySceneName); //unpause the game
        //I don't think we want to switch to the scene
        //TODO: set this callback to playercontroller.togglePause, or maybe refactor that fucntion to be invoked on the togglePauseEvent
        //update: do this by refactoring playerController.togglePause to be two functions. one invokes the event and one swaps the control scheme
        mainMenuButton.clicked += () => sceneManager.SwitchToSceneName("MainMenuScene");
        quitButton.clicked += () => Application.Quit();
    }


    // Start is called before the first frame update
    void Start()
    {
        sceneManager = SceneManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
