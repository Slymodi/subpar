

using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AreaChangeSceneHandler : SimulationBehaviour {
  public NetworkPrefabRef PlayerPrefab;
  private List<CharacterAreaDetection> _characters = new List<CharacterAreaDetection>();

  public void SpawnCharacter(NetworkRunner runner, PlayerRef player) {
    if (Runner.IsServer) {
      var character = runner.Spawn(PlayerPrefab, position: Vector3.up, inputAuthority: player).GetComponent<CharacterAreaDetection>();
      _characters.Add(character);
    }
  }

  public void ToggleScene(int sceneIndex, bool adding) {
    if (adding) {
      if (!Runner.SceneManager().LoadedScenes.Contains(sceneIndex))
        Runner.SceneManager().AddScene(sceneIndex);
    } else {
      bool onePlayerStillInScene = false;

      foreach (var characterAreaDetection in _characters) {
        if (characterAreaDetection.ActiveScenes.Contains(sceneIndex))
          onePlayerStillInScene = true;
      }

      if (Runner.IsClient || !onePlayerStillInScene || !Runner.SceneManager().ClientSceneManagement)
        Runner.SceneManager().RemoveScene(sceneIndex);
    }
  }
}