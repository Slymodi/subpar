using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRenderer : MonoBehaviour
{
    [SerializeField] Camera secondPassCamera;
    [SerializeField] LayerMask secondPassCameraAim;
    [SerializeField] LayerMask secondPassCameraRoll;
    [SerializeField] GameStateController gameStateController;

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
