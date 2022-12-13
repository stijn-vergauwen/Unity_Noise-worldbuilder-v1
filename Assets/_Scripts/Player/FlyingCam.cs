using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCam : MonoBehaviour
{
  [SerializeField] CharacterInput input;
  [SerializeField] PlayerPerspective playerPerspective;

  [SerializeField] Transform anchor;
  [SerializeField] Transform verticalPivot;
  [SerializeField] Camera cam;

  [Header("Movement")]
  [SerializeField] MinMax moveSpeed;
  [SerializeField] float turnSpeed;
  [SerializeField] float verticalStartAngle;
  [SerializeField] float zoomSpeed;
  [SerializeField] MinMax zoomLimit;
  [SerializeField] float defaultHeight;

  [Header("Smoothness")]
  [SerializeField] float moveSmoothness;
  [SerializeField] float zoomSmoothness;

  [Header("Other")]
  [SerializeField] LayerMask terrainMask;

  [Header("Player settings")]
  [SerializeField] bool invertRotation;

  Vector3 targetPosition;
  Vector3 moveVelocity;

  float targetZoom;
  float zoomVelocity;

  public void ActivatePerspective(Vector3 position, Quaternion rotation) {
    anchor.gameObject.SetActive(true);
    anchor.transform.SetPositionAndRotation(
      CalculateHeightAdjustedTargetPosition(position),
      rotation
    );
    SetTargetPosition(position);
    targetZoom = GetCurrentZoom();
    verticalPivot.localRotation = Quaternion.Euler(Vector3.right * verticalStartAngle);
  }

  public void DeactivatePerspective() {
    anchor.gameObject.SetActive(false);
  }

  void Update() {
    UpdateCameraMovement();
  }

  void UpdateCameraMovement() {
    UpdateMovementTarget(input.movementInput);
    UpdateRotation(input.turnInput);
    UpdateZoomTarget(-Input.mouseScrollDelta.y);

    MoveAnchor();
    ZoomCamera();
  }

  // Handling input

  void UpdateMovementTarget(Vector3 input) {
    if(input == Vector3.zero) return;

    Vector3 newTarget = targetPosition + anchor.rotation * input * GetMoveSpeed();
    SetTargetPosition(newTarget);
  }

  void UpdateRotation(Vector2 input) {
    if(input != Vector2.zero &&
      Input.GetMouseButton(1)
    ) {
      if(invertRotation) input = -input;
      RotateAnchor(input.x);
      RotatePivot(input.y);
    }
  }

  void UpdateZoomTarget(float input) {
    if(input == 0) return;

    targetZoom = Mathf.Clamp(targetZoom + input * zoomSpeed, zoomLimit.min, zoomLimit.max);
  }

  // Updating values

  void MoveAnchor() {
    Vector3 newPosition = Vector3.SmoothDamp(anchor.position, CalculateHeightAdjustedTargetPosition(targetPosition), ref moveVelocity, moveSmoothness);
    anchor.position = newPosition;
    
    playerPerspective.SetPlayerPositionAndRotation(targetPosition, anchor.rotation);
  }

  void SetTargetPosition(Vector3 newTarget) {
    newTarget.y = GetTerrainHeight(newTarget);
    targetPosition = newTarget;
  }

  Vector3 CalculateHeightAdjustedTargetPosition(Vector3 target) {
    Vector3 adjustedTarget = target;
    adjustedTarget.y = Mathf.Lerp(target.y, defaultHeight, GetZoomPercentage());
    return adjustedTarget;
  }

  void RotateAnchor(float angleInput) {
    anchor.Rotate(Vector3.down, angleInput);
  }

  void RotatePivot(float angleInput) {
    verticalPivot.Rotate(Vector3.right, angleInput);
  }

  void ZoomCamera() {
    float zPosition = Mathf.SmoothDamp(GetCurrentZoom(), targetZoom, ref zoomVelocity, zoomSmoothness);
    cam.transform.localPosition = Vector3.back * zPosition;
  }

  // Utility

  float GetTerrainHeight(Vector3 position) {
    position.y = 100; // height could be adjustable, just needs enough clearance above terrain
    float terrainHeight = 0;

    RaycastHit hit;
    if(Physics.Raycast(position, Vector3.down, out hit, 100, terrainMask)) {
      terrainHeight = hit.point.y;
    }
    return terrainHeight;
  }

  float GetCurrentZoom() {
    return -cam.transform.localPosition.z;
  }

  float GetZoomPercentage() {
    return (GetCurrentZoom() - zoomLimit.min) / (zoomLimit.max - zoomLimit.min);
  }

  float GetMoveSpeed() {
    return Mathf.Lerp(moveSpeed.min, moveSpeed.max, GetZoomPercentage());
  }
}
