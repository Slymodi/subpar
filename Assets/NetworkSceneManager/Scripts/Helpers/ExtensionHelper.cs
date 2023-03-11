using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

public static class ExtensionHelper {

  private static CustomNetworkSceneManager _sceneManager;

  public static CustomNetworkSceneManager SceneManager(this NetworkRunner runner) => _sceneManager;
  public static void SetSceneManager(this NetworkRunner runner, CustomNetworkSceneManager manager) => _sceneManager = manager;

  public static TaskAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation op) {
    TaskCompletionSource<AsyncOperation> task = new TaskCompletionSource<AsyncOperation>();
    op.completed += (operation) => task.SetResult(op);
    return task.Task.GetAwaiter();
  }
}