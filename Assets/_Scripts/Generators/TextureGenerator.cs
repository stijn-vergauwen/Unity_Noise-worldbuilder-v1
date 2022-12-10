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
}
