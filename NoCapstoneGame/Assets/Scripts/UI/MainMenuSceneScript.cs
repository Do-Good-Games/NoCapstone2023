using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuSceneScript : MonoBehaviour
{
    private SceneManager sceneManager;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button startButton = root.Q<Button>("StartButton");
        Button creditsButton = root.Q<Button>("CreditsButton");
        Button quitButton = root.Q<Button>("QuitButton");

        startButton.clicked += () => sceneManager.SwitchToSceneName(sceneManager.gameplaySceneName);
        creditsButton.clicked += () => sceneManager.SwitchToSceneName("CreditsScene");
        quitButton.clicked += () => Application.Quit(); //make this quit the game
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
