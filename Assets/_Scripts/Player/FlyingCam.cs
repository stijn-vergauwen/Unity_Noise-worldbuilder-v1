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
  [SerializeField] float moveSpeed;
  [SerializeField] float turnSpeed;
  [SerializeField] float zoomSpeed;
  [SerializeField] MinMax zoomLimit;
  [SerializeField] float defaultHeight;

  [Header("Smoothness")]
  [SerializeField] float moveSmoothness;
  [SerializeField] float zoomSmoothness;

  [Header("Other")]
  [SerializeField] LayerMask terrainMask;

  Vector3 targetPosition;
  float targetZoom;

  void Update() {
    UpdateMovement(input.movementInput);
    UpdateRotation(input.turnInput);

    float scrollInput = Input.mouseScrollDelta.y;
    if(scrollInput != 0) {
      UpdateZoom(scrollInput);
    }
  }

  // updating values

  void UpdateMovement(Vector3 input) {
    // steps for movement
    // 
    // - change targetPosition by input
    // - raycast down to find terrain height
    // - set targetPosition.y as terrain height
    // - move anchor to new targetPosition using moveSmoothness. taking account of the following:
    // - if cam is zoomed in, follow terrain height exactly, so no going underground, when far away, ignore terrain height. interpolate between these.
    // 

    Vector3 newTarget = targetPosition + anchor.rotation * input * moveSpeed;
    SetTargetPosition(
      newTarget.x,
      GetTerrainHeight(newTarget),
      newTarget.z
    );
    
  }

  void UpdateRotation(Vector2 input) {
    // steps for rotation
    // 
    // - rotate anchor by horizontal input
    // - rotate verticalPivot by vertical input
    // - test if this is enough to make it work?
    // 
  }

  void UpdateZoom(float input) {
    // steps for zoom
    // 
    // - change zoom by input
    // - clamp zoom to minmax
    // - move cam z position to zoom value using zoomSmoothness
    // 
  }

  void MoveAnchor(Vector3 target) {
    anchor.position = targetPosition;
  }

  void SetTargetPosition(float x, float y, float z) {
    targetPosition = new Vector3(x, y, z);
  }

  float GetTerrainHeight(Vector3 position) {
    position.y = 50; // height could be adjustable, just needs enough clearance above terrain
    float terrainHeight = 0;

    RaycastHit hit;
    if(Physics.Raycast(position, Vector3.down, out hit, 50, terrainMask)) {
      terrainHeight = hit.point.y;
    }
    return terrainHeight;
  }

  Vector3 GetFlatPosition() {
    return new Vector3(targetPosition.x, defaultHeight, targetPosition.z);
  }
}
