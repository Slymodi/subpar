using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class StarSpawnerController : MonoBehaviour
{

    [SerializeField] private GameObject star;
    [SerializeField] private GameObject starSpawner;
    public GameObject particle;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;

            
        star.SetActive(true);
        starSpawner.SetActive(false);
        ParticleSpawner.Instance.SpawnHitParticle(particle, transform.position, transform.rotation);
    }

}
