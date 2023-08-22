using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    private static SceneManager _instance;
    public static SceneManager Instance { get { return _instance; } }

    public bool canSwitchScenes = true;

    private void Awake()
    {
        canSwitchScenes = true;

        if (_instance == null)
        {
            _instance = this;
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

    //used for if you want to switch to a scene by direct reference
    public void SwitchToScenePrefab(Scene scene)
    {
        if (canSwitchScenes)
        {
            //include loading screen type thing
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);
        }

    }

    public void SwitchToSceneName(string sceneName)
    {
        if (canSwitchScenes)
        {
            //include loading screen type thing
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

    }

    public void GoToMainMenu()
    {
        if (canSwitchScenes)
        {
            //SceneManager.LoadScene(MainMenuScene);    //this branch does not know about mainmenuscene
        }

    }
}
