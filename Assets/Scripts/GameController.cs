using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameStateController))]
public class GameController : MonoBehaviour
{
    [SerializeField] Ball ball;
    [SerializeField] float minBallY = -1f;
    [SerializeField] float maxStillSpeed = 0.1f;
    [SerializeField] float minStillTime = 0.5f;
    float currentStillTime = 0f;

    GameStateController gameStateController;

    void Awake()
    {
        gameStateController = GetComponent<GameStateController>();        

        gameStateController.onAimingStart.AddListener(OnAimStart);
        gameStateController.onRollingStart.AddListener(OnRollingStart);
    }

    void FixedUpdate() {
        if (ball.transform.position.y < minBallY) Restart();

        if (gameStateController.State == GameStateController.GameState.rolling) {

            if (ball.GetSpeed() < maxStillSpeed) {
                currentStillTime += Time.deltaTime;
            } else {
                currentStillTime = 0;
            }

            if (currentStillTime > minStillTime) {
                currentStillTime = 0;
                gameStateController.State = GameStateController.GameState.aiming;
            }
        }
    }

    void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnAimStart() {
        // inputController.enabled = true;
        ball.SetMoveable(false);
    }

    void OnRollingStart() {
        // inputController.enabled = false;
        ball.SetMoveable(true);
    }
}
