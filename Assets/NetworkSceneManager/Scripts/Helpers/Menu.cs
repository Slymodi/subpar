using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
  public static string SessionName;
  public static bool AllowClientSideManagement;

  public void SetSessionName(string name) {
    SessionName = name;
  }

  public void SetAllowClientSideManagement(bool value) {
    AllowClientSideManagement = value;
  }

  public void LoadButtonLauncherScene() {
    SceneManager.LoadScene(5);
  }

  public void LoadCharacterLauncherScene() {
    SceneManager.LoadScene(6);
  }
}