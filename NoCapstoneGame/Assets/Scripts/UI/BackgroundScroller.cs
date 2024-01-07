using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float speed = .1f;
    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField] private Material m_Material;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        m_Renderer.material = m_Material;
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_Renderer.material.mainTextureOffset += new Vector2(
            0, Time.deltaTime * (speed * gameManager.GetCameraSpeed()));
    }
}
