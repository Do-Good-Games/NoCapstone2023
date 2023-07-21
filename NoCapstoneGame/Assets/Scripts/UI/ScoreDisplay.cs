using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] public TMPro.TextMeshProUGUI textMeshPro;
    [SerializeField] public string format;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        textMeshPro.text = gameManager.getScore().ToString(format);
    }
}
