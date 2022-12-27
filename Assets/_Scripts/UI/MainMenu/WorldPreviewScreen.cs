using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class WorldPreviewScreen : MonoBehaviour
{
  [SerializeField] GameSettingsSO gameSettingsData;
  [SerializeField] GameSettingsSO defaultSettings;
  [SerializeField] float timeToRefreshPreview;
  [SerializeField] UIReferences ui;

  [Header("Channels")]
  [SerializeField] SceneChangeEventChannelSO sceneChangeChannel;

  private WorldSettings worldSettings;

  private int biomeTransitionSmoothness;
  private bool spawnVegetation;
  private bool simulateWater;

  private int terrainDistance;
  private int VegetationDistance;
  private int WaterDistance;

  private float timeOfLastChange;
  private bool changesMade;

  public UnityEvent OnUpdatePreview;

  private void Start() {
    ResetToDefaultSettings();
  }

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

    print("Update preview");

    OnUpdatePreview?.Invoke();
  }

  private void ApplyChanges() {
    changesMade = false;
    gameSettingsData.SetWorldSettings(worldSettings);
    gameSettingsData.SetTerrainSettings(biomeTransitionSmoothness, spawnVegetation, simulateWater);
    gameSettingsData.SetDistances(terrainDistance, VegetationDistance, WaterDistance);
  }

  public void RandomizeSeedValues() {
    UpdateHeightSeed(Random.Range(0, 1000));
    UpdateTemperatureSeed(Random.Range(0, 1000));
    UpdateHumiditySeed(Random.Range(0, 1000));
  }

  public void ResetToDefaultSettings() {
    worldSettings = defaultSettings.GetWorldSettings();
    biomeTransitionSmoothness = defaultSettings.biomeTransitionSmoothness;
    spawnVegetation = defaultSettings.spawnVegetation;
    simulateWater = defaultSettings.simulateWater;
    terrainDistance = defaultSettings.terrainDistance;
    VegetationDistance = defaultSettings.VegetationDistance;
    WaterDistance = defaultSettings.WaterDistance;
    UpdateAllUI();
    RandomizeSeedValues();
  }

  private void UpdateAllUI() {
    ui.heightSeed.SetValue(worldSettings.heightMap.seed);
    ui.temperatureSeed.SetValue(worldSettings.temperatureMap.seed);
    ui.humiditySeed.SetValue(worldSettings.humidityMap.seed);

    ui.heightScale.SetValue((int)worldSettings.heightMap.scale);
    ui.temperatureScale.SetValue((int)worldSettings.temperatureMap.scale);
    ui.humidityScale.SetValue((int)worldSettings.humidityMap.scale);

    ui.transitionSmoothness.SetValue(biomeTransitionSmoothness);
    ui.spawnVegetation.isOn = spawnVegetation;
    ui.simulateWater.isOn = simulateWater;

    ui.terrainDistance.SetValue(terrainDistance);
    ui.vegetationDistance.SetValue(VegetationDistance);
    ui.waterDistance.SetValue(WaterDistance);
  }

  // Value Updates

  public void UpdateHeightSeed(int newSeed) {
    worldSettings.heightMap.seed = newSeed;
    OnPreviewChangeMade();

    ui.heightSeed.SetValue(newSeed);
  }

  public void UpdateTemperatureSeed(int newSeed) {
    worldSettings.temperatureMap.seed = newSeed;
    OnPreviewChangeMade();

    ui.temperatureSeed.SetValue(newSeed);
  }

  public void UpdateHumiditySeed(int newSeed) {
    worldSettings.humidityMap.seed = newSeed;
    OnPreviewChangeMade();

    ui.humiditySeed.SetValue(newSeed);
  }

  public void UpdateHeightScale(int newScale) {
    worldSettings.heightMap.scale = newScale;
    OnPreviewChangeMade();

    ui.heightScale.SetValue(newScale);
  }

  public void UpdateTemperatureScale(int newScale) {
    worldSettings.temperatureMap.scale = newScale;
    OnPreviewChangeMade();

    ui.temperatureScale.SetValue(newScale);
  }

  public void UpdateHumidityScale(int newScale) {
    worldSettings.humidityMap.scale = newScale;
    OnPreviewChangeMade();

    ui.humidityScale.SetValue(newScale);
  }

  public void UpdateTransitionSmoothness(int value) {
    int newSmoothness = Mathf.Clamp(value, 0, 3);
    biomeTransitionSmoothness = newSmoothness;
    OnPreviewChangeMade();

    ui.transitionSmoothness.SetValue(value);
  }

  public void UpdateSpawnVegetation(bool value) {
    spawnVegetation = value;

    ui.spawnVegetation.isOn = value;
  }

  public void UpdateSimulateWater(bool value) {
    simulateWater = value;

    ui.simulateWater.isOn = value;
  }

  public void UpdateTerrainDistance(int distance) {
    terrainDistance = distance;

    UpdateVegetationDistance(VegetationDistance);
    UpdateWaterDistance(WaterDistance);

    ui.terrainDistance.SetValue(distance);
  }

  public void UpdateVegetationDistance(int distance) {
    VegetationDistance = Mathf.Clamp(distance, 20, terrainDistance - 80);

    ui.vegetationDistance.SetValue(VegetationDistance);
  }

  public void UpdateWaterDistance(int distance) {
    WaterDistance = Mathf.Clamp(distance, 20, terrainDistance - 80);

    ui.waterDistance.SetValue(WaterDistance);
  }

  private void OnPreviewChangeMade() {
    timeOfLastChange = Time.time;
    changesMade = true;
  }

  [System.Serializable]
  private struct UIReferences {
    public NumberInput heightSeed;
    public NumberInput temperatureSeed;
    public NumberInput humiditySeed;

    public Slider heightScale;
    public Slider temperatureScale;
    public Slider humidityScale;

    public Slider transitionSmoothness;
    public Toggle spawnVegetation;
    public Toggle simulateWater;

    public Slider terrainDistance;
    public Slider vegetationDistance;
    public Slider waterDistance;
  }
}
