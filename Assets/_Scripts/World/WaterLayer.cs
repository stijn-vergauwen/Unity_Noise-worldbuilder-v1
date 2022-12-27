using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayer : MonoBehaviour
{
  [SerializeField] WorldBuilder worldBuilder;
  [SerializeField] ChunksManager chunksManager;
  [SerializeField] MeshFilter waterLayerPrefab;
  [SerializeField] GameSettingsSO gameSettingsData;

  [Header("Settings")]
  [SerializeField] bool simulateWater;
  [SerializeField] int waterLodLevel;
  [SerializeField] float seaLevel = 0;
  [SerializeField] float waveHeight = 1;
  [SerializeField, Min(.001f)] float noiseScale = .09f;
  [SerializeField] float timeScale = .3f;

  float xOffset;
  float yOffset;

  private void Start() {
    if(worldBuilder.useGameSettingsData) {
      GetGameSettingsData();
    }
  }

  void GetGameSettingsData() {
    simulateWater = gameSettingsData.simulateWater;
  }


  public bool CheckIfWaterInChunk(int[,] biomeMap) {
    int mapSize = biomeMap.GetLength(0);
    int oceanBiomeId = worldBuilder.BiomeSet.FindBiome("Ocean").biomeId;

    for(int y = 0; y < mapSize; y++) {
      for(int x = 0; x < mapSize; x++) {
        if(biomeMap[x,y] == oceanBiomeId) return true;
      }
    }
    return false;
  }

  public MeshFilter CreateChunkWaterLayer(Chunk chunk) {
    Vector3 waterLayerPosition = new Vector3(chunk.transform.position.x, seaLevel, chunk.transform.position.z);
    MeshFilter newWaterLayer = Instantiate(waterLayerPrefab, waterLayerPosition, Quaternion.identity, chunk.transform);
    Mesh waveMesh = MeshGenerator.GenerateWaterMesh(worldBuilder.ChunkMapSize, worldBuilder.Tilesize, waterLodLevel).CreateMesh(false);
    newWaterLayer.mesh = waveMesh;
    return newWaterLayer;
  }

  public Vector3[] CalculateNewMeshVertices(Vector3[] vertices, Coord chunkCoord) {
    Vector2 chunkOffset = worldBuilder.ChunkCoordToOffset(chunkCoord);
    for(int i = 0; i < vertices.Length; i++) {
      vertices[i].y = CalculateHeight(vertices[i].x, vertices[i].z, chunkOffset) * waveHeight;
    }
    return vertices;
  }

  float CalculateHeight(float x, float y, Vector2 chunkOffset) {
    float sampleX = (x + chunkOffset.x) / noiseScale + xOffset;
    float sampleY = (y + chunkOffset.y) / noiseScale + yOffset;
    return Mathf.PerlinNoise(sampleX, sampleY) - .5f;
  }

  // Updating water 

  void Update() {
    if(simulateWater) {
      UpdateOffset();
      UpdateWater();
    }
  }

  void UpdateOffset() {
    float offsetChange = Time.deltaTime * timeScale;
    xOffset += offsetChange;
    yOffset += offsetChange;
  }

  void UpdateWater() {
    Chunk[] activeChunks = chunksManager.GetActiveChunks();
    foreach(Chunk chunk in activeChunks) {
      UpdateChunkWaterLayer(chunk);
    }
  }

  void UpdateChunkWaterLayer(Chunk chunk) {
    if(chunk.simulateChunkWater) {
      chunk.UpdateWaterVertices(CalculateNewMeshVertices(
        chunk.GetWaterVertices(),
        chunk.chunkCoord
      ));
    }
  }
}
