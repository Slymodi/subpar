using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class StarController : NetworkBehaviour
{
     [SerializeField] private Game _game;
    
    void Awake()
    {
        _game = GameObject.Find("Game").GetComponent<Game>();
    }
    [Networked] public int _playerRef { get; set;}
    void OnTriggerStay(Collider other)
    {

        if (other.TryGetComponent(out PlayerInputConsumer playerIC))
        {

            if (playerIC.playerRef == _playerRef) return;
            _playerRef = playerIC.playerRef;
            //switch stars
            _game.SwapScore(this, playerIC.playerRef);
        // spawn particle ParticleSpawner.Instance.SpawnHitParticle(particle, transform.position, transform.rotation);
            ChangeColor(playerIC.color);
        }
    }

    public void ChangeColor(Color color)
    {
        transform.GetComponentInChildren<Renderer>().material.color = color;
    }
    
}
