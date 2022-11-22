using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator
{
  // create textures for ground meshes

  public static Texture2D GenerateFromColorArray(int size, Color[] colorArray) {
    Texture2D texture = new Texture2D(size, size);
    texture.filterMode = FilterMode.Point;
    texture.wrapMode = TextureWrapMode.Clamp;

    texture.SetPixels(colorArray);
    texture.Apply();

    return texture;
  }

  // from noise map
  public static Texture2D GenerateFromNoiseMap(float[,] noiseMap, Color colorA, Color colorB) {
    int size = noiseMap.GetLength(0);
    Color[] colorArray = new Color[size * size];

    for (int y = 0; y < size; y++) {
      for (int x = 0; x < size; x++) {
        colorArray[y * size + x] = Color.Lerp(colorA, colorB, noiseMap[x,y]);
      }
    }

    return GenerateFromColorArray(size, colorArray);
  }

  public static Texture2D GenerateFromNoiseMap(float[,] noiseMap) {
    return GenerateFromNoiseMap(noiseMap, Color.black, Color.white);
  }

  // from biome map
  public static Texture2D GenerateFromBiomeMap(int[,] biomeMap, BiomeSetSO biomeSet) {
    int size = biomeMap.GetLength(0);
    Color[] colorArray = new Color[size * size];

    // apply biome color to array
    for (int y = 0; y < size; y++) {
      for (int x = 0; x < size; x++) {
        colorArray[y * size + x] = biomeSet.FindBiome(biomeMap[x,y]).groundColor;

        // to try biome blending, i think i could make a function that gets the colors from biomes around the current tile, and returns the average
        // then set the ground color to that average? i think that's pretty doable actually

        // colorArray[y * size + x] = GetAverageGroundColor(x, y, 2, biomeMap, biomeSet);
      }
    }

    return GenerateFromColorArray(size, colorArray);
  }

  static Color GetAverageGroundColor(int xPos, int yPos, int searchRadius, int[,] biomeMap, BiomeSetSO biomeSet) {

    // getting an average like this works, there shouldn't be a transition to water but ocean biome is temporary like this anyway
    // also it should somehow search across chunk borders.
    // maybe first check if chunks exists all around current one, then only calculate averages if that's the case,
    // the player will be too far away anyway to see non blended ground.




    int size = biomeMap.GetLength(0);
    float totalR = 0;
    float totalG = 0;
    float totalB = 0;
    int sampleCount = 0;
    
    for (int yOffset = -searchRadius; yOffset <= searchRadius; yOffset++) {
      for (int xOffset = -searchRadius; xOffset <= searchRadius; xOffset++) {
        int x = xPos + xOffset;
        int y = yPos + yOffset;

        if(x >= 0 && x < size && y >= 0 && y < size) {
          Color sample = biomeSet.FindBiome(biomeMap[x, y]).groundColor;
          totalR += sample.r;
          totalG += sample.g;
          totalB += sample.b;
          sampleCount++;
        }
      }
    }

    return new Color(
      totalR / sampleCount,
      totalG / sampleCount,
      totalB / sampleCount
    );
  }
}
