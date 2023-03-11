using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NORotationHandler : NetworkBehaviour {
  public Transform GFX;
  [Networked] private Angle CurrentAngle { get; set; }

  public override void FixedUpdateNetwork() {
    CurrentAngle += 1;
    GFX.rotation = Quaternion.Euler(45, (float)CurrentAngle, 45);
  }
}