using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartPosition : MonoBehaviour
{
  [SerializeField] WorldBuilder worldBuilder;
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
    Vector3 startPosition = GetGroundHeight() + Vector3.up * offsetAboveGround;
    player.StartPlayer(startPosition);
  }

  Vector3 GetGroundHeight() {
    return worldBuilder.CoordToPosition(new Coord(0, 0));
  }
}
