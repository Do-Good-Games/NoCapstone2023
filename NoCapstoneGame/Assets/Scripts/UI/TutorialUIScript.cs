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

        backgroundArray = new StyleBackground[4];
        backgroundArray[0] = new StyleBackground(UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Asteroid-Fiona.bmp"));

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
        Debug.Log("sliding left");
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
        Debug.Log("sliding right");
        if (backGroundArrayValue < (backgroundArray.Length - 1))
        {
            backGroundArrayValue++;
            root.style.backgroundImage = backgroundArray[backGroundArrayValue];
            Debug.Log(backGroundArrayValue);
        }
        else
        {
            //the tutorial is done
            return;
        }
    }

}
