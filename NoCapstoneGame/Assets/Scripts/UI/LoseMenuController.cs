using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LoseMenuController : MonoBehaviour
{
    private SceneManager sceneManager;
    SFXManager sfxManager;
    private GameManager gameManager;

    VisualElement root;

    Button restartButton ;
    //Button quitButton;
    Button mainMenuButton;

    private Label highScore;
    private Label finalScore;

    SoundPlayer soundPlayer ;
    [SerializeField] AudioReference deathSoundReference;

    [SerializeField] private UIDocument UIDoc;


    private void OnEnable()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
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
        gameManager.OnPlayerDeath.AddListener(ShowDeathMenu);
    }

    public void ShowDeathMenu()
    {

        if (!PlayerPrefs.HasKey("highScore"))
        {
            PlayerPrefs.SetFloat("highScore", gameManager.GetScore());
        }
        else
        {
            //check if gamescore is greater than highscore
            if (PlayerPrefs.GetFloat("highScore") < gameManager.GetScore())
            {
                PlayerPrefs.SetFloat("highScore", gameManager.GetScore()); 
            }
        }

        //set the text
        finalScore.text = ("Current Score \n" + gameManager.GetScore());
        highScore.text = ("High Score \n" + PlayerPrefs.GetFloat("highScore"));
        //set dimensions and position of the menu so it fits in the hud
        root.style.visibility = Visibility.Visible;
    }

    // Update is called once per frame
    void Update()
    {
        //soundPlayer.RequestPlay();
    }
}
