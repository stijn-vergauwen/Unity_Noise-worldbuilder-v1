using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/BiomeSet")]
public class BiomeSetSO : ScriptableObject
{
  // holds a set of biomes
  [SerializeField] BiomeSO[] biomes;

  [Header("fallback if no suitable biome found")]
  [SerializeField] BiomeSO debugBiome;

  public BiomeSO[] GetBiomes() {
    return biomes;
  }

  public BiomeSO FindBiome(int biomeId) {
    foreach(BiomeSO biome in biomes) {
      if(biome.biomeId == biomeId) return biome;
    }
    return debugBiome;
  }
}
