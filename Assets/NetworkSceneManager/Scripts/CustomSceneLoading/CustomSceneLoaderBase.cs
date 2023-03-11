using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

public abstract class CustomSceneLoaderBase : Fusion.Behaviour, INetworkSceneManager {
  private static WeakReference<CustomSceneLoaderBase> s_currentlyLoading = new WeakReference<CustomSceneLoaderBase>(null);

  public bool ShowHierarchyWindowOverlay = true;


  private Task _switchSceneTask;
  private bool _currentSceneOutdated;
  protected SceneRef _currentScene;

  public NetworkRunner Runner { get; private set; }

  protected virtual void OnEnable() {
#if UNITY_EDITOR
    if (ShowHierarchyWindowOverlay) {
      UnityEditor.EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowOverlay;
    }
#endif
  }

  protected virtual void OnDisable() {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.hierarchyWindowItemOnGUI -= HierarchyWindowOverlay;
#endif
  }

  protected virtual void LateUpdate() {
    if (!Runner)
      return;

    if (_switchSceneTask != null && _switchSceneTask.Status != TaskStatus.Running)
      //busy
      return;

    _currentSceneOutdated = !IsScenesUpdated();

    if (!_currentSceneOutdated) {
      //up to date

      Runner.SceneManager()?.OnUpToDate?.Invoke();
      return;
    }

    if (s_currentlyLoading.TryGetTarget(out var target)) {
      Assert.Check(target != this);
      if (!target) {
        // orphaned loader?
        s_currentlyLoading.SetTarget(null);
      } else {
        LogTrace($"Waiting for {target} to finish loading");
        return;
      }
    }

    var prevScene = _currentScene;
    _currentScene = GetDesiredSceneToLoad();
    _currentSceneOutdated = false;

    LogTrace($"Scene transition {prevScene}->{_currentScene}");
    _switchSceneTask = SwitchSceneWrapper(prevScene, _currentScene);
  }

  protected static bool IsScenePathOrNameEqual(Scene scene, string nameOrPath) {
    return scene.path == nameOrPath || scene.name == nameOrPath;
  }

  public static bool TryGetScenePathFromBuildSettings(SceneRef sceneRef, out string path) {
    if (sceneRef.IsValid) {
      path = SceneUtility.GetScenePathByBuildIndex(sceneRef);
      if (!string.IsNullOrEmpty(path)) {
        return true;
      }
    }
    path = string.Empty;
    return false;
  }

  public bool IsScenePathOrNameEqual(Scene scene, SceneRef sceneRef) {
    if (TryGetScenePathFromBuildSettings(sceneRef, out var path)) {
      return IsScenePathOrNameEqual(scene, path);
    } else {
      return false;
    }
  }

  public List<NetworkObject> FindNetworkObjects(Scene scene, bool disable = true, bool addVisibilityNodes = false) {

    var networkObjects = new List<NetworkObject>();
    var gameObjects = scene.GetRootGameObjects();
    var result = new List<NetworkObject>();

    // get all root gameobjects and move them to this runners scene
    foreach (var go in gameObjects) {
      networkObjects.Clear();
      go.GetComponentsInChildren(true, networkObjects);

      foreach (var sceneObject in networkObjects) {
        if (sceneObject.Flags.IsSceneObject()) {
          if (sceneObject.gameObject.activeInHierarchy || sceneObject.Flags.IsActivatedByUser()) {
            Assert.Check(sceneObject.NetworkGuid.IsValid);
            result.Add(sceneObject);
          }
        }
      }

      if (addVisibilityNodes) {
        // register all render related components on this gameobject with the runner, for use with IsVisible
        RunnerVisibilityNode.AddVisibilityNodes(go, Runner);
      }
    }

    if (disable) {
      // disable objects; each will be activated if there's a matching state object
      foreach (var sceneObject in result) {
        sceneObject.gameObject.SetActive(false);
      }
    }

    return result;
  }

  protected abstract bool IsScenesUpdated();
  protected abstract SceneRef GetDesiredSceneToLoad();

    #region INetworkSceneObjectProvider

  void INetworkSceneManager.Initialize(NetworkRunner runner) {
    Initialize(runner);
  }

  void INetworkSceneManager.Shutdown(NetworkRunner runner) {
    Shutdown(runner);
  }

  bool INetworkSceneManager.IsReady(NetworkRunner runner) {
    Assert.Check(Runner == runner);
    if (_switchSceneTask != null && _switchSceneTask.Status == TaskStatus.Running || !Runner) {
      return false;
    }
    if (_currentSceneOutdated) {
      return false;
    }

    return true;
  }

    #endregion

  protected virtual void Initialize(NetworkRunner runner) {
    Assert.Check(!Runner);
    Runner = runner;
  }

  protected virtual void Shutdown(NetworkRunner runner) {
    Assert.Check(Runner == runner);

    try {
      // ongoing loading, dispose
      if (_switchSceneTask != null && _switchSceneTask.Status == TaskStatus.Running) {
        LogWarn($"There is an ongoing scene load ({_currentScene}), stopping and disposing coroutine.");
        (_switchSceneTask as IDisposable)?.Dispose();
      }
    } finally {
      Runner = null;
      _switchSceneTask = null;
      _currentScene = SceneRef.None;
      _currentSceneOutdated = false;
    }
  }

  protected abstract Task<IEnumerable<NetworkObject>> SwitchScene(SceneRef prevScene, SceneRef newScene);

  [System.Diagnostics.Conditional("FUSION_NETWORK_SCENE_MANAGER_TRACE")]
  protected void LogTrace(string msg) {
    Log.Debug($"[NetworkSceneManager] {(this != null ? this.name : "<destroyed>")}: {msg}");
  }

  protected void LogError(string msg) {
    Log.Error($"[NetworkSceneManager] {(this != null ? this.name : "<destroyed>")}: {msg}");
  }

  protected void LogWarn(string msg) {
    Log.Warn($"[NetworkSceneManager] {(this != null ? this.name : "<destroyed>")}: {msg}");
  }

  private async Task SwitchSceneWrapper(SceneRef prevScene, SceneRef newScene) {
    IEnumerable<NetworkObject> sceneObjects;
    Exception error = null;

    try {
      Assert.Check(!s_currentlyLoading.TryGetTarget(out _));
      s_currentlyLoading.SetTarget(this);
      Runner.InvokeSceneLoadStart();
      sceneObjects = await SwitchScene(prevScene, newScene);
    } catch (Exception ex) {
      sceneObjects = null;
      error = ex;
    } finally {
      Assert.Check(s_currentlyLoading.TryGetTarget(out var target) && target == this);
      s_currentlyLoading.SetTarget(null);
      _switchSceneTask = null;
      LogTrace($"Coroutine finished for {newScene}");
    }

    if (error != null) {
      LogError($"Failed to switch scenes: {error}");
    } else {
      Runner.RegisterSceneObjects(sceneObjects);
      Runner.InvokeSceneLoadDone();
    }
  }

#if UNITY_EDITOR
  private static Lazy<GUIStyle> s_hierarchyOverlayLabelStyle = new Lazy<GUIStyle>(() => {
    var result = new GUIStyle(UnityEditor.EditorStyles.miniButton);
    result.alignment = TextAnchor.MiddleCenter;
    result.fontSize = 9;
    result.padding = new RectOffset(4, 4, 0, 0);
    result.fixedHeight = 13f;
    return result;
  });

  private void HierarchyWindowOverlay(int instanceId, Rect position) {
    if (!Runner) {
      return;
    }

    if (!Runner.MultiplePeerUnityScene.IsValid()) {
      return;
    }

    if (Runner.MultiplePeerUnityScene.GetHashCode() == instanceId) {

      var rect = new Rect(position) {
        xMin = position.xMax - 56, xMax = position.xMax - 2, yMin = position.yMin + 1,
      };

      if (GUI.Button(rect, $"{Runner.Mode} {(Runner.LocalPlayer.IsValid ? "P" + Runner.LocalPlayer.PlayerId.ToString() : "")}", s_hierarchyOverlayLabelStyle.Value)) {
        UnityEditor.EditorGUIUtility.PingObject(Runner);
        UnityEditor.Selection.activeGameObject = Runner.gameObject;
      }
    }
  }

#endif
}