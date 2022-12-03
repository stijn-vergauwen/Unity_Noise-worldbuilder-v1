using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPosition : MonoBehaviour
{
  [SerializeField] Transform player;
  [SerializeField] LayerMask terrainMask;

  void OnEnable() {
    ChunksManager.OnStartareaLoaded += SetStartPosition;
  }

  void OnDisable() {
    ChunksManager.OnStartareaLoaded -= SetStartPosition;
  }

  void SetStartPosition() {
    Vector3 startPosition = Vector3.up * (GetGroundHeight() + 10);
    player.position = startPosition;
  }

  float GetGroundHeight() {
    float height = 100;
    RaycastHit hit;
    if(Physics.Raycast(Vector3.up * 100, Vector3.down, out hit, 120, terrainMask)) {
      height = hit.point.y;
    }
    return height;
  }
}
