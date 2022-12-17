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

  public static Action OnStartareaLoaded;

  // setup
  public void StartUpdator() {
    terrainUpdator.StartUpdator();
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
      AddChunk(chunkCoord, chunk);
    }
    return chunk;
  }

  void ClearChunks() {
    chunks.Clear();
  }

  public Chunk[] GetActiveChunks() {
    return activeChunks.ToArray();
  }

  public void AddActiveChunk(Chunk chunk) {
    activeChunks.Add(chunk);
  }

  public void RemoveActiveChunk(Chunk chunk) {
    activeChunks.Remove(chunk);
  }

  public void ClearActiveChunks() {
    DeactivateAllActiveChunks();
    activeChunks.Clear();
  }

  public void DeactivateAllActiveChunks() {
    foreach(Chunk chunk in activeChunks) {
      chunk.DeactivateChunk();
    }
  }

  // chunk update related things

  public void RaiseStartAreaLoaded() {
    OnStartareaLoaded?.Invoke();
  }

  public void ActivateChunk(Chunk chunk) {
    if(!chunk.hasBiomeTexture) {
      if(!CheckIfChunkCanCreateTerrain(chunk.chunkCoord)) return;

      chunk.SetTerrain(colorCalculator.CalculateGroundColors(
        chunk.chunkCoord,
        chunk.biomeMap
      ));

      chunk.DrawMap(worldBuilder.mapToDraw);
    }

    chunk.ToggleTerrain(true);
    chunk.ToggleWater(true);
  }

  public void ToggleChunkVegetation(Chunk chunk, bool value) {
    if(value && !chunk.hasVegetation) {
      CreateVegetationForChunk(chunk);
    }

    chunk.ToggleVegetation(value);
  }

  public void ToggleChunkWaterLayer(Chunk chunk, bool value) {
    chunk.ToggleWaterSimulation(value);
  }

  public void DeactivateChunkAtCoord(Coord chunkCoord) {
    Chunk chunk;
    if(TryGetChunkByCoord(chunkCoord, out chunk)) {
      chunk.DeactivateChunk();
    }
  }

  // chunk data

  void CreateVegetationForChunk(Chunk chunk) {
    vegetationPlacer.PlaceVegetationProps(chunk, worldBuilder.ChunkSize, worldBuilder.BiomeSet);
  }

  public void TryCreateChunkWaterLayer(Chunk chunk) {
    if(worldBuilder.UseWaterLayer() && waterLayer.CheckIfWaterInChunk(chunk.biomeMap)) {
      MeshFilter chunkWaterLayer = waterLayer.CreateChunkWaterLayer(chunk);
      chunk.SetWaterLayer(chunkWaterLayer);
    }
  }

  bool CheckIfChunkCanCreateTerrain(Coord chunkCoord) {
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

  public void UpdateDrawnMap() {
    foreach(Chunk chunk in chunks.Values) {
      chunk.DrawMap(worldBuilder.mapToDraw);
    }
  }

  void CheckPlayerInput() {
    if(Input.GetKeyDown(KeyCode.M)) {
      NextMapToDraw();
      UpdateDrawnMap();
    }
  }

  void NextMapToDraw() {
    if(worldBuilder.mapToDraw == MapToDraw.Humidity) {
      worldBuilder.mapToDraw = 0;

    } else {
      worldBuilder.mapToDraw++;
    }
  }
  
  public BiomeSetSO GetBiomeSet() {
    return worldBuilder.BiomeSet;
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
