using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLayer : MonoBehaviour
{
  [SerializeField] WorldBuilder worldBuilder;
  [SerializeField] MeshFilter waterLayerPrefab;

  [Header("Settings")]
  [SerializeField] int waterLodLevel;
  [SerializeField] float waveHeight = 1;
  [SerializeField] float noiseScale = .09f;
  [SerializeField] float timeScale = .3f;

  float xOffset;
  float yOffset;

  public MeshFilter CreateChunkWaterLayer(Chunk chunk) {
    MeshFilter newWaterLayer = Instantiate(waterLayerPrefab, chunk.transform.position, Quaternion.identity, chunk.transform);
    Mesh waveMesh = MeshGenerator.GenerateWaterMesh(worldBuilder.ChunkMapSize, worldBuilder.Tilesize, waterLodLevel).CreateMesh(false);
    newWaterLayer.mesh = waveMesh;
    return newWaterLayer;
  }

  public Vector3[] CalculateNewMeshVertices(Vector3[] vertices) {
    for(int i = 0; i < vertices.Length; i++) {
      vertices[i].y = CalculateHeight(vertices[i].x, vertices[i].z) * waveHeight;
    }
    return vertices;
  }

  float CalculateHeight(float x, float y) {
    float sampleX = x * noiseScale + xOffset;
    float sampleY = y * noiseScale + yOffset;
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
