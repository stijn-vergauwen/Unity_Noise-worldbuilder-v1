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
    if(!chunk.hasVegetation) {
      chunk.SetVegetation(GenerateVegetationData(
        chunk.biomeMap,
        chunkSize,
        biomeSet
      ));
    }

    VegetationInChunk[] vegetationData = chunk.vegetationInChunk;

    foreach(VegetationInChunk vegetation in vegetationData) {
      PlaceVegetationInChunk(chunk, vegetation);
    }
  }

  void PlaceVegetationInChunk(Chunk chunk, VegetationInChunk vegetation) {
    Vector3 propPosition = worldBuilder.CoordToPosition(chunk.chunkCoord, vegetation.tileCoord) + vegetation.posOffset;

    if(vegetation.placeWithRaycast) {
      RaycastHit hit;
      Vector3 raisedPosition = propPosition + Vector3.up;
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
