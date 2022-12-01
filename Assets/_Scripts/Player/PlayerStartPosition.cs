using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPosition : MonoBehaviour
{
  [SerializeField] Transform player;
  [SerializeField] LayerMask terrainMask;

  void Start() {
    Invoke("SetStartPosition", .01f);
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
    print("ground height found at: " + height);
    return height;
  }
}
