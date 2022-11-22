using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
  // contains perlin noise methods that the other scripts need for data generation

  public static float[,] GenerateNoiseMap(int size, NoiseSettings settings, Vector2 chunkOffset) {
    float[,] noiseMap = new float[size, size];

    System.Random prng = new System.Random(settings.seed);
    Vector2[] octaveOffsets =  new Vector2[settings.octaves];

    float maxPossibleHeight = 0;
    float amplitude = 1;
    
    // setup random octave positions, & calculate max height
    for(int i = 0; i < settings.octaves; i++) {
      float offsetX = prng.Next(-1000, 1000) + settings.offset.x + chunkOffset.x;
      float offsetY = prng.Next(-1000, 1000) - settings.offset.y + chunkOffset.y;
      octaveOffsets[i] = new Vector2(offsetX, offsetY);

      maxPossibleHeight += amplitude;
      amplitude *= settings.persistance;
    }

    // prevent negative scale setting, this causes badness
    if(settings.scale <= 0) {
      settings.scale = .0001f;
    }

    // precalculate adjustment to noise value (this is for output closer to between 0 & 1)
    float noiseValueMultiplier = 1 / (maxPossibleHeight * .75f + .2f); // i think this calculation is the closest i can get it, pretty good.
    
    // create the noise map
    for(int y = 0; y < size; y++) {
      for(int x = 0; x < size; x++) {
        noiseMap[x,y] = GetNoiseValue(new Vector2Int(x, y), settings, octaveOffsets, noiseValueMultiplier);
      }
    }

    return noiseMap;
  }

  static float GetNoiseValue(Vector2Int position, NoiseSettings settings, Vector2[] octaveOffsets, float valueMultiplier) {
    float amplitude = 1;
    float frequency = 1;
    float noiseValue = 0;

    foreach(Vector2 octaveOffset in octaveOffsets) {
      Vector2 sample = (position + octaveOffset) / settings.scale * frequency;
      float perlinValue = Mathf.PerlinNoise(sample.x, sample.y);
      noiseValue += perlinValue * amplitude;

      amplitude *= settings.persistance;
      frequency *= settings.lacunarity;
    }

    // adjust noise value by multiplier and clamp
    return Mathf.Clamp01(noiseValue * valueMultiplier);
  }
}
