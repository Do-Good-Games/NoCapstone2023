using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LoseMenuController : MonoBehaviour
{
    private SceneManager sceneManager;

    VisualElement root;

    Button restartButton ;
    //Button quitButton;
    Button mainMenuButton;

    private Label highScore;
    private Label finalScore;

    SoundPlayer soundPlayer ;
    [SerializeField] AudioReference deathSoundReference;
    SFXManager sfxManager;

    [SerializeField] private UIDocument UIDoc;


    private void OnEnable()
    {
    }

    // Start is called before the first frame update
    void Start()
    {



        sceneManager = SceneManager.Instance;
        sfxManager = SFXManager.Instance;
        //Application.Quit();


        root = UIDoc.rootVisualElement;

        highScore = root.Q<Label>("HighScore");
        finalScore = root.Q<Label>("FinalScore");

        restartButton = UIDoc.rootVisualElement.Q<Button>("RestartButton");
        //quitButton = UIDoc.rootVisualElement.Q<Button>("QuitButton");
        mainMenuButton = UIDoc.rootVisualElement.Q<Button>("MainMenuButton");

        restartButton.clicked += () => { sceneManager.SwitchToScene(sceneManager.gameplaySceneName); };
        //quitButton.clicked += () => Application.Quit(); //make this quit the game
        mainMenuButton.clicked += () => { sceneManager.SwitchToScene("MainMenuScene"); };

        root.style.visibility = Visibility.Hidden;

    }

    public void ShowDeathMenu()
    {

        if (!PlayerPrefs.HasKey("highScore"))
        {
            PlayerPrefs.SetFloat("highScore", GameManager.Instance.GetScore());
        }
        else
        {
            //check if gamescore is greater than highscore
            if (PlayerPrefs.GetFloat("highScore") < GameManager.Instance.GetScore())
            {
                PlayerPrefs.SetFloat("highScore", GameManager.Instance.GetScore());
            }
        }

        //set the text
        finalScore.text = ("Current Score: " + GameManager.Instance.GetScore());
        highScore.text = ("High Score: " + PlayerPrefs.GetFloat("highScore"));
        //set dimensions and position of the menu so it fits in the hud
        root.style.visibility = Visibility.Visible;
    }

    // Update is called once per frame
    void Update()
    {
        //soundPlayer.RequestPlay();
    }
}
