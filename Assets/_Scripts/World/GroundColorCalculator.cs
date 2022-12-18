using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundColorCalculator : MonoBehaviour
{
  [SerializeField] ChunksManager chunksManager;
  [SerializeField] WorldBuilder worldBuilder;

  public Color[] CalculateGroundColors(Coord chunkCoord, int[,] biomeMap) {
    int size = biomeMap.GetLength(0);
    BiomeSetSO biomeSet = worldBuilder.BiomeSet;
    Color[] colorArray = new Color[size * size];

    for (int y = 0; y < size; y++) {
      for (int x = 0; x < size; x++) {
        LocalCoord localCoord = new LocalCoord(chunkCoord, new Coord(x, y));
        colorArray[y * size + x] = GetAverageGroundColor(localCoord, worldBuilder.BiomeTransitionSmoothness, biomeMap, biomeSet);
      }
    }

    return colorArray;
  }

  Color GetAverageGroundColor(LocalCoord localCoord, int searchRadius, int[,] biomeMap, BiomeSetSO biomeSet) {
    int size = biomeMap.GetLength(0);
    float totalR = 0;
    float totalG = 0;
    float totalB = 0;
    int sampleCount = 0;
    
    for (int yOffset = -searchRadius; yOffset <= searchRadius; yOffset++) {
      for (int xOffset = -searchRadius; xOffset <= searchRadius; xOffset++) {
        Coord searchCoord = localCoord.tileCoord.AddOffset(xOffset, yOffset);
        int biomeId;
        // check if coord is within chunk, if in different chunk, get biome from other chunk
        if(searchCoord.x >= 0 && searchCoord.x < size && searchCoord.y >= 0 && searchCoord.y < size) {
          biomeId = biomeMap[searchCoord.x, searchCoord.y];

        } else {
          biomeId = GetBiomeIdInChunk(worldBuilder.LocalToWorldCoord(localCoord.chunkCoord, searchCoord));
        }

        Color sample = biomeSet.FindBiome(biomeId).groundColor;
        totalR += sample.r;
        totalG += sample.g;
        totalB += sample.b;
        sampleCount++;
      }
    }

    return new Color(
      totalR / sampleCount,
      totalG / sampleCount,
      totalB / sampleCount
    );
  }

  int GetBiomeIdInChunk(Coord worldCoord) {
    LocalCoord localCoord = worldBuilder.WorldToLocalCoord(worldCoord);
    Chunk chunk;
    int biomeId = 0;

    if(chunksManager.TryGetChunkByCoord(localCoord.chunkCoord, out chunk)) {
      biomeId = chunk.GetBiomeIdAtCoord(localCoord.tileCoord);
    }

    return biomeId;
  }
}
