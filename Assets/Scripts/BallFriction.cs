using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallFriction : MonoBehaviour
{
    [SerializeField] float constantDeceleration = 1f;
    [SerializeField] float proportionalDeceleration = 1f;
    Rigidbody rigidBody;

    void Start() {
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float maxDeceleration = constantDeceleration + proportionalDeceleration * rigidBody.velocity.magnitude;        
        float deceleration = Mathf.Max(maxDeceleration, rigidBody.velocity.magnitude);
        rigidBody.velocity -= deceleration * rigidBody.velocity.normalized * Time.fixedDeltaTime;
    }
}
