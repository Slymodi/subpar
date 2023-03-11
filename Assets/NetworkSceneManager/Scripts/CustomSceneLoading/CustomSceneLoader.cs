using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneLoader : CustomSceneLoaderBase {
  private SceneRef _desiredScene;

  protected override bool IsScenesUpdated() {
    if (Runner.SceneManager() && Runner.SceneManager().Object) {

      Runner.SceneManager().UnloadOutdatedScenes();

      return Runner.SceneManager().IsSceneUpdated(out _desiredScene);
    }

    return true;
  }
  protected override SceneRef GetDesiredSceneToLoad() {
    return _desiredScene;
  }
  protected override async Task<IEnumerable<NetworkObject>> SwitchScene(SceneRef prevScene, SceneRef newScene) {
    prevScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1).buildIndex;
    Debug.Log($"Switching Scene from {prevScene} to {newScene}");

    List<NetworkObject> sceneObjects = new List<NetworkObject>();
    if (newScene >= 0) {
      var loadedScene = await LoadSceneAsset(newScene, LoadSceneMode.Additive);
      Debug.Log($"Loaded scene {newScene}: {loadedScene}");
      sceneObjects = FindNetworkObjects(loadedScene, disable: false);
    }

    Debug.Log($"Switched Scene from {prevScene} to {newScene} - loaded {sceneObjects.Count} scene objects");
    return sceneObjects;
  }

  private async Task<Scene> LoadSceneAsset(int sceneIndex, LoadSceneMode mode) {
    var scene = new Scene();
    var op = await SceneManager.LoadSceneAsync(sceneIndex, mode);
    op.completed += (operation) => scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
    return scene;
  }

}