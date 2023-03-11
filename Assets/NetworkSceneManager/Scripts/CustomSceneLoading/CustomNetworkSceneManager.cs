using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkSceneManager : NetworkBehaviour {

  private const int MAXSCENES = 4;
  [Networked] public NetworkBool ClientSceneManagement { get; set; }
  [Networked, Capacity(MAXSCENES)] public NetworkLinkedList<SceneRef> CurrentScenes => default;
  [Networked(OnChanged = nameof(OnReloadChanged))]
  public byte ReloadByte { get; set; }
  public List<SceneRef> InterestedInScenes = new List<SceneRef>(MAXSCENES);
  public List<SceneRef> LoadedScenes = new List<SceneRef>(MAXSCENES);
  public Action OnUpToDate;

  private Stack<SceneRef> _scenesToReload = new Stack<SceneRef>();

  private SceneRef _sceneToUnload;


  public override void Spawned() {
    if (Runner.SceneManager()) {
      Runner.Despawn(Object);
      return;
    }
    DontDestroyOnLoad(this);
    Runner.SetSceneManager(this);
  }

  public void AddScene(int sceneIndex) {
    if ((Object.HasStateAuthority) && (CurrentScenes.Count >= CurrentScenes.Capacity || CurrentScenes.Contains(sceneIndex) || sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)) {
      Debug.LogError("Scene not added");
      return;
    }

    if (Object.HasStateAuthority)
      CurrentScenes.Add(sceneIndex);

    if (!InterestedInScenes.Contains(sceneIndex))
      InterestedInScenes.Add(sceneIndex);
  }

  public void RemoveScene(int sceneIndex) {
    if (Object.HasStateAuthority)
      CurrentScenes.Remove(sceneIndex);

    InterestedInScenes.Remove(sceneIndex);
  }

  private void ReloadCurrentScenes() {
    for (int i = 0; i < LoadedScenes.Count; i++) {
        _scenesToReload.Push(LoadedScenes[i]);
    }
  }

  public void UnloadOutdatedScenes() {
    _sceneToUnload = LoadedScenes.Except(ClientSceneManagement ? InterestedInScenes.Intersect(CurrentScenes) : CurrentScenes).FirstOrDefault();

    //Reload scenes is priority
    if (_scenesToReload.Count > 0) {
      _sceneToUnload = _scenesToReload.Pop();

      if (_scenesToReload.Count == 0)
        OnUpToDate += TriggerReloadByte;
    }

    if (_sceneToUnload) {
      Debug.Log("Unloading scene - " + _sceneToUnload);
      SceneManager.UnloadSceneAsync(_sceneToUnload);
      LoadedScenes.Remove(_sceneToUnload);
    }
  }

  public bool IsSceneUpdated(out SceneRef sceneRef) {
    for (int i = 0; i < CurrentScenes.Count; i++) {
      if (CurrentScenes[i] == default) continue;
      if (LoadedScenes.Contains(CurrentScenes[i])) continue;
      if (ClientSceneManagement == false || InterestedInScenes.Contains(CurrentScenes[i])) {
        sceneRef = CurrentScenes[i];
        LoadedScenes.Add(CurrentScenes[i]);
        return false;
      }
    }
    sceneRef = SceneRef.None;
    return true;
  }

  public void StartReloadScenes() {
    ReloadCurrentScenes();
  }

  public void TriggerReloadByte() {
    ReloadByte = (byte)((ReloadByte + 1) % 2);
    OnUpToDate -= TriggerReloadByte;
  }

  public static void OnReloadChanged(Changed<CustomNetworkSceneManager> changed) {
    if (changed.Behaviour.Runner.IsClient)
      changed.Behaviour.ReloadCurrentScenes();
  }
}