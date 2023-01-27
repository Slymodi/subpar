using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BallRenderer : MonoBehaviour
{
    [SerializeField] Camera secondPassCamera
    {
        get
        {
            if (_secondPassCamera == null) _secondPassCamera = Resources.FindObjectsOfTypeAll<Camera>().Where(cam => cam.name == "InGameUICamera").FirstOrDefault();
            return _secondPassCamera;
        }
    }
    private Camera _secondPassCamera;
    
    [SerializeField] LayerMask secondPassCameraAim;
    [SerializeField] LayerMask secondPassCameraRoll;
    [SerializeField] GameStateController gameStateController
    {
        get
        {
            if (_gameStateController == null) _gameStateController = FindObjectOfType<GameStateController>();
            return _gameStateController;
        }
    }
    private GameStateController _gameStateController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameStateController.State == GameStateController.GameState.aiming) {
            secondPassCamera.cullingMask = secondPassCameraAim;
        }
        if (gameStateController.State == GameStateController.GameState.rolling) {
            secondPassCamera.cullingMask = secondPassCameraRoll;
        }
    }
}
