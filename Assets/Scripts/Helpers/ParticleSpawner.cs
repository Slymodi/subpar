using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : Singleton<ParticleSpawner>
{

    public void SpawnHitParticle(GameObject particle, Vector3 position, Quaternion rotation)
    {
        Instantiate(particle, position, rotation);
    }
}
