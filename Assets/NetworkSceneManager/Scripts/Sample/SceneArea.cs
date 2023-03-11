using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SceneArea : MonoBehaviour {
  public AreaChangeSceneHandler ChangeHandler;
  public int SceneIndex;

  public void RegisterScene(CharacterAreaDetection character) {
    if (!character.ActiveScenes.Contains(SceneIndex))
      character.ActiveScenes.Add(SceneIndex);

    ChangeHandler.ToggleScene(SceneIndex, true);
  }

  public void UnregisterScene(CharacterAreaDetection character) {
    character.ActiveScenes.Remove(SceneIndex);
    ChangeHandler.ToggleScene(SceneIndex, false);
  }
}