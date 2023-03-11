using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Launcher : MonoBehaviour {
  public NetworkPrefabRef NetworkSceneManager;
  public SimulationBehaviour ChangeSceneHandler;
  public Text GamemodeText;
  private static NetworkRunner _runner;

  private void Awake() {
    if (_runner != default) {
      Destroy(gameObject);
      return;
    }

    _runner = gameObject.AddComponent<NetworkRunner>();

    _runner.ProvideInput = true;
    _runner.StartGame(new StartGameArgs() {
        SessionName = Menu.SessionName,
        GameMode = GameMode.AutoHostOrClient,
        SceneManager = gameObject.AddComponent<CustomSceneLoader>(),
        Initialized = SpawnNetworkSceneManager
      }
    );

    void SpawnNetworkSceneManager(NetworkRunner runner) {
      runner.Spawn(NetworkSceneManager, onBeforeSpawned: ((networkRunner, o) => o.GetBehaviour<CustomNetworkSceneManager>().ClientSceneManagement = Menu.AllowClientSideManagement));
      runner.AddSimulationBehaviour(ChangeSceneHandler);
      SetupGamemodeText(runner);
    }
  }

  private void SetupGamemodeText(NetworkRunner runner) {
    GamemodeText.gameObject.SetActive(true);
    GamemodeText.text = runner.GameMode.ToString();
  }

  public void BackToMenu() {
    if (_runner)
      _runner.Shutdown();
    SceneManager.LoadScene(0);
  }
}