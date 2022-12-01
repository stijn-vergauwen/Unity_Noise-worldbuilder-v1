using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegitationPlacer : MonoBehaviour
{
  [SerializeField] WorldBuilder worldBuilder;

  [Header("Placement settings")]
  [SerializeField, Range(0, 1)] float maxOffset = 1;

  public void PlaceVegitationProps(Chunk chunk, int chunkSize, BiomeSetSO biomeSet) {
    if(!chunk.hasVegitation) {
      chunk.SetVegitation(GenerateVegitationData(
        chunk.biomeMap,
        chunkSize,
        biomeSet
      ));
    }

    VegitationInChunk[] vegitationData = chunk.vegitationInChunk;

    foreach(VegitationInChunk vegitation in vegitationData) {
      PlaceVegitationInChunk(chunk, vegitation);
    }
  }

  void PlaceVegitationInChunk(Chunk chunk, VegitationInChunk vegitation) {
    Vector3 tilePosition = worldBuilder.CoordToPosition(chunk.chunkCoord, vegitation.tileCoord);

    Instantiate(
      vegitation.prefab,
      tilePosition + vegitation.posOffset,
      Quaternion.AngleAxis(vegitation.angle, Vector3.down),
      chunk.PropHolder
    );
  }

  VegitationInChunk[] GenerateVegitationData(int[,] biomeMap, int chunkSize, BiomeSetSO biomeSet) {
    List<VegitationInChunk> vegitationData = new List<VegitationInChunk>();

    for(int y = 0; y < chunkSize; y++) {
      for(int x = 0; x < chunkSize; x++) {
        BiomeSO biome = biomeSet.FindBiome(biomeMap[x,y]);

        if(biome.HasAllowedVegitation() && Random.Range(0, 1000) < biome.vegitationDensity) {
          Coord tileCoord = new Coord(x, y);
          Vector3 posOffset = new Vector3(Random.Range(0, 1f), 0, Random.Range(0, 1f)) * worldBuilder.Tilesize * maxOffset;
          int angle = Random.Range(0, 360);
          GameObject propPrefab = biome.GetRandomVegitation();

          VegitationInChunk newVegitation = new VegitationInChunk(tileCoord, posOffset, angle, propPrefab);
          vegitationData.Add(newVegitation);
        }
      }
    }
    return vegitationData.ToArray();
  }
}
