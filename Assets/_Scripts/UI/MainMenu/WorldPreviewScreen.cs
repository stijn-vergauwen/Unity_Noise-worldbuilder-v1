using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPreviewScreen : MonoBehaviour
{
  [SerializeField] GameSettingsSO gameSettingsData;
  [SerializeField] float timeToRefreshPreview;

  [Header("Channels")]
  [SerializeField] SceneChangeEventChannelSO sceneChangeChannel;

  private WorldSettings selectedSettings;

  private int biomeTransitionSmoothness;
  private bool spawnVegetation;
  private bool simulateWater;

  private int terrainDistance;
  private int VegetationDistance;
  private int WaterDistance;

  private float timeOfLastChange;
  private bool changesMade;

  private void Update() {
    if(changesMade && Time.time > timeOfLastChange + timeToRefreshPreview) {
      UpdatePreview();
    }
  }

  public void StartGame() {
    ApplyChanges();

    sceneChangeChannel.RaiseEvent(SceneName.Game);
  }

  public void UpdatePreview() {
    ApplyChanges();

    // redraw preview
  }

  private void ApplyChanges() {
    changesMade = false;
    gameSettingsData.SetWorldSettings(selectedSettings);
    gameSettingsData.SetOtherSettings(biomeTransitionSmoothness, spawnVegetation, simulateWater);
    gameSettingsData.SetDistances(terrainDistance, VegetationDistance, WaterDistance);
  }

  public void RandomizeSeedValues() {
    UpdateHeightSeed(Random.Range(0, 1000));
    UpdateTemperatureSeed(Random.Range(0, 1000));
    UpdateHumiditySeed(Random.Range(0, 1000));
  }

  // TODO: Update Elements after values updated
  // Value Updates

  public void UpdateHeightSeed(int newSeed) {
    selectedSettings.heightMap.seed = newSeed;
    UpdateLastChangeTime();
    // Update UI element
  }

  public void UpdateTemperatureSeed(int newSeed) {
    selectedSettings.temperatureMap.seed = newSeed;
    UpdateLastChangeTime();
    // Update UI element
  }

  public void UpdateHumiditySeed(int newSeed) {
    selectedSettings.humidityMap.seed = newSeed;
    UpdateLastChangeTime();
    // Update UI element
  }

  public void UpdateHeightScale(int newScale) {
    selectedSettings.heightMap.scale = newScale;
    UpdateLastChangeTime();
    // Update UI element
  }

  public void UpdateTemperatureScale(int newScale) {
    selectedSettings.temperatureMap.scale = newScale;
    UpdateLastChangeTime();
    // Update UI element
  }

  public void UpdateHumidityScale(int newScale) {
    selectedSettings.humidityMap.scale = newScale;
    UpdateLastChangeTime();
    // Update UI element
  }

  public void UpdateTransitionSmoothness(int value) {
    int newSmoothness = Mathf.Clamp(value, 0, 5);
    biomeTransitionSmoothness = newSmoothness;
    UpdateLastChangeTime();
    // Update UI element
  }

  public void UpdateSpawnVegetation(bool value) {
    spawnVegetation = value;
    UpdateLastChangeTime();
    // Update UI element
  }

  public void UpdateSimulateWater(bool value) {
    simulateWater = value;
    UpdateLastChangeTime();
    // Update UI element
  }

  public void UpdateTerrainDistance(int distance) {
    terrainDistance = distance;
    UpdateLastChangeTime();
    // Update UI element
  }

  public void UpdateVegetationDistance(int distance) {
    VegetationDistance = distance;
    UpdateLastChangeTime();
    // Update UI element
  }

  public void UpdateWaterDistance(int distance) {
    WaterDistance = distance;
    UpdateLastChangeTime();
    // Update UI element
  }

  private void UpdateLastChangeTime() {
    timeOfLastChange = Time.time;
    changesMade = true;
  }
}
