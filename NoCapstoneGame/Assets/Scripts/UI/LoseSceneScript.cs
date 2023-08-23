using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LoseSceneScript : MonoBehaviour
{
    private SceneManager sceneManager;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button restartButton = root.Q<Button>("RestartButton");
        Button quitButton = root.Q<Button>("QuitButton");

        restartButton.clicked += () => sceneManager.SwitchToSceneName("Ian Scene");
        quitButton.clicked += () => Application.Quit(); //make this quit the game
    }

    // Start is called before the first frame update
    void Start()
    {
        sceneManager = SceneManager.Instance;
        //Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
