using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuSceneScript : MonoBehaviour
{

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button startButton = root.Q<Button>("StartButton");
        Button creditsButton = root.Q<Button>("CreditsButton");
        Button quitButton = root.Q<Button>("QuitButton");

        startButton.clicked += () => SceneManager.LoadScene("Ian Scene");
        creditsButton.clicked += () => SceneManager.LoadScene("CreditsScene");
        quitButton.clicked += () => Application.Quit(); //make this quit the game
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
