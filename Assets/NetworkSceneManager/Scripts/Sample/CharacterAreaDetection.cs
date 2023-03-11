using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class CharacterAreaDetection : SimulationBehaviour {
  public LayerMask SceneAreaLayer;
  public List<int> ActiveScenes = new List<int>();
  private Collider[] _collider = new Collider[1];
  private Collider _lastSceneArea;
  private int _checkSchedule = 10;

  public override void FixedUpdateNetwork() {
    if (Object.HasStateAuthority || Object.HasInputAuthority && Runner.Simulation.Tick % _checkSchedule == Object.InputAuthority % _checkSchedule) {
      if (Physics.OverlapSphereNonAlloc(transform.position, 1, _collider, SceneAreaLayer) > 0) {
        if (_lastSceneArea == _collider[0])
          return;

        _lastSceneArea = _collider[0];
        _collider[0].GetComponent<SceneArea>().RegisterScene(this);
      } else {
        if (_lastSceneArea != default)
          _collider[0].GetComponent<SceneArea>().UnregisterScene(this);
        _lastSceneArea = default;
      }
    }
  }
}