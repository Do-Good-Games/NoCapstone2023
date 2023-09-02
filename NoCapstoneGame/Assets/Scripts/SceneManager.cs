using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    private static SceneManager _instance;
    public static SceneManager Instance { get { return _instance; } }

    public bool canSwitchScenes = true;

    public GameObject sceneTransitionSprite;
    public SpriteRenderer sceneTransitionRenderer;


    public float opacity = 0f;
    public float fadeValue = 0.5f;



    void OnEnable()
    {
        Debug.Log("OnEnable called");

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

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

        DontDestroyOnLoad(this);
        DontDestroyOnLoad(sceneTransitionSprite);
        DontDestroyOnLoad(sceneTransitionRenderer);
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchToSceneName("SampleScene");
    }


    // Update is called once per frame
    void Update()
    {

    }


    //used for if you want to switch to a scene by direct reference
    public void SwitchToScenePrefab(Scene scene)
    {
        StartCoroutine(SwitchToScenePrefabCoroutine(scene));
    }

    public IEnumerator SwitchToScenePrefabCoroutine(Scene scene)
    {
        if (canSwitchScenes)
        {
            //place the sprite
            sceneTransitionSprite.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

            //fade in the sprite
            while (opacity <= 1)
            {
                opacity += fadeValue;
                sceneTransitionRenderer.color = new Color(0, 0, 0, opacity);
                yield return new WaitForSeconds(0.1f);
            }


            //transition to new scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);

            //hold control
            Time.timeScale = 0;

            //fade out the sprite
            while (opacity >= 0)
            {
                opacity -= fadeValue;
                sceneTransitionRenderer.color = new Color(0, 0, 0, opacity);
                yield return new WaitForSecondsRealtime(0.1f);
            }

            //release control
            Time.timeScale = 1;
        }
    }


    public void SwitchToSceneName(string sceneName)
    {
        StartCoroutine(SwitchToSceneNameCoroutine(sceneName));
    }

    public IEnumerator SwitchToSceneNameCoroutine(string sceneName)
    {
        if (canSwitchScenes)
        {
            //place the sprite
            sceneTransitionSprite.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

            //fade in the sprite
            while (opacity <= 1)
            {
                opacity += fadeValue;
                sceneTransitionRenderer.color = new Color(0, 0, 0, opacity);
                yield return new WaitForSeconds(0.1f);
            }


            //transition to new scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);

            //hold control
            Time.timeScale = 0;

            //fade out the sprite
            while (opacity >= 0)
            {
                opacity -= fadeValue;
                sceneTransitionRenderer.color = new Color(0, 0, 0, opacity);
                yield return new WaitForSecondsRealtime(0.1f);
            }

            //release control
            Time.timeScale = 1;
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
