using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPosition : MonoBehaviour
{
  [SerializeField] PlayerPerspective player;
  [SerializeField] LayerMask terrainMask;

  [SerializeField] float offsetAboveGround;

  void OnEnable() {
    ChunksManager.OnStartareaLoaded += SetStartPosition;
  }

  void OnDisable() {
    ChunksManager.OnStartareaLoaded -= SetStartPosition;
  }

  void SetStartPosition() {
    Vector3 startPosition = Vector3.up * (GetGroundHeight() + offsetAboveGround);
    player.StartPlayer(startPosition);
  }

  float GetGroundHeight() {
    float height = 100;
    RaycastHit hit;
    if(Physics.Raycast(Vector3.up * height, Vector3.down, out hit, height, terrainMask)) {
      height = hit.point.y;
    }
    return height;
  }
}
