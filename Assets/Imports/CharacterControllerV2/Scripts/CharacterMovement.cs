using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
  // Basic movement, looking around, jumping, sprinting

  [SerializeField] CharacterController controller;
  [SerializeField] PlayerPerspective playerPerspective;

  [Header("Movement settings")]
  [SerializeField, Range(100, 1000)] int walkStrength = 400;
  [SerializeField, Range(1, 10)] float walkSpeed = 2;
  [SerializeField, Range(100, 1000)] int sprintStrength = 600;
  [SerializeField, Range(1, 10)] float sprintSpeed = 4;
  [SerializeField, Range(0, 1000)] int counteractStrength = 200;
  [SerializeField, Range(100, 600)] int jumpStrength = 200;

  [Header("turn settings")]
  [SerializeField] float turnSpeed = 1;

  [Header("Grounded check")]
  [SerializeField] LayerMask groundedMask; // probably layer "Default"

  float pivotAngle;
  bool hasJumpInput;
  bool isSprinting;

  bool canMove;

  // movement relative to floor
  Transform currentFloor;
  Vector3 currentFloorVel;
  Vector3 prevFloorPos;

  void OnEnable() {
    controller.input.OnJump += OnJump;
    controller.input.OnSprint += OnSprint;
  }

  void OnDisable() {
    controller.input.OnJump -= OnJump;
    controller.input.OnSprint -= OnSprint;
  }

  public void ActivatePerspective(Vector3 position, Quaternion rotation) {
    controller.gameObject.SetActive(true);
    controller.transform.SetPositionAndRotation(position, rotation);
    
    controller.rb.velocity = Vector3.zero;
    pivotAngle = 0;
    controller.pivot.localRotation = Quaternion.Euler(Vector3.left * pivotAngle);
  }

  public void DeactivatePerspective() {
    controller.gameObject.SetActive(false);
  }

  public void ToggleMovement(bool disableMovement) {
    canMove = !disableMovement;
  }

  void Update() {
    if(!canMove) return;
    
    UpdateRotation();
  }

  void FixedUpdate() {
    UpdateMovement();

    // jump if grounded and trying to jump
    if(canMove && hasJumpInput) {
      hasJumpInput = false;
      if(controller.isGrounded) {
        controller.rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
      }
    }
    
    

    // isGrounded & floor check
    bool isGroundedCheck = Physics.CheckSphere(transform.position + Vector3.up * .4f, .45f, groundedMask);
    controller.isGrounded = isGroundedCheck;

    UpdateCurrentFloor(isGroundedCheck);

    playerPerspective.SetPlayerPositionAndRotation(controller.rb.position, controller.rb.rotation);
  }

  void UpdateRotation() {
    Vector2 turnDir = controller.input.turnInput;
    if(turnDir != Vector2.zero) {
      transform.Rotate(Vector3.up * turnDir.x, Space.Self);

      pivotAngle = Mathf.Clamp(pivotAngle + turnDir.y * turnSpeed, -90, 90);
      controller.pivot.localRotation = Quaternion.Euler(Vector3.left * pivotAngle);
    }
  }

  void UpdateMovement() {
    if(!controller.isGrounded) return;
    // update movement through rigidbody

    Vector3 moveInputDir = controller.input.movementInput.normalized;

    // current velocity relative to what character is standing on
    Vector3 currentVel = controller.rb.velocity - currentFloorVel;
    float currentSpeed = currentVel.magnitude;

    if(canMove && moveInputDir != Vector3.zero) {
      if(isSprinting) {
        if(currentSpeed < sprintSpeed) {
          controller.rb.AddRelativeForce(moveInputDir * sprintStrength);
        }

      } else {
        if(currentSpeed < walkSpeed) {
          controller.rb.AddRelativeForce(moveInputDir * walkStrength);
        }
      }
    }

    // counteract difference in movement input direction & current direction
    if(currentSpeed > .1f) {
      Vector3 currentMoveDir = currentVel.normalized;
      currentMoveDir.y = 0;
      controller.rb.AddForce(((transform.rotation * moveInputDir) - currentMoveDir) * counteractStrength);
    }
  }

  void UpdateCurrentFloor(bool isGrounded) {
    // needs to be called from fixedUpdate

    if(isGrounded) {
      RaycastHit hitInfo;
      if(Physics.Raycast(transform.position + Vector3.up * .1f, Vector3.down, out hitInfo, .15f, groundedMask)) {
        if(currentFloor == hitInfo.collider.transform) {
          currentFloorVel = (hitInfo.collider.transform.position - prevFloorPos) / Time.fixedDeltaTime;
        }
        currentFloor = hitInfo.collider.transform;
        prevFloorPos = hitInfo.collider.transform.position;
        return;
      }
    }

    currentFloor = null;
    currentFloorVel = Vector3.zero;
    prevFloorPos = Vector3.zero;
  }

  void OnJump() {
    hasJumpInput = true;
  }

  void OnSprint(bool state) {
    isSprinting = state;
  }
}
