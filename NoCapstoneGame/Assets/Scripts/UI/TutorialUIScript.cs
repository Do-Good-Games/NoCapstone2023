using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class TutorialUIScript : MonoBehaviour
{

    [SerializeField] UIDocument UIDoc;

    private VisualElement root;

    private Button leftButton;
    private Button rightButton;
    private Button endTutorialButton;
    private VisualElement background;

    [SerializeField] private Sprite[] spriteArr;

    private StyleBackground[] backgroundArray;
    private int backGroundArrayIndex = 0;

    private GameManager gameManager;
    private SceneManager sceneManager;

    // Start is called before the first frame update
    void Start()
    {
        root = UIDoc.rootVisualElement;

        leftButton = root.Q<Button>("LeftButton");
        rightButton = root.Q<Button>("RightButton");
        endTutorialButton = root.Q<Button>("EndTutorialButton");
        endTutorialButton.style.display = DisplayStyle.None;

        //https://docs.unity3d.com/Manual/UIE-set-background-images-with-an-image-asset.html
        backgroundArray = new StyleBackground[4];
        backgroundArray[0] = new StyleBackground(spriteArr[0]);
        backgroundArray[1] = new StyleBackground(spriteArr[1]);
        backgroundArray[2] = new StyleBackground(spriteArr[2]);
        backgroundArray[3] = new StyleBackground(spriteArr[3]);

        //https://docs.unity3d.com/Manual/UIE-set-background-images-with-an-image-asset.html
        root.style.backgroundImage = backgroundArray[0];

        leftButton.clicked += () => TutorialSlideLeft();
        rightButton.clicked += () => TutorialSlideRight();
        endTutorialButton.clicked += () => EndTutorial();

        //this is here for testing, since setting playerPrefs values does not revert when you stop playing

        //disable the tutorial if 'tutorialDone' is true
        Debug.Log("show tutorial is " + PlayerPrefs.GetInt("ShowTutorial"));
        if (PlayerPrefs.GetInt("ShowTutorial") == 0)
        {
            Debug.Log("disabling " + PlayerPrefs.GetInt("ShowTutorial"));
            //disable this  
            root.style.display = DisplayStyle.None;
        }

        gameManager = GameManager.Instance;
        sceneManager = SceneManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TutorialSlideLeft()
    {
        // Debug.Log("going left");
        if(backGroundArrayIndex > 0)
        {
            backGroundArrayIndex--;
            root.style.backgroundImage = backgroundArray[backGroundArrayIndex];
            // Debug.Log(backGroundArrayIndex);
            endTutorialButton.style.display = DisplayStyle.None;
        }
        else
        {
            SceneManager.Instance.SwitchToScene("MainMenuScene");
        }
    }

    private void TutorialSlideRight()
    {
        // Debug.Log("going right");
        if (backGroundArrayIndex < (backgroundArray.Length - 2))
        {
            backGroundArrayIndex++;
            root.style.backgroundImage = backgroundArray[backGroundArrayIndex];
            // Debug.Log(backGroundArrayIndex);
        }
        else
        {
            backGroundArrayIndex++;
            root.style.backgroundImage = backgroundArray[backgroundArray.Length-1];
            endTutorialButton.style.display = DisplayStyle.Flex;
        }
    }

    private void EndTutorial()
    {
        // Debug.Log("finish tutorial");
        //root.style.display = DisplayStyle.None;
        //set tutorial done to true, creating it if it doesn't exist
        PlayerPrefs.SetInt("ShowTutorial", 0);
        sceneManager.SwitchToScene(sceneManager.gameplaySceneName);
        return;
    }
}
