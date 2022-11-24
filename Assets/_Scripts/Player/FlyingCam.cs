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

  Vector3 position;
  float zoom;

  void Update() {
    UpdateMovement(input.movementInput);
    UpdateRotation(input.turnInput);

    float scrollInput = Input.mouseScrollDelta.y;
    if(scrollInput != 0) {
      UpdateZoom(scrollInput);
    }
  }

  void UpdateMovement(Vector3 input) {
    // steps for movement
    // 
    // - change position by input
    // - raycast down to find terrain height
    // - set position.y as terrain height
    // - move anchor to new position using moveSmoothness. taking account of the following:
    // - if cam is zoomed in, follow terrain height exactly, so no going underground, when far away, ignore terrain height. interpolate between these.
    // 
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

  Vector3 GetFlatPosition() {
    return new Vector3(position.x, defaultHeight, position.z);
  }
}
