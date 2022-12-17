using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
  [SerializeField] Transform vegetationPropsHolder;
  [SerializeField] GameObject meshHolder;
  
  public Transform PropHolder => vegetationPropsHolder;

  public Coord chunkCoord {get; private set;}

  // data maps
  float[,] heightMap;
  float[,] temperatureMap;
  float[,] humidityMap;
  public int[,] biomeMap {get; private set;}

  public bool hasVegetation {get; private set;} = false;
  public bool hasVegetationData {get; private set;} = false;
  public VegetationInChunk[] vegetationInChunk {get; private set;}

  public bool hasBiomeTexture {get; private set;} = false;
  Texture2D biomeTexture;

  ChunksManager manager;
  MeshFilter meshFilter;
  MeshRenderer meshRenderer;
  MeshCollider meshCollider;

  // water layer mesh
  public bool hasWaterLayer {get; private set;} = false;
  public bool simulateChunkWater {get; private set;} = false;
  MeshFilter waterLayerMeshFilter;

  public void Init(ChunksManager chunksManager, Coord chunkCoord, float[,] heightMap, float[,] temperatureMap, float[,] humidityMap) {
    manager = chunksManager;
    this.chunkCoord = chunkCoord;
    this.heightMap = heightMap;
    this.temperatureMap = temperatureMap;
    this.humidityMap = humidityMap;

    meshFilter = meshHolder.GetComponent<MeshFilter>();
    meshRenderer = meshHolder.GetComponent<MeshRenderer>();
    meshCollider = meshHolder.GetComponent<MeshCollider>();

    meshRenderer.material.SetFloat("_Glossiness", 0);
    CreateBiomeMap();
    manager.TryCreateChunkWaterLayer(this);
  }

  // accessors
  public float GetHeightMapAtCoord(Coord tileCoord) {
    return heightMap[tileCoord.x, tileCoord.y];
  }

  public int GetBiomeIdAtCoord(Coord tileCoord) {
    return biomeMap[tileCoord.x, tileCoord.y];
  }

  // creating & drawing map data
  void CreateBiomeMap() {
    biomeMap = BiomeGenerator.GenerateBiomeMap(heightMap, temperatureMap, humidityMap, manager.GetBiomeSet());
  }

  public void CreateFlatMesh(int meshSize, float tileSize) {
    MeshData meshData = MeshGenerator.GenerateFlatMesh(meshSize, tileSize);

    meshFilter.mesh = meshData.CreateMesh(true);
  }

  public void CreateMesh(float tileSize, float heightMultiplier, AnimationCurve heightCurve) {
    MeshData meshData = MeshGenerator.GenerateMesh(heightMap, tileSize, heightMultiplier, heightCurve);
    Mesh mesh = meshData.CreateMesh(true);
    meshFilter.mesh = mesh;
    meshCollider.sharedMesh = mesh;
  }

  public void DrawMap(MapToDraw mapToDraw) {
    if(mapToDraw == MapToDraw.Biome) {
      if(hasBiomeTexture) {
        meshRenderer.material.mainTexture = biomeTexture;
      }

    } else if(mapToDraw == MapToDraw.Height) {
      meshRenderer.material.mainTexture = TextureGenerator.GenerateFromNoiseMap(heightMap);

    } else if(mapToDraw == MapToDraw.Temperature) {
      meshRenderer.material.mainTexture = TextureGenerator.GenerateFromNoiseMap(temperatureMap, Color.blue, Color.red);

    } else if(mapToDraw == MapToDraw.Humidity) {
      meshRenderer.material.mainTexture = TextureGenerator.GenerateFromNoiseMap(humidityMap, Color.white, Color.blue);
    }
  }

  public void SetTerrain(Color[] groundColorArray) {
    biomeTexture = TextureGenerator.GenerateFromColorArray(
      biomeMap.GetLength(0),
      groundColorArray
    );
    // maybe set meshRenderer to this texture
    hasBiomeTexture = true;
  }

  public void SetVegetation(VegetationInChunk[] vegetation) {
    vegetationInChunk = vegetation;
    hasVegetationData = true;
  }

  public void SetWaterLayer(MeshFilter meshFilter) {
    waterLayerMeshFilter = meshFilter;
    hasWaterLayer = true;
  }

  // chunk updates

  public void DeactivateChunk() {
    ToggleTerrain(false);
    ToggleVegetation(false);
    ToggleWater(false);
    simulateChunkWater = false;
    DestroyVegetation();
  }

  public void ToggleTerrain(bool isVisible) {
    if(hasBiomeTexture && meshHolder.activeSelf != isVisible) {
      meshHolder.SetActive(isVisible);
    }
  }

  public void ToggleVegetation(bool isVisible) {
    if(hasVegetationData && vegetationPropsHolder.gameObject.activeSelf != isVisible) {
      vegetationPropsHolder.gameObject.SetActive(isVisible);
      
      if(isVisible) {
        hasVegetation = true;
      }
    }
  }

  public void ToggleWater(bool isVisible) {
    if(hasWaterLayer && waterLayerMeshFilter.gameObject.activeSelf != isVisible) {
      waterLayerMeshFilter.gameObject.SetActive(isVisible);
    }
  }

  public void ToggleWaterSimulation(bool simulate) {
    if(hasWaterLayer && simulateChunkWater != simulate) {
      simulateChunkWater = simulate;

      if(!simulate) {
        ResetWaterVertices();
      }
    }
  }

  void DestroyVegetation() {
    foreach(Transform prop in vegetationPropsHolder) {
      Destroy(prop.gameObject);
    }
    hasVegetation = false;
  }

  // water updates

  public Vector3[] GetWaterVertices() {
    return waterLayerMeshFilter.mesh.vertices; // TODO: this allocates garbage every frame, use GetVertices() if you want to optimize
  }

  public void UpdateWaterVertices(Vector3[] newVertices) {
    waterLayerMeshFilter.mesh.vertices = newVertices;
  }

  void ResetWaterVertices() {
    Vector3[] vertices = GetWaterVertices();
    for(int i = 0; i < vertices.Length; i++) {
        vertices[i].y = 0;
    }
    UpdateWaterVertices(vertices);
  }
}