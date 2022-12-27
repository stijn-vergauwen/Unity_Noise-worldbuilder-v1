using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
  public enum BuildMode {Map, OpenWorld}

  [SerializeField] ChunksManager chunksManager;
  [SerializeField] Chunk chunkPrefab;
  [SerializeField] Transform chunkHolder;
  [SerializeField] GameSettingsSO gameSettingsData;

  [Header("Settings")]
  [SerializeField] WorldSettings worldSettings;

  [Header("Generate settings")]
  [SerializeField] bool randomizeNoiseSeeds = false;
  [SerializeField, Range(0, 5)] int biomeTransitionSmoothness;

  public bool spawnVegetation;
  public bool endlessTerrain;
  public bool mapPreviewMode;
  public bool useGameSettingsData;

  public MapToDraw mapToDraw;
  [SerializeField] BuildMode buildMode;

  // ChunkSize is correct for coordinates, ChunkMapSize is correct for dataMap & mesh size
  public int ChunkSize => worldSettings.chunkSize;
  public int ChunkMapSize => worldSettings.chunkSize + 1;

  public float Tilesize => worldSettings.tileSize;

  public BiomeSetSO BiomeSet => worldSettings.biomeSet;

  public int BiomeTransitionSmoothness => biomeTransitionSmoothness;

  void Awake() {
    if(randomizeNoiseSeeds) {
      RandomizeSeedValues();
    }
  }

  void Start() {
    if(mapPreviewMode) {
      return;
    }

    if(useGameSettingsData) {
      GetGameSettingsData();
    }

    if(endlessTerrain) {
      chunksManager.StartUpdator();

    } else {
      GenerateWorld();
    }

    chunksManager.RaiseStartAreaLoaded();
  }

  void RandomizeSeedValues() {
    worldSettings.heightMap.seed = Random.Range(1, 1000);
    worldSettings.temperatureMap.seed = Random.Range(1, 1000);
    worldSettings.humidityMap.seed = Random.Range(1, 1000);
  }

  public void RefreshPreview() {
    ClearChunks();
    GetGameSettingsData();
    spawnVegetation = false;
    GenerateWorld();
  }

  void GetGameSettingsData() {
    worldSettings = gameSettingsData.GetWorldSettings();
    biomeTransitionSmoothness = gameSettingsData.biomeTransitionSmoothness;
    spawnVegetation = gameSettingsData.spawnVegetation;
  }

  void GenerateWorld() {
    List<Chunk> chunks = new List<Chunk>();

    // generate all chunks, the -size + 1 is to go the same amount in positive & negative directions
    for (int y = -worldSettings.worldSize + 1; y < worldSettings.worldSize; y++) {
      for (int x = -worldSettings.worldSize + 1; x < worldSettings.worldSize; x++) {
        Coord chunkCoord = new Coord(x, y);
        Chunk newChunk = SetupNewChunk(chunkCoord);

        chunksManager.AddChunk(chunkCoord, newChunk);
        chunksManager.AddActiveChunk(newChunk);
        chunks.Add(newChunk);
      }
    }

    foreach(Chunk chunk in chunks) {
      if(IsAtWorldEdge(chunk.chunkCoord)) continue;

      chunksManager.ActivateChunk(chunk);

      if(spawnVegetation) {
        chunksManager.ToggleChunkVegetation(chunk, true);
      }
      chunksManager.ToggleChunkWaterLayer(chunk, true);
    }
  }

  bool IsAtWorldEdge(Coord chunkCoord) {
    return Mathf.Abs(chunkCoord.x) == worldSettings.worldSize - 1 || Mathf.Abs(chunkCoord.y) == worldSettings.worldSize - 1;
  }

  public Chunk SetupNewChunk(Coord chunkCoord) {
    Vector2 chunkOffset = ChunkCoordToOffset(chunkCoord);
    Vector3 chunkPosition = transform.position + new Vector3(chunkOffset.x, 0, chunkOffset.y) * Tilesize;

    Chunk newChunk = CreateChunk(chunkCoord, chunkOffset, chunkPosition);

    if(buildMode == BuildMode.Map) {
      newChunk.CreateFlatMesh(ChunkMapSize, Tilesize);
      
    } else {
      newChunk.CreateMesh(Tilesize, worldSettings.heightMultiplier, worldSettings.meshHeightCurve);
    }

    newChunk.DrawMap(mapToDraw);

    return newChunk;
  }

  Chunk CreateChunk(Coord chunkCoord, Vector2 chunkOffset, Vector3 chunkPosition) {
    Chunk newChunk = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity, chunkHolder);

    newChunk.Init(
      chunksManager,
      chunkCoord,
      Noise.GenerateNoiseMap(ChunkMapSize, worldSettings.heightMap, chunkOffset),
      Noise.GenerateNoiseMap(ChunkMapSize, worldSettings.temperatureMap, chunkOffset),
      Noise.GenerateNoiseMap(ChunkMapSize, worldSettings.humidityMap, chunkOffset)
    );

    return newChunk;
  }

  void ClearChunks() {
    chunksManager.ClearActiveChunks();
    chunksManager.ClearChunks();
    foreach(Transform chunkTransform in chunkHolder.transform) {
      Destroy(chunkTransform.gameObject);
    }
  }

  // Position & Coord utility

  public Vector2 ChunkCoordToOffset(Coord chunkCoord) {
    return new Vector2(chunkCoord.x, chunkCoord.y) * ChunkSize;
  }

  public Vector3 CoordToPosition(Coord chunkCoord, Coord tileCoord) {
    Vector3 position = CoordToFlatPosition(LocalToWorldCoord(chunkCoord, tileCoord));
    Chunk chunk;
    if(chunksManager.TryGetChunkByCoord(chunkCoord, out chunk)) {
      position.y = MapHeightToWorldHeight(GetLowestheightAtCoord(chunk, tileCoord));
    }
    return position;
  }
  
  public Vector3 CoordToPosition(Coord worldCoord) {
    LocalCoord localCoord = WorldToLocalCoord(worldCoord);
    Vector3 position = CoordToFlatPosition(worldCoord);
    Chunk chunk;
    if(chunksManager.TryGetChunkByCoord(localCoord.chunkCoord, out chunk)) {
      position.y = MapHeightToWorldHeight(GetLowestheightAtCoord(chunk, localCoord.tileCoord));
    }
    return position;
  }

  public Vector3 CoordToFlatPosition(Coord worldCoord) {
    return new Vector3(
      worldCoord.x - ChunkSize * .5f,
      0,
      worldCoord.y - ChunkSize * .5f
    ) * Tilesize;
  }

  public Coord PositionToWorldCoord(Vector3 worldPos) {
    return new Coord(
      Mathf.RoundToInt((worldPos.x + ChunkSize * .5f) / Tilesize),
      Mathf.RoundToInt((worldPos.z + ChunkSize * .5f) / Tilesize)
    );
  }

  public LocalCoord PositionToLocalCoord(Vector3 worldPos) {
    return WorldToLocalCoord(PositionToWorldCoord(worldPos));
  }

  public Coord LocalToWorldCoord(Coord chunkCoord, Coord tileCoord) {
    return new Coord(
      chunkCoord.x * ChunkSize + tileCoord.x,
      chunkCoord.y * ChunkSize + tileCoord.y
    );
  }

  public LocalCoord WorldToLocalCoord(Coord worldCoord) {
    Coord chunkCoord = new Coord(
      Mathf.FloorToInt((float)worldCoord.x / ChunkSize),
      Mathf.FloorToInt((float)worldCoord.y / ChunkSize)
    );

    return new LocalCoord(chunkCoord, TileFromWorldCoord(worldCoord));
  }

  float MapHeightToWorldHeight(float value) {
    return worldSettings.meshHeightCurve.Evaluate(value) * worldSettings.heightMultiplier * Tilesize;
  }

  Coord TileFromWorldCoord(Coord worldCoord) {
    // gets the correct tile coord with both positive & negative input numbers
    return new Coord(
      (worldCoord.x < 0 ? ChunkSize + worldCoord.x % ChunkSize : worldCoord.x) % ChunkSize,
      (worldCoord.y < 0 ? ChunkSize + worldCoord.y % ChunkSize : worldCoord.y) % ChunkSize
    );
  }

  float GetLowestheightAtCoord(Chunk chunk, Coord tileCoord) {
    float[] heightsOfCorners = {
      chunk.GetHeightMapAtCoord(tileCoord),
      chunk.GetHeightMapAtCoord(tileCoord.AddOffset(1, 0)),
      chunk.GetHeightMapAtCoord(tileCoord.AddOffset(0, 1)),
      chunk.GetHeightMapAtCoord(tileCoord.AddOffset(1, 1))
    };
    return Mathf.Min(heightsOfCorners);
  }

  public Coord GetHalfChunkSize() {
    return new Coord(
      Mathf.RoundToInt(ChunkSize * .5f),
      Mathf.RoundToInt(ChunkSize * .5f)
    );
  }

  public bool UseWaterLayer() {
    return buildMode == BuildMode.OpenWorld;
  }
}

public enum MapToDraw {Biome, Height, Temperature, Humidity}