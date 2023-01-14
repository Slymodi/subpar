using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameStateController : MonoBehaviour
{
    public enum GameState
    {
        aiming,
        rolling,
    }

    [ReadOnly] [SerializeField] private GameState state = GameState.aiming;
    [SerializeField] bool verbose = false;

    public GameState State {
        get => state;
        set {
            if (state != value) {
                state = value;
                OnStateChange();
            }
        }
    }

    public UnityEvent onAimingStart;
    public UnityEvent onRollingStart;

    void Start()
    {
        OnStateChange();
    }

    void OnStateChange()
    {
        if (verbose) Debug.Log("New game state: " + State);
        switch (State)
        {
            case GameState.aiming:
                onAimingStart.Invoke();
                break;
            case GameState.rolling:
                onRollingStart.Invoke();
                break;
        }
    }
}
