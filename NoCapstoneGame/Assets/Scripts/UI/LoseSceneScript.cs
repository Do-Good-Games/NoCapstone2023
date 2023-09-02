using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LoseSceneScript : MonoBehaviour
{
    private SceneManager sceneManager;

    VisualElement root;

    Button restartButton ;
    Button quitButton;

    SoundPlayer soundPlayer ;

    private void OnEnable()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        sceneManager = SceneManager.Instance;
        //Application.Quit();


        root = GetComponent<UIDocument>().rootVisualElement;

        restartButton = root.Q<Button>("RestartButton");
        quitButton = root.Q<Button>("QuitButton");

        restartButton.clicked += () => sceneManager.SwitchToSceneName(sceneManager.gameplaySceneName);
        quitButton.clicked += () => Application.Quit(); //make this quit the game

        if(soundPlayer = GetComponentInParent<SoundPlayer>()){
            soundPlayer.RequestPlay();

        } else
        {
            Debug.Log("soundplayer not valid");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //soundPlayer.RequestPlay();
    }
}
