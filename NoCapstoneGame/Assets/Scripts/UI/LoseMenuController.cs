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
    Button quitButton;

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

        restartButton = UIDoc.rootVisualElement.Q<Button>("RestartButton");
        quitButton = UIDoc.rootVisualElement.Q<Button>("QuitButton");

        restartButton.clicked += () => { sceneManager.SwitchToScene(sceneManager.gameplaySceneName); };
        quitButton.clicked += () => Application.Quit(); //make this quit the game

        root.style.visibility = Visibility.Hidden;

    }

    public void ShowDeathMenu()
    {
        root.style.visibility = Visibility.Visible;
    }

    // Update is called once per frame
    void Update()
    {
        //soundPlayer.RequestPlay();
    }
}
