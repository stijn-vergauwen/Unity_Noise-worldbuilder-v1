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

  Vector3 lastUpdatePosition;

  public void StartUpdator() {
    chunkSearchRadius = worldBuilder.PositionToLocalCoord(
      Vector3.right * chunkMaxTerrainDistance
    ).chunkCoord.x;

    UpdateVisibleChunks();
    StartCoroutine(CheckChunkUpdateRoutine());
  }

  IEnumerator CheckChunkUpdateRoutine() {
    while(true) {
      if(GetDistanceToPlayer(lastUpdatePosition) > playerMoveThreshold) {
        lastUpdatePosition = playerPerspective.GetPlayerPosition();
        UpdateVisibleChunks();
      }

      yield return null;
    }
  }

  void UpdateVisibleChunks() {
    LocalCoord playerCoord = worldBuilder.PositionToLocalCoord(playerPerspective.GetPlayerPosition());
    chunksManager.ClearActiveChunks();

    // TODO: don't clear active chunks each update.
    // find a way to only remove the chunks that shouldn't be active anymore, and add chunks that should become active.
    // this is a cleaner solution, and allows chunks that get deactivated to also clear vegetation props

    for(int yOffset = -chunkSearchRadius + 1; yOffset < chunkSearchRadius; yOffset ++) {
      for(int xOffset = -chunkSearchRadius + 1; xOffset < chunkSearchRadius; xOffset ++) {
        UpdateChunk(chunksManager.GetOrCreateChunk(
          playerCoord.chunkCoord.AddOffset(xOffset, yOffset)
        ));
      }
    }
  }

  // Chunk visibility

  void UpdateChunk(Chunk chunk) {
    if(ShowChunk(chunk)) {
      chunksManager.ActivateChunk(chunk);

      if(ShowChunkVegetation(chunk)) {
        chunksManager.ActivateChunkVegetation(chunk);
      }

      if(SimulateChunkWater(chunk)) {
        chunksManager.ActivateChunkWaterLayer(chunk);
      }
    }
  }

  public bool ShowChunk(Chunk chunk) {
    return GetDistanceToPlayer(chunk.transform.position) < chunkMaxTerrainDistance;
  }

  public bool ShowChunkVegetation(Chunk chunk) {
    return (
      worldBuilder.spawnVegetation &&
      GetDistanceToPlayer(chunk.transform.position) < chunkMaxPropDistance
    );
  }

  public bool SimulateChunkWater(Chunk chunk) {
    return ShowChunk(chunk); // TODO: add a max water distance;
  }

  // Utility

  float GetDistanceToPlayer(Vector3 point) {
    return Vector3.Distance(point, playerPerspective.GetPlayerPosition());
  }
}
