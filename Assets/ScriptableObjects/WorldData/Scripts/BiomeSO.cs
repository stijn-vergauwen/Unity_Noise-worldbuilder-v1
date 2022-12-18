using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/BiomeData")]
public class BiomeSO : ScriptableObject
{
  public string biomeName;
  public int biomeId;
  public Color groundColor;

  [Header("required conditions (0 to 1)")]
  public MinMax height;
  public MinMax temperature;
  public MinMax humidity;

  [Header("how likely this biome spawns")]
  [Range(0, 100)] public int biomePriority = 100;

  [Header("Vegetation props")]
  public VegetationSetSO[] allowedVegetation;
  [Range(0, 1000)] public int vegetationDensity = 100;

  // calculates how ideal the given conditions are for this biome.
  public int GetConditionPreference(float givenHeight, float givenTemperature, float givenHumidity) {
    int preference = 0;

    if(height.CheckValue(givenHeight) &&
      temperature.CheckValue(givenTemperature) &&
      humidity.CheckValue(givenHumidity)
    ) {
      // set preference to average proximity to minmax center scaled from 0 to biomePriority
      float averageProximity = (height.GetProximityToCenter(givenHeight) + temperature.GetProximityToCenter(givenTemperature) + humidity.GetProximityToCenter(givenHumidity)) / 3;
      preference = Mathf.RoundToInt(Mathf.Lerp(0, biomePriority, averageProximity));
    }

    return preference;
  }

  public bool HasAllowedVegetation() {
    return allowedVegetation.Length > 0;
  }

  public GameObject GetRandomVegetation(out bool placeWithRaycast) {
    VegetationSetSO vegetationSet = allowedVegetation[Random.Range(0, allowedVegetation.Length)];
    placeWithRaycast = vegetationSet.CheckNeedsRaycast();
    return vegetationSet.GetRandomProp();
  }
}
