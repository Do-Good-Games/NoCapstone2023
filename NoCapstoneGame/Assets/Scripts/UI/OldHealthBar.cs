using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldHealthBar : MonoBehaviour
{
    [SerializeField] PlayerController player;

    private float initialHeight;
    private int maxHealth;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        initialHeight = transform.localScale.y;
        maxHealth = player.maxHealth;
        gameManager = GameManager.Instance;
        gameManager.OnPlayerHurt.AddListener(UpdateHealthBar);
    }

    // Update is called once per frame
    void UpdateHealthBar()
    {
        float segmentHeight = initialHeight / maxHealth;
        float totalHeight = segmentHeight * gameManager.GetPlayerHealth();
        transform.localScale = new Vector3(transform.localScale.x, totalHeight, transform.localScale.z);
    }
}
