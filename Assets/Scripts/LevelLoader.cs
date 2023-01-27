using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
 
// use unity to load prefab as child of this object
 public void LoadLevel(string levelName)
 {
  GameObject level = Instantiate(Resources.Load(levelName)) as GameObject;
  level.SetActive(false);
  level.transform.parent = transform;
 }
    public void ShowLevel() {

        // show all children of this object
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void HideLevel() {
        // hide all children of this object
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void UnloadLevel()
    {
        //clear children of this object
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

}
