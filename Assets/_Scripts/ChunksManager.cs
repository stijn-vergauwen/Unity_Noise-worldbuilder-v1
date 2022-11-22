using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksManager : MonoBehaviour
{
  public enum MapToDraw {Height, Biome, Temperature, Humidity}

  [SerializeField] WorldBuilder worldBuilder;
  [SerializeField] VegitationPlacer vegitationPlacer;
  [SerializeField] MapToDraw mapToDraw;
  public bool endlessTerrain;

  [Header("Player movement based chunk updating")]
  [SerializeField] Transform playerTransform;
  [SerializeField] int playerMoveThreshold;
  [SerializeField] int chunkMaxTerrainDistance;
  [SerializeField] int chunkMaxPropDistance;
  [SerializeField, Min(1)] int chunkGenerateRadius; // radius at which new chunks get created & setup

  LocalCoord playerCoord;
  Vector2 lastUpdatePosition;

  Dictionary<Coord, Chunk> chunks = new Dictionary<Coord, Chunk>();

  public Action<MapToDraw> OnDrawMap;

  void Start() {
    if(endlessTerrain) {
      UpdateVisibleChunks();
      StartCoroutine(CheckChunkUpdateRoutine());
    }
  }

  void Update() {
    CheckPlayerInput();
  }

  public void AddChunk(Coord chunkCoord, Chunk chunk) {
    chunks.Add(chunkCoord, chunk);
  }

  public bool TryGetChunkByCoord(Coord chunkCoord, out Chunk foundChunk) {
    return chunks.TryGetValue(chunkCoord, out foundChunk);
  }

  Chunk GetOrCreateChunk(Coord chunkCoord) {
    Chunk chunk;
    if(!TryGetChunkByCoord(chunkCoord, out chunk)) {
      chunk = worldBuilder.GenerateChunk(chunkCoord);
      chunk.OnDrawMap(mapToDraw);
    }
    return chunk;
  }

  public void ClearChunks() {
    chunks.Clear();
  }

  // updating chunks

  void UpdateVisibleChunks() {
    for(int yOffset = -chunkGenerateRadius + 1; yOffset < chunkGenerateRadius; yOffset ++) {
      for(int xOffset = -chunkGenerateRadius + 1; xOffset < chunkGenerateRadius; xOffset ++) {
        Chunk chunk = GetOrCreateChunk(playerCoord.chunkCoord.AddOffset(xOffset, yOffset));
        chunk.SetChunkActive(CheckChunkVisibility(chunk));
        chunk.SetVegitationActive(CheckChunkPropVisibility(chunk));
      }
    }
  }

  public bool CheckChunkVisibility(Chunk chunk) {
    return ChunkDistanceToPlayer(chunk) < chunkMaxTerrainDistance;
  }

  public bool CheckChunkPropVisibility(Chunk chunk) {
    return (
      worldBuilder.SpawnVegitation &&
      ChunkDistanceToPlayer(chunk) < chunkMaxPropDistance
    );
  }

  float ChunkDistanceToPlayer(Chunk chunk) {
    return Vector3.Distance(chunk.transform.position, playerTransform.position);
  }

  public void CreateVegitationForChunk(Chunk chunk) {
    vegitationPlacer.PlaceVegitationProps(chunk, worldBuilder.ChunkSize, worldBuilder.BiomeSet);
  }

  IEnumerator CheckChunkUpdateRoutine() {
    while(true) {
      Vector2 flatPlayerPos = new Vector2(playerTransform.position.x, playerTransform.position.z);
      if(Vector2.Distance(flatPlayerPos, lastUpdatePosition) > playerMoveThreshold) {
        playerCoord = worldBuilder.PositionToLocalCoord(playerTransform.position);
        lastUpdatePosition = flatPlayerPos;
        UpdateVisibleChunks();

        // print($"Player is in chunk (x:{playerCoord.chunkCoord.x}, y:{playerCoord.chunkCoord.y})");
      }

      yield return new WaitForSeconds(.5f);
    }
  }

  
  // does this make any sense? i only want chunks to depend on ChunksManager
  public BiomeSetSO GetBiomeSet() {
    return worldBuilder.BiomeSet;
  }

  public void DisplayMap() {
    print($"display {mapToDraw} map");
    
    OnDrawMap?.Invoke(mapToDraw);
  }

  void CheckPlayerInput() {
    if(Input.GetKeyDown(KeyCode.M)) {
      NextMapToDraw();
      DisplayMap();
    }
  }

  void NextMapToDraw() {
    if(mapToDraw == MapToDraw.Humidity) {
      mapToDraw = 0;

    } else {
      mapToDraw++;
    }
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.green;
    foreach(Chunk chunk in chunks.Values) {
      Vector3 chunkposition = worldBuilder.CoordToFlatPosition(worldBuilder.LocalToWorldCoord(chunk.chunkCoord, new Coord(0, 0)));
      Vector3 halfChunkSize = new Vector3(.5f, 0, .5f) * worldBuilder.ChunkSize;
      Gizmos.DrawSphere(chunkposition + halfChunkSize, 2);
    }
  }
}
