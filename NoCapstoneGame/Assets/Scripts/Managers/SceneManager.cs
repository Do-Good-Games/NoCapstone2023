using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneManager : MonoBehaviour
{
    private static SceneManager _instance;
    public static SceneManager Instance { get { return _instance; } }

    public bool canSwitchScenes = true;

    public VisualElement sceneTransitionElement;
    public UIDocument sceneTransitionUIDoc;
    
    [SerializeField] public string gameplaySceneName = "Ryan Scene";
    


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
        sceneTransitionElement = sceneTransitionUIDoc.rootVisualElement;

        DontDestroyOnLoad(this);
        //DontDestroyOnLoad(sceneTransitionElement);
        DontDestroyOnLoad(sceneTransitionUIDoc);
    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchToScene("SampleScene");
    }


    // Update is called once per frame
    void Update()
    {

    }


    //used for if you want to switch to a scene by direct reference
    public void SwitchToScene(Scene scene)
    {
        StartCoroutine(SwitchToSceneCoroutine(scene.name));
    }



    public void SwitchToScene(string sceneName)
    {
        StartCoroutine(SwitchToSceneCoroutine(sceneName));
    }


    public IEnumerator SwitchToSceneCoroutine(string sceneName)
    {
        if (canSwitchScenes)
        {
            //place the sprite
            sceneTransitionUIDoc.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

            //fade in the sprite
            while (opacity <= 1)
            {
                opacity += fadeValue;
                sceneTransitionElement.style.opacity = opacity;
                //sceneTransitionRenderer.color = new Color(0, 0, 0, opacity);
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
                sceneTransitionElement.style.opacity = opacity;
                //sceneTransitionRenderer.color = new Color(0, 0, 0, opacity);
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




    //public IEnumerator SwitchToScenePrefabCoroutine(Scene scene)
    //{
    //    if (canSwitchScenes)
    //    {
    //        //place the sprite
    //        sceneTransitionUIDoc.transform.position = new Vector3(0.0f, 0.0f, 0.0f);

    //        //fade in the sprite
    //        while (opacity <= 1)
    //        {
    //            opacity += fadeValue;
    //            sceneTransitionElement.style.opacity = opacity;
    //               // = new Color(0, 0, 0, opacity);
    //            yield return new WaitForSeconds(0.1f);
    //        }


    //        //transition to new scene
    //        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.name);

    //        //hold control
    //        Time.timeScale = 0;

    //        //fade out the sprite
    //        while (opacity >= 0)
    //        {
    //            opacity -= fadeValue;
    //            sceneTransitionElement.style.opacity = opacity;
    //            //sceneTransitionRenderer.color = new Color(0, 0, 0, opacity);
    //            yield return new WaitForSecondsRealtime(0.1f);
    //        }

    //        //release control
    //        Time.timeScale = 1;
    //    }
    //}
}
