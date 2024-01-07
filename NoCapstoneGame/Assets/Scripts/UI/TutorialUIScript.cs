using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialUIScript : MonoBehaviour
{

    [SerializeField] UIDocument UIDoc;
    [SerializeField] VisualElement background0;
    [SerializeField] VisualElement background1;
    [SerializeField] VisualElement background2;
    [SerializeField] VisualElement background3;

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

        backgroundArray = new StyleBackground[3];
        backgroundArray[0] = new StyleBackground(UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Art/TutorialArt/Slide1.png"));
        backgroundArray[1] = new StyleBackground(UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Art/TutorialArt/Slide2.png"));
        backgroundArray[2] = new StyleBackground(UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI/Art/TutorialArt/Slide3.png"));

    https://docs.unity3d.com/Manual/UIE-set-background-images-with-an-image-asset.html
        root.style.backgroundImage = backgroundArray[0];

        leftButton.clicked += () => TutorialSlideLeft();
        rightButton.clicked += () => TutorialSlideRight();

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
            return;
        }
    }
}
