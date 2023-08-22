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

        resumeButton.clicked += () => sceneManager.SwitchToSceneName("Ian Scene"); //unpause the game
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
