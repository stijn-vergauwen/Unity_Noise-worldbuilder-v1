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
  [SerializeField, Range(0, 5)] int biomeTransitionSmoothness;

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

  // TODO: this class is becoming a ChunkManager+allthisotherstuff class, splitting and refactoring needed


  public Action<MapToDraw> OnDrawMap;
  public Action<MapToDraw> OnVisibleChunksUpdate;

  public static Action OnStartareaLoaded;

  // setup
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
    CheckPlayerInput(); // only for testing
  }

  // chunks CRUD, the main task of this class

  public void AddChunk(Coord chunkCoord, Chunk chunk) {
    chunks.Add(chunkCoord, chunk);
  }

  public bool CheckChunkByCoord(Coord chunkCoord) {
    return chunks.ContainsKey(chunkCoord);
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

  // updating chunks, should move to terrainUpdator

  void UpdateVisibleChunks() {
    for(int yOffset = -chunkGenerateRadius + 1; yOffset < chunkGenerateRadius; yOffset ++) {
      for(int xOffset = -chunkGenerateRadius + 1; xOffset < chunkGenerateRadius; xOffset ++) {
        Chunk chunk = GetOrCreateChunk(playerCoord.chunkCoord.AddOffset(xOffset, yOffset));
        chunk.SetChunkActive(CheckChunkVisibility(chunk));
        chunk.SetVegitationActive(CheckChunkPropVisibility(chunk));
      }
    }
    OnVisibleChunksUpdate?.Invoke(mapToDraw);
  }

  IEnumerator CheckChunkUpdateRoutine() {
    while(true) {
      if(GetDistanceToPlayer(lastUpdatePosition) > playerMoveThreshold) {
        playerCoord = worldBuilder.PositionToLocalCoord(playerPerspective.GetPlayerPosition());
        lastUpdatePosition = playerPerspective.GetFlatPlayerPosition();
        UpdateVisibleChunks();
      }

      yield return new WaitForSeconds(.05f);
    }
  }

  // chunk visibility, should move to terrainUpdator

  public bool CheckChunkVisibility(Chunk chunk) {
    return GetDistanceToPlayer(chunk.transform.position) < chunkMaxTerrainDistance;
  }

  public bool CheckChunkPropVisibility(Chunk chunk) {
    return (
      worldBuilder.SpawnVegitation &&
      GetDistanceToPlayer(chunk.transform.position) < chunkMaxPropDistance
    );
  }

  // distance to player utility

  float GetDistanceToPlayer(Vector2 point) {
    return Vector2.Distance(point, playerPerspective.GetFlatPlayerPosition());
  }

  float GetDistanceToPlayer(Vector3 point) {
    return GetDistanceToPlayer(new Vector2(point.x, point.z));
  }

  // chunk vegitation

  public void CreateVegitationForChunk(Chunk chunk) {
    vegitationPlacer.PlaceVegitationProps(chunk, worldBuilder.ChunkSize, worldBuilder.BiomeSet);
  }

  // accessor
  
  public BiomeSetSO GetBiomeSet() {
    return worldBuilder.BiomeSet;
  }

  // water layer related

  public void CheckIfWaterInChunk(Chunk chunk) {
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
    if(!waterLayer.SimulateWater) return;
    Vector3[] updatedVertices = waterLayer.CalculateNewMeshVertices(waterMeshFilter.mesh.vertices, chunkCoord);
    waterMeshFilter.mesh.vertices = updatedVertices;
  }

  // smoothing biome colors

  public bool CheckIfChunkCanCreateTerrain(Coord chunkCoord) {
    return GetChunkNeighborCount(chunkCoord) == 8;
  }

  int GetChunkNeighborCount(Coord chunkCoord) {
    int neighborCount = 0;
    foreach(Coord offset in GetSurroundingCoordOffsets()) {
      Coord neighborCoord = chunkCoord.AddOffset(offset);
      if(CheckChunkByCoord(neighborCoord)) {
        neighborCount++;
      }
    }
    return neighborCount;
  }

  public Color[] CreateAveragedChunkGroundColors(Coord chunkCoord, int[,] biomeMap) {
    int size = biomeMap.GetLength(0);
    BiomeSetSO biomeSet = GetBiomeSet();
    Color[] colorArray = new Color[size * size];

    for (int y = 0; y < size; y++) {
      for (int x = 0; x < size; x++) {
        LocalCoord localCoord = new LocalCoord(chunkCoord, new Coord(x, y));
        colorArray[y * size + x] = GetAverageGroundColor(localCoord, biomeTransitionSmoothness, biomeMap, biomeSet);
      }
    }

    return colorArray;
  }

  Color GetAverageGroundColor(LocalCoord localCoord, int searchRadius, int[,] biomeMap, BiomeSetSO biomeSet) {
    int size = biomeMap.GetLength(0);
    float totalR = 0;
    float totalG = 0;
    float totalB = 0;
    int sampleCount = 0;
    
    for (int yOffset = -searchRadius; yOffset <= searchRadius; yOffset++) {
      for (int xOffset = -searchRadius; xOffset <= searchRadius; xOffset++) {
        Coord searchCoord = localCoord.tileCoord.AddOffset(xOffset, yOffset);
        int biomeId;
        // check if coord is within chunk, if in different chunk, get biome from other chunk
        if(searchCoord.x >= 0 && searchCoord.x < size && searchCoord.y >= 0 && searchCoord.y < size) {
          biomeId = biomeMap[searchCoord.x, searchCoord.y];

        } else {
          biomeId = GetBiomeIdInChunk(worldBuilder.LocalToWorldCoord(localCoord.chunkCoord, searchCoord));
        }

        Color sample = biomeSet.FindBiome(biomeId).groundColor;
        totalR += sample.r;
        totalG += sample.g;
        totalB += sample.b;
        sampleCount++;
      }
    }

    return new Color(
      totalR / sampleCount,
      totalG / sampleCount,
      totalB / sampleCount
    );
  }

  int GetBiomeIdInChunk(Coord worldCoord) {
    LocalCoord localCoord = worldBuilder.WorldToLocalCoord(worldCoord);
    Chunk chunk;
    int biomeId = 0;

    if(TryGetChunkByCoord(localCoord.chunkCoord, out chunk)) {
      biomeId = chunk.GetBiomeIdAtCoord(localCoord.tileCoord);
    }

    return biomeId;
  }

  public Coord[] GetSurroundingCoordOffsets() {
    return new Coord[] {
      new Coord(0, 1),
      new Coord(1, 1),
      new Coord(1, 0),
      new Coord(1, -1),
      new Coord(0, -1),
      new Coord(-1, -1),
      new Coord(-1, 0),
      new Coord(-1, 1)
    };
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
