using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BiomeGenerator
{
  // uses Noise class to generate temperature & humidity maps, which together creates a biome map

  public static int[,] GenerateBiomeMap(float[,] heightMap, float[,] temperatureMap, float[,] humidityMap, BiomeSetSO availableBiomeSet) {
    int size = temperatureMap.GetLength(0);
    int[,] biomeMap = new int[size, size];

    for (int y = 0; y < size; y++) {
      for (int x = 0; x < size; x++) {
        biomeMap[x,y] = FindValidBiome(heightMap[x,y], temperatureMap[x,y], humidityMap[x,y], availableBiomeSet);
      }
    }

    return biomeMap;
  }

  static int FindValidBiome(float height, float temperature, float humidity, BiomeSetSO availableBiomeSet) {
    int validId = 0;
    int bestFoundPreference = 0;

    foreach(BiomeSO biome in availableBiomeSet.GetBiomes()) {
      int biomePreference = biome.GetConditionPreference(height, temperature, humidity);

      if(biomePreference > bestFoundPreference) {
        validId = biome.biomeId;
        bestFoundPreference = biomePreference;
      }
    }

    if(validId == 0) {
      Debug.LogError($"Failed to find biome! given height: {height}, temperature: {temperature}, humidity: {humidity}");
    }

    return validId;
  }
}
