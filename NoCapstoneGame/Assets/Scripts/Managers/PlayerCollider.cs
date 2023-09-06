using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    

    private GameManager gameManager;

    public void Start()
    {
        gameManager = GameManager.Instance;
    }
   
}
