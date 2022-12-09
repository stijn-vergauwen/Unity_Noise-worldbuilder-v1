using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
  [SerializeField] Transform vegitationPropsHolder;
  [SerializeField] GameObject meshHolder;
  
  public Transform PropHolder => vegitationPropsHolder;

  public Coord chunkCoord {get; private set;}

  // data maps
  float[,] heightMap;
  float[,] temperatureMap;
  float[,] humidityMap;
  public int[,] biomeMap {get; private set;}

  public bool hasVegitation {get; private set;} = false;
  public VegitationInChunk[] vegitationInChunk {get; private set;}

  Texture2D biomeTexture;
  bool hasBiomeTexture = false;

  ChunksManager manager;
  MeshFilter meshFilter;
  MeshRenderer meshRenderer;
  MeshCollider meshCollider;

  // water layer mesh
  public bool hasWaterLayer {get; private set;} = false;
  public MeshFilter waterLayerMeshFilter {get; private set;} // TODO: make sure water layers get disabled when out of range

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
    manager.CheckIfWaterInChunk(this);
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

  public void OnDrawMap(ChunksManager.MapToDraw mapToDraw) {
    if(mapToDraw == ChunksManager.MapToDraw.Biome) {
      if(hasBiomeTexture) {
        meshRenderer.material.mainTexture = biomeTexture;
      }

    } else if(mapToDraw == ChunksManager.MapToDraw.Height) {
      meshRenderer.material.mainTexture = TextureGenerator.GenerateFromNoiseMap(heightMap);

    } else if(mapToDraw == ChunksManager.MapToDraw.Temperature) {
      meshRenderer.material.mainTexture = TextureGenerator.GenerateFromNoiseMap(temperatureMap, Color.blue, Color.red);

    } else if(mapToDraw == ChunksManager.MapToDraw.Humidity) {
      meshRenderer.material.mainTexture = TextureGenerator.GenerateFromNoiseMap(humidityMap, Color.white, Color.blue);
    }
  }

  public void SetVegitation(VegitationInChunk[] vegitation) {
    vegitationInChunk = vegitation;
    hasVegitation = true;
  }

  public void SetWaterLayer(MeshFilter meshFilter) {
    hasWaterLayer = true;
    waterLayerMeshFilter = meshFilter;
  }

  // biome texture

  void TryCreateBiomeTexture() {
    if(manager.CheckIfChunkCanCreateTerrain(chunkCoord)) {
      biomeTexture = TextureGenerator.GenerateFromColorArray(
        biomeMap.GetLength(0),
        manager.CreateAveragedChunkGroundColors(chunkCoord, biomeMap)
      );
      meshRenderer.material.mainTexture = biomeTexture;
      hasBiomeTexture = true;
    }
  }

  void CheckBiomeTexture(ChunksManager.MapToDraw mapToDraw) {
    if(mapToDraw == ChunksManager.MapToDraw.Biome && !hasBiomeTexture) {
      TryCreateBiomeTexture();
    }
  }

  // chunk updates
  public void SetChunkActive(bool value) {
    if(meshHolder.activeSelf != value) {
      meshHolder.SetActive(value);
      if(hasWaterLayer) {
        waterLayerMeshFilter.gameObject.SetActive(value);
      }

      if(value) {
        manager.OnDrawMap += OnDrawMap;
        manager.OnVisibleChunksUpdate += CheckBiomeTexture;

      } else {
        manager.OnDrawMap -= OnDrawMap;
        manager.OnVisibleChunksUpdate -= CheckBiomeTexture;
      }
    }
  }

  public void SetVegitationActive(bool value) {
    if(vegitationPropsHolder.gameObject.activeSelf != value) {
      if(value && !hasVegitation) {
        manager.CreateVegitationForChunk(this);
      }
      vegitationPropsHolder.gameObject.SetActive(value);
    }
  }


  // THIS IS REALLY TEMPORARY THOUGH
  // TODO: remove this update function, this should move to ChunksManager, but ChunksManager & WorldBuilder need some reworks & refactoring!

  void Update() {
    if(hasWaterLayer) {
      manager.UpdateChunkWaterLayer(waterLayerMeshFilter, chunkCoord);
    }
  }
}

public struct VegitationInChunk {
  public Coord tileCoord {get; private set;}
  public Vector3 posOffset {get; private set;}
  public int angle {get; private set;}
  public GameObject prefab {get; private set;}

  public VegitationInChunk(Coord tileCoord, Vector3 posOffset, int angle, GameObject prefab) {
    this.tileCoord = tileCoord;
    this.posOffset = posOffset;
    this.angle = angle;
    this.prefab = prefab;
  }
}