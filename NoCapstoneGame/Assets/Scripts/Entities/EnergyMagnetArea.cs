using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMagnetArea : MonoBehaviour
{
    [SerializeField] float pullStrength;

    public float getStrength()
    {
        return pullStrength;
    }
}
