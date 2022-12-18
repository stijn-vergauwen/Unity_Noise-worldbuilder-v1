using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
  
}

// Data structs

public struct Coord {
  public int x;
  public int y;

  public Coord(int x, int y) {
    this.x = x;
    this.y = y;
  }

  public Coord AddOffset(int xOffset, int yOffset) {
    return new Coord(x + xOffset, y + yOffset);
  }

  public Coord AddOffset(Coord offset) {
    return AddOffset(offset.x, offset.y);
  }

  public static bool operator ==(Coord a, Coord b) {
    return a.x == b.x && a.y == b.y;
  }

  public static bool operator !=(Coord a, Coord b) {
    return !(a.x == b.x && a.y == b.y);
  }

  public override bool Equals(object obj)
  {
    return base.Equals(obj);
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }
}

[System.Serializable]
public struct MinMax {
  public float min;
  public float max;

  public MinMax(float min, float max) {
    this.min = min;
    this.max = max;
  }

  public bool CheckValue(float value) {
    return (value >= min && value <= max);
  }

  public float GetCenterValue() {
    return min + (max - min * .5f);
  }

  public float GetProximityToCenter(float value) {
    float center = GetCenterValue();
    float diffToCenter = Mathf.Abs(center - value);
    float result = 1 - diffToCenter / (center - min);

    return result;
  }
}

// Data container structs

[System.Serializable]
public struct WorldSettings {
  [Header("General")]
  public float tileSize;
  public int chunkSize;
  public int worldSize;

  [Header("NoiseMap generation")]
  public NoiseSettings heightMap;
  public NoiseSettings temperatureMap;
  public NoiseSettings humidityMap;

  [Header("Biomes")]
  public BiomeSetSO biomeSet;

  [Header("Ground mesh")]
  public float heightMultiplier;
  public AnimationCurve meshHeightCurve;
}

[System.Serializable]
public struct NoiseSettings {
  public int seed;
  public Vector2 offset;

  public float scale;

  public int octaves;
  public float persistance;
  public float lacunarity;
}

public struct LocalCoord {
  public Coord chunkCoord;
  public Coord tileCoord;

  public LocalCoord(Coord chunkCoord, Coord tileCoord) {
    this.chunkCoord = chunkCoord;
    this.tileCoord = tileCoord;
  }
}