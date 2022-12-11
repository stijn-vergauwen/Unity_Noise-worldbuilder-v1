using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainUpdator : MonoBehaviour
{
  [SerializeField] ChunksManager chunksManager;
  [SerializeField] WorldBuilder worldBuilder;

  [Header("Player movement based chunk updating")]
  [SerializeField] PlayerPerspective playerPerspective;
  [SerializeField] int playerMoveThreshold;
  [SerializeField] int chunkMaxTerrainDistance;
  [SerializeField] int chunkMaxPropDistance;

  int chunkSearchRadius;

  LocalCoord playerCoord;
  Vector3 lastUpdatePosition;

  public void StartUpdator() {
    chunkSearchRadius = worldBuilder.PositionToLocalCoord(
      Vector3.right * chunkMaxTerrainDistance
    ).chunkCoord.x;

    UpdateVisibleChunks();
    StartCoroutine(CheckChunkUpdateRoutine());
  }

  void UpdateVisibleChunks() {
    for(int yOffset = -chunkSearchRadius + 1; yOffset < chunkSearchRadius; yOffset ++) {
      for(int xOffset = -chunkSearchRadius + 1; xOffset < chunkSearchRadius; xOffset ++) {
        Chunk chunk = chunksManager.GetOrCreateChunk(playerCoord.chunkCoord.AddOffset(xOffset, yOffset));
        chunk.SetChunkActive(CheckChunkVisibility(chunk));
        chunk.SetVegetationActive(CheckChunkPropVisibility(chunk));
      }
    }
    chunksManager.RaiseOnVisibleChunksUpdate();
  }

  IEnumerator CheckChunkUpdateRoutine() {
    while(true) {
      if(GetDistanceToPlayer(lastUpdatePosition) > playerMoveThreshold) {
        playerCoord = worldBuilder.PositionToLocalCoord(playerPerspective.GetPlayerPosition());
        lastUpdatePosition = playerPerspective.GetPlayerPosition();
        UpdateVisibleChunks();
      }

      yield return new WaitForSeconds(.05f);
    }
  }

  // Chunk visibility

  public bool CheckChunkVisibility(Chunk chunk) {
    return GetDistanceToPlayer(chunk.transform.position) < chunkMaxTerrainDistance;
  }

  public bool CheckChunkPropVisibility(Chunk chunk) {
    return (
      worldBuilder.spawnVegetation &&
      GetDistanceToPlayer(chunk.transform.position) < chunkMaxPropDistance
    );
  }

  // Utility

  float GetDistanceToPlayer(Vector3 point) {
    return Vector3.Distance(point, playerPerspective.GetPlayerPosition());
  }
}
