using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitching : MonoBehaviour
{
    private static SceneSwitching instance;

    public bool canSwitchScenes = true;

    private void Awake()
    {
        canSwitchScenes = true;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void SwitchToSceneName(string sceneName)
    {
        if (canSwitchScenes)
        {
            //include loading screen type thing
            SceneManager.LoadScene(sceneName);
        }

    }

    void GoToMainMenu()
    {
        if (canSwitchScenes)
        {
            //SceneManager.LoadScene(MainMenuScene);    //this branch does not know about mainmenuscene
        }

    }
}
