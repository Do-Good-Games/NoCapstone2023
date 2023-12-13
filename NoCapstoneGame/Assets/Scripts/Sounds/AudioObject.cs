using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    

[CreateAssetMenu(menuName = "Audio/AudioObject" , fileName = "AudioObject")]
public class AudioObject : ScriptableObject
{

    [SerializeField] public AudioSource m_Source;
    [SerializeField] public AudioClip m_Clip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
