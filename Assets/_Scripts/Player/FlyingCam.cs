using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCam : MonoBehaviour
{
  [SerializeField] CharacterInput input;

  [SerializeField] Transform anchor;
  [SerializeField] Transform verticalPivot;
  [SerializeField] Camera cam;

  [Header("Movement")]
  [SerializeField] MinMax moveSpeed;
  [SerializeField] float turnSpeed;
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

  void OnEnable() {
    ChunksManager.OnStartareaLoaded += SetStartValues;
  }

  void OnDisable() {
    ChunksManager.OnStartareaLoaded -= SetStartValues;
  }

  void SetStartValues() {
    SetTargetPosition(Vector3.zero);
    anchor.position = targetPosition;
    targetZoom = GetCurrentZoom();
  }

  void Update() {
    UpdateCameraMovement();
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

  void UpdateCameraMovement() {
    UpdateMovementTarget(input.movementInput);
    UpdateRotation(input.turnInput);
    UpdateZoomTarget(-Input.mouseScrollDelta.y);
    
    if(Mathf.Abs(zoomVelocity) > .1f) {
      UpdateTargetheight();
    }
    MoveAnchor();
    ZoomCamera();
  }

  void MoveAnchor() {
    anchor.position = Vector3.SmoothDamp(anchor.position, targetPosition, ref moveVelocity, moveSmoothness);
  }

  void SetTargetPosition(Vector3 newTarget) {
    targetPosition = newTarget;
    if(Mathf.Abs(zoomVelocity) < .1f) {
      UpdateTargetheight();
    }
  }

  void UpdateTargetheight() {
    float targetHeight = Mathf.Lerp(GetTerrainHeight(targetPosition), defaultHeight, GetZoomPercentage());
    targetPosition.y = targetHeight;
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
