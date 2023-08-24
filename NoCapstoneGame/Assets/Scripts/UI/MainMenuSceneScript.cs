using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuSceneScript : MonoBehaviour
{
    private SceneManager sceneManager;

    [SerializeField] UIDocument UIdoc;

    private VisualElement root;

    private Button startButton;
    private Button creditsButton;
    private Button quitButton;

    private void OnEnable()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        sceneManager = SceneManager.Instance;
        root = UIdoc.rootVisualElement;

        startButton = root.Q<Button>("StartButton");
        creditsButton = root.Q<Button>("CreditsButton");
        quitButton = root.Q<Button>("QuitButton");

        startButton.clicked += () => { Debug.Log("ping"); sceneManager.SwitchToSceneName(sceneManager.gameplaySceneName); };
        creditsButton.clicked += () => sceneManager.SwitchToSceneName("CreditsScene");
        quitButton.clicked += () => Application.Quit(); //make this quit the game
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
