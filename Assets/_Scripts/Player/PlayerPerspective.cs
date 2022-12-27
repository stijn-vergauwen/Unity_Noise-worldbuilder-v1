using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerPerspective : MonoBehaviour
{
  // hold state of player perspective (and player position), and switch between with transition on input

  public enum Perspective{FirstPerson, FlyingCam}

  [SerializeField] CharacterMovement firstPersonComponent;
  [SerializeField] FlyingCam flyingCamComponent;
  [SerializeField] Perspective startPerspective;
  [SerializeField] float transitionDuration = 1;

  [Header("Channels")]
  [SerializeField] ScreenFadeEventChannelSO screenFade;

  public Perspective currentPerspective;
  bool isInPerspectiveTransition;

  Vector3 playerPosition;
  Quaternion playerRotation;

  // events
  public Action<Vector3> OnPositionUpdate;

  // player perspective

  public void StartPlayer(Vector3 startPosition) {
    SetPlayerPositionAndRotation(startPosition, Quaternion.identity);
    SetPerspective(startPerspective);
  }

  void Update() {
    if(Input.GetKeyDown(KeyCode.T)) {
      SwitchPerspective();
    }
  }
  
  void SwitchPerspective() {
    Perspective newPerspective = currentPerspective == Perspective.FirstPerson ? Perspective.FlyingCam : Perspective.FirstPerson;

    if(!isInPerspectiveTransition) {
      StartCoroutine(PerspectiveTransitionRoutine(newPerspective));
    }
  }

  void SetPerspective(Perspective newPerspective) {
    if(newPerspective == Perspective.FirstPerson) {
      flyingCamComponent.DeactivatePerspective();
      firstPersonComponent.ActivatePerspective(playerPosition, playerRotation);

    } else if(newPerspective == Perspective.FlyingCam) {
      firstPersonComponent.DeactivatePerspective();
      flyingCamComponent.ActivatePerspective(playerPosition, playerRotation);
    }

    currentPerspective = newPerspective;
  }

  IEnumerator PerspectiveTransitionRoutine(Perspective newPerspective) {
    isInPerspectiveTransition = true;
    float halfDuration = transitionDuration * .5f;

    screenFade.RaiseEvent(true, halfDuration);
    yield return new WaitForSeconds(halfDuration);

    SetPerspective(newPerspective);

    screenFade.RaiseEvent(false, halfDuration);
    isInPerspectiveTransition = false;
  }







  // player position

  public void SetPlayerPositionAndRotation(Vector3 newPosition, Quaternion newRotation) {
    playerPosition = newPosition;
    playerRotation = newRotation;
    OnPositionUpdate?.Invoke(playerPosition);
  }

  public Vector3 GetPlayerPosition() {
    return playerPosition;
  }
}
