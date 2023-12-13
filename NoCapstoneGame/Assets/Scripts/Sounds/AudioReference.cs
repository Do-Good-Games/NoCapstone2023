using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioReference", fileName = "audio Reference")]
public class AudioReference : ScriptableObject
{
    [SerializeField] private List<AudioClip> m_ClipArr;
    private int m_Index = 0;

    /*[Tooltip("returns audio clip at current index of the array, doesn't change current index")]
    public AudioClip GetClip() { return GetClip(m_Index, false); }
    [Tooltip("returns audio clip at current index, allows you to determine whether or not to increment index")]
    public AudioClip GetClip(bool incrementIndex) { return GetClip(m_Index, incrementIndex); }
    [Tooltip("returns audio clip at specified index, defaulting to not incrementing the index")]
    public AudioClip GetClip(int index) {  return GetClip(index, false); }*/
    [Tooltip("returns the element at the specified index, -1 returns the currently stored index")]
    public AudioClip GetClip(int index = -1, bool incrementIndex = false)
    {
        if(index == -1)
        {
            index = m_Index;
        }

        if (incrementIndex)
        {
            m_Index = index +1 % m_ClipArr.Count;
            return m_ClipArr[index];
        } else
        {
            return m_ClipArr[m_Index];
        }
    }
    
    public AudioClip GetRandomClip(bool setNewIndex = false)
    {
        int index = Random.Range(0, m_ClipArr.Count);
        if (setNewIndex) { m_Index = index; m_Index++; }
        return m_ClipArr[index];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
