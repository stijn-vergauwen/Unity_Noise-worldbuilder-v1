using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksManager : MonoBehaviour
{
  public enum MapToDraw {Height, Biome, Temperature, Humidity}

  [SerializeField] WorldBuilder worldBuilder;
  [SerializeField] VegitationPlacer vegitationPlacer;
  [SerializeField] WaterLayer waterLayer;
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

  // TODO: make a list<> holding all current active chunks, update this each time UpdateVisibleChunks is called, saves checks & scales better than chunkGenerateRadius
  // also need to use that list to update water layers of active chunks with water



  public Action<MapToDraw> OnDrawMap;

  public static Action OnStartareaLoaded;

  void Start() {
    if(randomizeNoiseSeeds) {
      worldBuilder.RandomizeSeedValues();
    }

    if(endlessTerrain) {
      UpdateVisibleChunks();
      StartCoroutine(CheckChunkUpdateRoutine());
    }
    OnStartareaLoaded?.Invoke();
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

      yield return new WaitForSeconds(.02f);
    }
  }
  
  public BiomeSetSO GetBiomeSet() {
    return worldBuilder.BiomeSet;
  }

  public void CheckIfWaterInChunk(Chunk chunk) {
    if(!waterLayer.SimulateWater) return;
    int[,] biomeMap = chunk.biomeMap;
    int mapSize = biomeMap.GetLength(0);
    bool waterInChunk = false;
    BiomeSetSO biomeSet = GetBiomeSet();

    for(int y = 0; y < mapSize; y++) {
      for(int x = 0; x < mapSize; x++) {
        if(biomeSet.FindBiome(biomeMap[x,y]).biomeName == "Ocean") {
          waterInChunk = true;
          break;
        }
      }  
      if(waterInChunk) break;
    }

    if(waterInChunk) {
      MeshFilter chunkWaterLayer = waterLayer.CreateChunkWaterLayer(chunk);
      chunk.SetWaterLayer(chunkWaterLayer);
    }
  }

  public void UpdateChunkWaterLayer(MeshFilter waterMeshFilter, Coord chunkCoord) {
    Vector3[] updatedVertices = waterLayer.CalculateNewMeshVertices(waterMeshFilter.mesh.vertices, chunkCoord);
    waterMeshFilter.mesh.vertices = updatedVertices;
  }

  // display other data maps

  public void DisplayMap() {
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
