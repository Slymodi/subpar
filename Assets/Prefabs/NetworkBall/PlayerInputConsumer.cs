using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class PlayerInputConsumer : NetworkBehaviour
{


    [Networked] public NetworkButtons ButtonsPrevious { get; set; }
    [SerializeField] private Rigidbody ThisRigidBody;   
    [SerializeField] public PlayerRef playerRef { get; set; }

    //BALL FRICTION
    [SerializeField] float constantDeceleration = 1f;
    [SerializeField] float proportionalDeceleration = 1f;
    public float power = 0f;
    public float steeringAngle = 0f;

    [SerializeField] public Transform steeringPlaneForward;
    [SerializeField] public Transform steeringPlaneRear;

    [SerializeField]Projection projection;

    [SerializeField] private GameObject hitParticle;

    [Networked] public Color color { get; set; }


void Spawned(){
  projection = GetComponentInChildren<Projection>();

}
public override void FixedUpdateNetwork() {

  
    if (GetInput<NetworkInputData>(out var input) == false) return;

    // BALL FRICTION
        float maxDeceleration = constantDeceleration 
            + proportionalDeceleration 
            * ThisRigidBody.velocity.magnitude;        
        float deceleration = Mathf.Max(maxDeceleration, ThisRigidBody.velocity.magnitude);
        //ThisRigidBody.velocity -= deceleration * ThisRigidBody.velocity.normalized * Runner.DeltaTime;
    // compute pressed/released state
    var pressed = input.NetworkButtons.GetPressed(ButtonsPrevious);
    var released = input.NetworkButtons.GetReleased(ButtonsPrevious);

    // store latest input as 'previous' state we had
    ButtonsPrevious = input.NetworkButtons;


    //if (input.Buttons.IsSet(MyButtons.Forward)) { vector.z += 1; }

    if(input.NetworkButtons.IsSet(MyButtons.Projection)) {
      projection.SimulateTrajectory(input.power, input.angle);
    }

    //DoMove(input.direction);

    // jump (check for pressed)
    if (pressed.IsSet(MyButtons.Shoot)  && input.power > 0.1f) {
      DoJump(input.power, input.angle);
    }

    
  }

  void DoMove(Vector3 vector) {
    // dummy method with no logic in it
  }

  void DoJump(float power, float angle) {
    ThisRigidBody.AddForce( Quaternion.AngleAxis( angle + 90, Vector3.up)
      * new Vector3(1, 1, 0) * power * 500);

      ParticleSpawner.Instance.SpawnHitParticle(hitParticle, transform.position, Quaternion.identity);
  }


}
