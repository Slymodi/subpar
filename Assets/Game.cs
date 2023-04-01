using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;
public class Game : NetworkBehaviour
{

    [Networked][Capacity(32)]
    
    private NetworkDictionary<StarController, PlayerRef> _playerScores => default;
    [Networked][Capacity(32)] 
    NetworkDictionary<PlayerRef, int> _playerScoresList => default;
    [SerializeField] private Transform PhysicsInteractable;
    void Awake()
    {

    }

    public void AddScore(StarSpawnerController starSpawner, StarController star, PlayerRef player)
    {
        
        if (!Object.HasStateAuthority) return;

        _playerScores.Add(star, player);
      


        List<StarSpawnerController> starSpawners = FindObjectsOfType<StarSpawnerController>().ToList();
        if (starSpawners.Count == 2)
        {
            if (starSpawner == starSpawners[0])
                starSpawners[1].BecomeHole(PhysicsInteractable);
            else
                starSpawners[0].BecomeHole(PhysicsInteractable); 
        }

        if (!_playerScoresList.ContainsKey(player))
        {
            _playerScoresList.Set(player, 1);
            return;
        } 
        _playerScoresList.Set(player, _playerScoresList.Get(player) + 1);
    }
public void SwapScore(StarController star, PlayerRef to)
    {    
        if (!Object.HasStateAuthority) return;
        PlayerRef from = _playerScores.Get(star);
        _playerScores.Set(star, to);
        _playerScoresList.Set(from, _playerScoresList.Get(from) - 1);

        if (!_playerScoresList.ContainsKey(to))
        {
            _playerScoresList.Set(to, 1);
            return;
        } 
        _playerScoresList.Set(to, _playerScoresList.Get(to) + 1);

    }


    public void StartGame()
    {
        if (!Object.HasStateAuthority) return;
        //start game

        
    }
    public void EndGame()
    {
        if (!Object.HasStateAuthority) return;
        //end game
    }


}
