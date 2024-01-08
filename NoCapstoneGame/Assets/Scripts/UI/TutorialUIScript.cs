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
    private VisualElement background;

    private StyleBackground[] backgroundArray;
    private int backGroundArrayValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        root = UIDoc.rootVisualElement;

        leftButton = root.Q<Button>("LeftButton");
        rightButton = root.Q<Button>("RightButton");

        //https://docs.unity3d.com/Manual/UIE-set-background-images-with-an-image-asset.html
        backgroundArray = new StyleBackground[3];
        backgroundArray[0] = new StyleBackground(AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Art/TutorialArt/Slide1.png"));
        backgroundArray[1] = new StyleBackground(AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Art/TutorialArt/Slide2.png"));
        backgroundArray[2] = new StyleBackground(AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Art/TutorialArt/Slide3.png"));

        root.style.backgroundImage = backgroundArray[0];

        leftButton.clicked += () => TutorialSlideLeft();
        rightButton.clicked += () => TutorialSlideRight();

        //this is here for testing, since setting playerPrefs values does not revert when you stop playing
        PlayerPrefs.SetInt("tutorialDone", 0);

        //disable the tutorial if 'tutorialDone' is true
        Debug.Log("tutorial done is " + PlayerPrefs.GetInt("tutorialDone"));
        if (PlayerPrefs.GetInt("tutorialDone") == 1)
        {
            Debug.Log("disabling " + PlayerPrefs.GetInt("tutorialDone"));
            //disable this
            root.style.display = DisplayStyle.None;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TutorialSlideLeft()
    {
        if(backGroundArrayValue > 0)
        {
            backGroundArrayValue--;
            root.style.backgroundImage = backgroundArray[backGroundArrayValue];
            Debug.Log(backGroundArrayValue);
        }
        else
        {
            return;
        }
    }

    private void TutorialSlideRight()
    {
        if (backGroundArrayValue < (backgroundArray.Length - 1))
        {
            backGroundArrayValue++;
            root.style.backgroundImage = backgroundArray[backGroundArrayValue];
            Debug.Log(backGroundArrayValue);
        }
        else
        {
            Debug.Log("finish tutorial");
            root.style.display = DisplayStyle.None;
            //set tutorial done to true, creating it if it doesn't exist
            PlayerPrefs.SetInt("tutorialDone", 1);
            return;
        }
    }
}
