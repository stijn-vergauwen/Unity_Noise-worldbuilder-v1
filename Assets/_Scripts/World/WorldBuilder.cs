using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
  public enum BuildMode {Map, OpenWorld}

  [SerializeField] ChunksManager chunksManager;
  [SerializeField] Chunk chunkPrefab;
  [SerializeField] Transform chunkHolder;

  [Header("Settings")]
  [SerializeField] WorldSettings worldSettings;

  [Header("Generate settings")]
  [SerializeField] bool randomizeNoiseSeeds = false;
  [SerializeField, Range(0, 5)] int biomeTransitionSmoothness;

  public bool spawnVegetation;
  public bool endlessTerrain;

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
    if(!endlessTerrain) {
      GenerateWorld();
      chunksManager.RaiseStartAreaLoaded();
    }
  }

  void RandomizeSeedValues() {
    worldSettings.heightMap.seed = Random.Range(1, 1000);
    worldSettings.temperatureMap.seed = Random.Range(1, 1000);
    worldSettings.humidityMap.seed = Random.Range(1, 1000);
  }

  void GenerateWorld() {
    // this method became very messy, too many workaround solutions for making the chunks
    // so first i need to make the creation and toggling of chunk data way more modular and easy to work with.


    // // generate all chunks, the -size + 1 is to go the same amount in positive & negative directions
    // for (int y = -worldSettings.worldSize + 1; y < worldSettings.worldSize; y++) {
    //   for (int x = -worldSettings.worldSize + 1; x < worldSettings.worldSize; x++) {
    //     SetupNewChunk(new Coord(x, y));
    //   }
    // }
    // chunksManager.RaiseOnVisibleChunksUpdate();
    // chunksManager.DisplayMap();

    // for (int y = -worldSettings.worldSize + 1; y < worldSettings.worldSize; y++) {
    //   for (int x = -worldSettings.worldSize + 1; x < worldSettings.worldSize; x++) {
    //     Chunk chunk;
    //     if(chunksManager.TryGetChunkByCoord(new Coord(x, y), out chunk)) {
    //       chunk.SetChunkActive(chunk.hasBiomeTexture);
    //       if(SpawnVegetation) {
    //         chunk.SetVegetationActive(chunk.hasBiomeTexture);
    //       }
    //     }
    //   }
    // }
  }

  public Chunk SetupNewChunk(Coord chunkCoord) {
    Vector2 chunkOffset = ChunkCoordToOffset(chunkCoord);
    Vector3 chunkPosition = new Vector3(chunkOffset.x, 0, chunkOffset.y) * Tilesize;

    Chunk newChunk = CreateChunk(chunkCoord, chunkOffset, chunkPosition);

    chunksManager.AddChunk(chunkCoord, newChunk);

    if(buildMode == BuildMode.Map) {
      newChunk.CreateFlatMesh(ChunkMapSize, Tilesize);
      
    } else {
      newChunk.CreateMesh(Tilesize, worldSettings.heightMultiplier, worldSettings.meshHeightCurve);
    }

    if(!endlessTerrain) {
      newChunk.SetChunkActive(true);
    }
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

  // void ClearWorld() {
  //   int childCount = chunkHolder.childCount;
  //   for(int i = 0; i < childCount; i++) {
  //     Destroy(chunkHolder.GetChild(i).gameObject);
  //   }
  //   chunksManager.ClearChunks();
  // }

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



  // delete and rebuild world when R key is pressed
  // void Update() {
  //   if(Input.GetKeyDown(KeyCode.R)) {
  //     ClearWorld();
  //     GenerateWorld();
  //   }
  // }

}

public enum MapToDraw {Biome, Height, Temperature, Humidity}