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

    [SerializeField] private OptionsManager optionsManager;
    private Button optionsButton;




    // Start is called before the first frame update
    void Start()
    {
        sceneManager = SceneManager.Instance;
        root = UIdoc.rootVisualElement;

        startButton = root.Q<Button>("StartButton");
        
        creditsButton = root.Q<Button>("CreditsButton");
        quitButton = root.Q<Button>("QuitButton");

        optionsButton = root.Q<Button>("OptionsButton");

        creditsRoot = CreditsUIDoc.rootVisualElement;
        creditsRoot.visible = false;
        creditsCloseButton = creditsRoot.Q<Button>("closeButton");
        

        startButton.clicked += () => { 
            if(PlayerPrefs.GetInt("ShowTutorial") == 0)
            {
                sceneManager.SwitchToScene(sceneManager.gameplaySceneName); 
            } else
            {
                sceneManager.SwitchToScene("TutorialScene");
            }
        };
        creditsButton.clicked += () => { creditsRoot.visible = true;  root.visible = false; } ;
        //quitButton.clicked += () => { Application.Quit(); };//make this quit the game 
        optionsButton.clicked += () => optionsManager.ShowOptionsMenu();


        creditsCloseButton.clicked += () => { creditsRoot.visible = false; root.visible = true; };
    }

    // Update is called once per frame
    void Update()
    {
    }
}
