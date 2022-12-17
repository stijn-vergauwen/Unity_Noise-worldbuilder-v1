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
  [SerializeField] int maxRenderDistance;
  [SerializeField] int maxPropDistance;
  [SerializeField] int maxWaterSimulatingDistance;

  int chunkSearchRadius;
  Dictionary<Coord, ChunkActivity> chunkActivityDict;

  Vector3 lastUpdatePosition;

  // Dictionary related

  bool TryGetChunkActivity(Coord chunkCoord, out ChunkActivity foundChunkActivity) {
    return chunkActivityDict.TryGetValue(chunkCoord, out foundChunkActivity);
  }

  ChunkActivity GetOrCreateChunkActivity(Coord chunkCoord) {
    ChunkActivity chunkActivity;
    if(!chunkActivityDict.TryGetValue(chunkCoord, out chunkActivity)) {
      chunkActivity = new ChunkActivity(true);
      chunkActivityDict.Add(chunkCoord, chunkActivity);
    }
    return chunkActivity;
  }

  void ResetChunksActivity() {
    foreach(ChunkActivity chunkActivity in chunkActivityDict.Values) {
      chunkActivity.ResetActive();
    }
  }

  void DeleteChunkActivity(Coord chunkCoord) {
    chunkActivityDict.Remove(chunkCoord);
  }

  // Chunk updating

  public void StartUpdator() {
    chunkSearchRadius = worldBuilder.PositionToLocalCoord(
      Vector3.right * maxRenderDistance
    ).chunkCoord.x;

    int dictionarySize = (chunkSearchRadius * 2) * (chunkSearchRadius * 2);
    chunkActivityDict = new Dictionary<Coord, ChunkActivity>(
      dictionarySize
    );

    UpdateChunkActivity();
    StartCoroutine(ChunkActivityUpdateRoutine());
  }

  IEnumerator ChunkActivityUpdateRoutine() {
    while(true) {
      if(GetDistanceToPlayer(lastUpdatePosition) > playerMoveThreshold) {
        lastUpdatePosition = playerPerspective.GetPlayerPosition();
        UpdateChunkActivity();
      }

      yield return null;
    }
  }

  void UpdateChunkActivity() {
    // TODO: refactor further

    LocalCoord playerCoord = worldBuilder.PositionToLocalCoord(playerPerspective.GetPlayerPosition());
    ResetChunksActivity();

    for(int yOffset = -chunkSearchRadius + 1; yOffset < chunkSearchRadius; yOffset ++) {
      for(int xOffset = -chunkSearchRadius + 1; xOffset < chunkSearchRadius; xOffset ++) {
        Coord searchCoord = playerCoord.chunkCoord.AddOffset(xOffset, yOffset);
        float distanceToPlayer = GetDistanceToPlayer(
          worldBuilder.CoordToPosition(searchCoord, worldBuilder.GetHalfChunkSize())
        );

        if(ShowChunk(distanceToPlayer)) {
          ChunkActivity chunkActivity = GetOrCreateChunkActivity(searchCoord);
          chunkActivity.KeepActive();
        }
      }
    }

    Coord[] chunkActivityEntriesToDelete;
    ApplyChunkActivityUpdates(out chunkActivityEntriesToDelete);
    DeleteChunkActivityEntries(chunkActivityEntriesToDelete);
  }

  void ApplyChunkActivityUpdates(out Coord[] entriesToDelete) {
    List<Coord> chunkActivityEntriesToDelete = new List<Coord>();

    foreach(Coord chunkCoord in chunkActivityDict.Keys) {
      ChunkActivity chunkActivity;
      if(TryGetChunkActivity(chunkCoord, out chunkActivity)) {
        if(chunkActivity.HasDeactivated()) {
          chunksManager.DeactivateChunkAtCoord(chunkCoord);
          chunkActivityEntriesToDelete.Add(chunkCoord);

        } else {
          ApplyChunkActivity(chunkCoord, chunkActivity);
        }
      }
    }

    entriesToDelete = chunkActivityEntriesToDelete.ToArray();
  }

  void DeleteChunkActivityEntries(Coord[] chunkCoords) {
    foreach(Coord chunkCoord in chunkCoords) {
      DeleteChunkActivity(chunkCoord);
      Chunk chunk;
      if(chunksManager.TryGetChunkByCoord(chunkCoord, out chunk)) {
        chunksManager.RemoveActiveChunk(chunk);
      }
    }
  }

  void ApplyChunkActivity(Coord chunkCoord, ChunkActivity chunkActivity) {
    float distanceToPlayer = GetDistanceToPlayer(
      worldBuilder.CoordToPosition(chunkCoord, worldBuilder.GetHalfChunkSize())
    );
  
    Chunk chunk = chunksManager.GetOrCreateChunk(chunkCoord);
    chunksManager.ActivateChunk(chunk);

    chunksManager.ToggleChunkVegetation(chunk, ShowChunkVegetation(distanceToPlayer));
    chunksManager.ToggleChunkWaterLayer(chunk, SimulateChunkWater(distanceToPlayer));
    
    if(chunkActivity.HasActivated()) {
      chunksManager.AddActiveChunk(chunk);
    }
  }

  bool ShowChunk(float distanceToPlayer) {
    return distanceToPlayer < maxRenderDistance;
  }

  bool ShowChunkVegetation(float distanceToPlayer) {
    return (
      worldBuilder.spawnVegetation &&
      distanceToPlayer < maxPropDistance
    );
  }

  bool SimulateChunkWater(float distanceToPlayer) {
    return distanceToPlayer < maxWaterSimulatingDistance;
  }

  // Utility

  float GetDistanceToPlayer(Vector3 point) {
    return Vector3.Distance(point, playerPerspective.GetPlayerPosition());
  }

  class ChunkActivity {
    // TODO: either remove prevActive as i don't use it, OR change name to ChunkVisibility & store visibility state for terrain, vegetation, and water separately
    public bool prevActive;
    public bool newActive;

    public ChunkActivity(bool newActive) {
      this.newActive = newActive;
      prevActive = false;
    }

    public void KeepActive() {
      newActive = true;
    }

    public void ResetActive() {
      prevActive = newActive;
      newActive = false;
    }

    public bool HasActivated() {
      return !prevActive && newActive;
    }

    public bool StayedActive() {
      return prevActive && newActive;
    }

    public bool HasDeactivated() {
      return !newActive;
    }
  }
}
