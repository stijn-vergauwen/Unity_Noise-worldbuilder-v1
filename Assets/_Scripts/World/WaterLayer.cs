using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayer : MonoBehaviour
{
  [SerializeField] WorldBuilder worldBuilder;
  [SerializeField] MeshFilter waterLayerPrefab;

  [Header("Settings")]
  [SerializeField] bool simulateWater;
  [SerializeField] int waterLodLevel;
  [SerializeField] float seaLevel = 0;
  [SerializeField] float waveHeight = 1;
  [SerializeField, Min(.001f)] float noiseScale = .09f;
  [SerializeField] float timeScale = .3f;

  public bool SimulateWater => simulateWater;

  float xOffset;
  float yOffset;

  public MeshFilter CreateChunkWaterLayer(Chunk chunk) {
    Vector3 waterLayerPosition = new Vector3(chunk.transform.position.x, seaLevel, chunk.transform.position.z);
    MeshFilter newWaterLayer = Instantiate(waterLayerPrefab, waterLayerPosition, Quaternion.identity, chunk.transform);
    Mesh waveMesh = MeshGenerator.GenerateWaterMesh(worldBuilder.ChunkMapSize, worldBuilder.Tilesize, waterLodLevel).CreateMesh(false);
    newWaterLayer.mesh = waveMesh;
    return newWaterLayer;
  }

  public Vector3[] CalculateNewMeshVertices(Vector3[] vertices, Coord chunkCoord) {
    Vector2 chunkOffset = worldBuilder.ChunkCoordToOffset(chunkCoord);
    // print("vertex: " + vertices[20]);
    for(int i = 0; i < vertices.Length; i++) {
      vertices[i].y = CalculateHeight(vertices[i].x, vertices[i].z, chunkOffset) * waveHeight;
    }
    return vertices;
  }

  float CalculateHeight(float x, float y, Vector2 chunkOffset) {
    float sampleX = (x + chunkOffset.x) / noiseScale + xOffset;
    float sampleY = (y + chunkOffset.y) / noiseScale + yOffset;

    // if(y == 9.5f) {
    //   if(x == 9.5f) {
    //     print("end of chunk, sampleX: " + sampleX);
    //   }

    //   if(x == -10.5f) {
    //     print("start of chunk, sampleX: " + sampleX);
    //   }
    // }

    return Mathf.PerlinNoise(sampleX, sampleY);
  }

  void Update() {
    UpdateOffset();
  }

  void UpdateOffset() {
    float offsetChange = Time.deltaTime * timeScale;
    xOffset += offsetChange;
    yOffset += offsetChange;
  }
}
