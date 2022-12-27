using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/GameSettings")]
public class GameSettingsSO : ScriptableObject
{
  [SerializeField] WorldSettings worldSettings;

  public int biomeTransitionSmoothness;
  public bool spawnVegetation;
  public bool simulateWater;

  public int terrainDistance;
  public int VegetationDistance;
  public int WaterDistance;

  public WorldSettings GetWorldSettings() {
    return worldSettings;
  }

  public void SetWorldSettings(WorldSettings newSettings) {
    worldSettings = newSettings;
  }

  public void SetTerrainSettings(int transitionSmoothness, bool spawnVegetation, bool simulateWater) {
    this.biomeTransitionSmoothness = transitionSmoothness;
    this.spawnVegetation = spawnVegetation;
    this.simulateWater = simulateWater;
  }

  public void SetDistances(int terrain, int vegetation, int water) {
    terrainDistance = terrain;
    VegetationDistance = vegetation;
    WaterDistance = water;
  }
}
