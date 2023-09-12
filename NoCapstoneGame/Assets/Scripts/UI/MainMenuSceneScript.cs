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

    [SerializeField] private Button startButton;
    private Button creditsButton;
    private Button quitButton;

    [SerializeField] private UIDocument CreditsUIDoc;
    private VisualElement creditsRoot;
    private Button creditsCloseButton;


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


        creditsRoot = CreditsUIDoc.rootVisualElement;
        creditsRoot.visible = false;
        creditsCloseButton = creditsRoot.Q<Button>("closeButton");
        

        startButton.clicked += () => { Debug.Log("pingu"); sceneManager.SwitchToScene(sceneManager.gameplaySceneName); };
        creditsButton.clicked += () => { creditsRoot.visible = true;  root.visible = false; } ;
        quitButton.clicked += () => { Application.Quit(); };//make this quit the game 

        creditsCloseButton.clicked += () => { creditsRoot.visible = false; root.visible = true; };
    }

    // Update is called once per frame
    void Update()
    {
    }
}
