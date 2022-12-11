using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksManager : MonoBehaviour
{
  [SerializeField] WorldBuilder worldBuilder;
  [SerializeField] VegetationPlacer vegetationPlacer;
  [SerializeField] WaterLayer waterLayer;
  [SerializeField] GroundColorCalculator colorCalculator;
  [SerializeField] TerrainUpdator terrainUpdator;

  Dictionary<Coord, Chunk> chunks = new Dictionary<Coord, Chunk>();

  List<Chunk> activeChunks = new List<Chunk>();

  // TODO: make a list<> holding all current active chunks, update this each time UpdateVisibleChunks is called, saves checks & scales better than chunkGenerateRadius
  // also need to use that list to update water layers of active chunks with water



  public Action<MapToDraw> OnDrawMap;
  public Action<MapToDraw> OnVisibleChunksUpdate;

  public static Action OnStartareaLoaded;

  // setup
  void Start() {
    if(worldBuilder.endlessTerrain) {
      terrainUpdator.StartUpdator();
      RaiseStartAreaLoaded();
    }
  }

  void Update() {
    // CheckPlayerInput(); // only for testing
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

  public Chunk GetOrCreateChunk(Coord chunkCoord) {
    Chunk chunk;
    if(!TryGetChunkByCoord(chunkCoord, out chunk)) {
      chunk = worldBuilder.SetupNewChunk(chunkCoord);
      chunk.OnDrawMap(worldBuilder.mapToDraw);
    }
    return chunk;
  }

  public void ClearChunks() {
    chunks.Clear();
  }

  public void AddActiveChunk(Chunk chunk) {
    activeChunks.Add(chunk);
  }

  public void ClearActiveChunks() {
    activeChunks.Clear();
  }

  public void HideAllActiveChunks() {
    foreach(Chunk chunk in activeChunks) {
      chunk.SetChunkActive(false);
    }
  }

  // chunk update related things

  public void RaiseStartAreaLoaded() {
    OnStartareaLoaded?.Invoke();
  }

  public void RaiseOnVisibleChunksUpdate() {
    OnVisibleChunksUpdate?.Invoke(worldBuilder.mapToDraw);
  }

  // chunk class communication
  
  public BiomeSetSO GetBiomeSet() {
    return worldBuilder.BiomeSet;
  }

  public void CreateVegetationForChunk(Chunk chunk) {
    vegetationPlacer.PlaceVegetationProps(chunk, worldBuilder.ChunkSize, worldBuilder.BiomeSet);
  }

  public bool CheckIfChunkCanCreateTerrain(Coord chunkCoord) {
    return GetChunkNeighborCount(chunkCoord) == 8;
  }

  public Color[] CalculateGroundColors(Coord chunkCoord, int[,] biomeMap) {
    return colorCalculator.CalculateGroundColors(chunkCoord, biomeMap);
  }

  // water layer related

  public void CheckIfWaterInChunk(Chunk chunk) {
    if(waterLayer.CheckIfWaterInChunk(chunk.biomeMap)) {
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

  Coord[] GetSurroundingCoordOffsets() {
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
    OnDrawMap?.Invoke(worldBuilder.mapToDraw);
  }

  void CheckPlayerInput() {
    if(Input.GetKeyDown(KeyCode.M)) {
      NextMapToDraw();
      DisplayMap();
    }
  }

  void NextMapToDraw() {
    if(worldBuilder.mapToDraw == MapToDraw.Humidity) {
      worldBuilder.mapToDraw = 0;

    } else {
      worldBuilder.mapToDraw++;
    }
  }

  // void OnDrawGizmos() {
  //   Gizmos.color = Color.green;
  //   foreach(Chunk chunk in chunks.Values) {
  //     Vector3 chunkposition = worldBuilder.CoordToFlatPosition(worldBuilder.LocalToWorldCoord(chunk.chunkCoord, new Coord(0, 0)));
  //     Vector3 halfChunkSize = new Vector3(.5f, 0, .5f) * worldBuilder.ChunkSize;
  //     Gizmos.DrawSphere(chunkposition + halfChunkSize, 2);
  //   }
  // }
}
