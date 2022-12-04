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
  [SerializeField] bool randomizeNoiseSeeds = false;

  [Header("Player movement based chunk updating")]
  [SerializeField] PlayerPerspective playerPerspective;
  [SerializeField] int playerMoveThreshold;
  [SerializeField] int chunkMaxTerrainDistance;
  [SerializeField] int chunkMaxPropDistance;
  [SerializeField, Min(1)] int chunkGenerateRadius; // radius at which new chunks get created & setup

  LocalCoord playerCoord;
  Vector2 lastUpdatePosition;

  Dictionary<Coord, Chunk> chunks = new Dictionary<Coord, Chunk>();

  public Action<MapToDraw> OnDrawMap;

  public static Action OnStartareaLoaded;

  void Start() {
    if(randomizeNoiseSeeds) {
      worldBuilder.RandomizeSeedValues();
    }

    if(endlessTerrain) {
      UpdateVisibleChunks();
      OnStartareaLoaded?.Invoke();
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
    return GetDistanceToPlayer(chunk.transform.position) < chunkMaxTerrainDistance;
  }

  public bool CheckChunkPropVisibility(Chunk chunk) {
    return (
      worldBuilder.SpawnVegitation &&
      GetDistanceToPlayer(chunk.transform.position) < chunkMaxPropDistance
    );
  }

  float GetDistanceToPlayer(Vector2 point) {
    return Vector2.Distance(point, playerPerspective.GetFlatPlayerPosition());
  }

  float GetDistanceToPlayer(Vector3 point) {
    return GetDistanceToPlayer(new Vector2(point.x, point.z));
  }

  public void CreateVegitationForChunk(Chunk chunk) {
    vegitationPlacer.PlaceVegitationProps(chunk, worldBuilder.ChunkSize, worldBuilder.BiomeSet);
  }

  IEnumerator CheckChunkUpdateRoutine() {
    while(true) {
      if(GetDistanceToPlayer(lastUpdatePosition) > playerMoveThreshold) {
        playerCoord = worldBuilder.PositionToLocalCoord(playerPerspective.GetPlayerPosition());
        lastUpdatePosition = playerPerspective.GetFlatPlayerPosition();
        UpdateVisibleChunks();
      }

      yield return new WaitForSeconds(.1f);
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
