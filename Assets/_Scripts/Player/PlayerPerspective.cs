using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerspective : MonoBehaviour
{
  public enum Perspective{FirstPerson, FlyingCam}

  Perspective currentPerspective;

  Vector3 playerPosition;

  Coroutine perspectiveTransition;

  // hold state of player perspective, and switch between with transition on input

  public void SetPlayerPosition(Vector3 newPosition) {
    playerPosition = newPosition;
  }

  public Vector3 GetPlayerPosition() {
    return playerPosition;
  }

  


  void SwitchPerspective() {
    // start transition coroutine if none already active
  }


  // IEnumerator PerspectiveTransitionRoutine(Perspective target) {

  // }
}
