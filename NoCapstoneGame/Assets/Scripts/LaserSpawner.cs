using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    [SerializeField] GameObject laserPrefab;

    public void SpawnLaser()
    {
        GameObject.Instantiate(laserPrefab, this.transform.position, this.transform.rotation);
    }
}
