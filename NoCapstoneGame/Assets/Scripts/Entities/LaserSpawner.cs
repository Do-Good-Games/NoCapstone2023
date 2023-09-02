using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    [SerializeField] Projectile laserPrefab;
    [SerializeField] float launchSpeed;

    public void SpawnLaser()
    {
        Projectile laser = GameObject.Instantiate<Projectile>(laserPrefab, this.transform.position, this.transform.rotation);
        laser.Launch(transform.up * launchSpeed);
    }
}
