using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPerspective : MonoBehaviour
{
  public enum Perspective{FirstPerson, FlyingCam}

  Perspective currentPerspective;

  Coroutine perspectiveTransition;

  // hold state of player perspective, and switch between with transition on input



  void SwitchPerspective() {
    // start transition coroutine if none already active
  }

  // IEnumerator PerspectiveTransitionRoutine(Perspective target) {

  // }
}
