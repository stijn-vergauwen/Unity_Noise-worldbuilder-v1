using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationPlacer : MonoBehaviour
{
  [SerializeField] WorldBuilder worldBuilder;
  [SerializeField] LayerMask terrainMask;

  [Header("Placement settings")]
  [SerializeField, Range(0, 1)] float maxOffset = 1;

  public void PlaceVegetationProps(Chunk chunk, int chunkSize, BiomeSetSO biomeSet) {
    if(!chunk.hasVegetationData) {
      chunk.SetVegetation(GenerateVegetationData(
        chunk.biomeMap,
        chunkSize,
        biomeSet
      ));
    }

    foreach(VegetationInChunk vegetation in chunk.vegetationInChunk) {
      PlaceVegetationInChunk(chunk, vegetation);
    }
  }

  void PlaceVegetationInChunk(Chunk chunk, VegetationInChunk vegetation) {
    Vector3 propPosition = worldBuilder.CoordToPosition(chunk.chunkCoord, vegetation.tileCoord) + vegetation.posOffset;

    if(vegetation.placeWithRaycast) {
      RaycastHit hit;
      Vector3 raisedPosition = propPosition + Vector3.up * 2;
      float rayDistance = 2;
      if (Physics.Raycast(raisedPosition, Vector3.down, out hit, rayDistance, terrainMask)) {
        propPosition.y = hit.point.y;
      }
    }

    Instantiate(
      vegetation.prefab,
      propPosition,
      Quaternion.AngleAxis(vegetation.angle, Vector3.down),
      chunk.PropHolder
    );
  }

  VegetationInChunk[] GenerateVegetationData(int[,] biomeMap, int chunkSize, BiomeSetSO biomeSet) {
    List<VegetationInChunk> vegetationData = new List<VegetationInChunk>();

    for(int y = 0; y < chunkSize; y++) {
      for(int x = 0; x < chunkSize; x++) {
        BiomeSO biome = biomeSet.FindBiome(biomeMap[x,y]);

        if(biome.HasAllowedVegetation() && Random.Range(0, 1000) < biome.vegetationDensity) {
          Coord tileCoord = new Coord(x, y);
          Vector3 posOffset = new Vector3(Random.Range(0, 1f), 0, Random.Range(0, 1f)) * worldBuilder.Tilesize * maxOffset;
          int angle = Random.Range(0, 360);
          bool placeWithRaycast;
          GameObject propPrefab = biome.GetRandomVegetation(out placeWithRaycast);

          VegetationInChunk newVegetation = new VegetationInChunk(tileCoord, posOffset, angle, placeWithRaycast, propPrefab);
          vegetationData.Add(newVegetation);
        }
      }
    }
    return vegetationData.ToArray();
  }
}

public struct VegetationInChunk {
  public Coord tileCoord {get; private set;}
  public Vector3 posOffset {get; private set;}
  public int angle {get; private set;}
  public bool placeWithRaycast {get; private set;}
  public GameObject prefab {get; private set;}

  public VegetationInChunk(Coord tileCoord, Vector3 posOffset, int angle, bool placeWithRaycast, GameObject prefab) {
    this.tileCoord = tileCoord;
    this.posOffset = posOffset;
    this.angle = angle;
    this.placeWithRaycast = placeWithRaycast;
    this.prefab = prefab;
  }
}