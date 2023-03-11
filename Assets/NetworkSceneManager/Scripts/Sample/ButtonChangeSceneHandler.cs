using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class ButtonChangeSceneHandler : SimulationBehaviour {
  public Button[] Buttons = new Button[4];
  public GameObject ReloadButton;

  public void ToggleScene(int sceneIndex) {
    if (!Runner.SceneManager().LoadedScenes.Contains(sceneIndex))
      Runner.SceneManager().AddScene(sceneIndex);
    else
      Runner.SceneManager().RemoveScene(sceneIndex);
  }

  public void ReloadScenesButton() {
    if (Runner.SceneManager() && Runner.IsServer) 
      Runner.SceneManager().StartReloadScenes();
  }

  private void FixedUpdate() {
    if (!Runner.SceneManager()) return;
    for (int i = 0; i < Buttons.Length; i++) {
      Buttons[i].interactable = Runner.IsRunning;
      if (Runner.SceneManager().LoadedScenes.Contains(i + 1))
        Buttons[i].image.color = Color.red;
      else
        Buttons[i].image.color = Color.white;
    }
  }
}