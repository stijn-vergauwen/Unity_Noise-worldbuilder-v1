using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    // redraw preview
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
    ApplyChanges();
    UpdateAllUI();
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

  // TODO: Update Elements after values updated
  // Value Updates

  public void UpdateHeightSeed(int newSeed) {
    worldSettings.heightMap.seed = newSeed;
    OnChange();
    // Update UI element
    ui.heightSeed.SetValue(newSeed);
  }

  public void UpdateTemperatureSeed(int newSeed) {
    worldSettings.temperatureMap.seed = newSeed;
    OnChange();
    // Update UI element
    ui.temperatureSeed.SetValue(newSeed);
  }

  public void UpdateHumiditySeed(int newSeed) {
    worldSettings.humidityMap.seed = newSeed;
    OnChange();
    // Update UI element
    ui.humiditySeed.SetValue(newSeed);
  }

  public void UpdateHeightScale(int newScale) {
    worldSettings.heightMap.scale = newScale;
    OnChange();
    // Update UI element
    ui.heightScale.SetValue(newScale);
  }

  public void UpdateTemperatureScale(int newScale) {
    worldSettings.temperatureMap.scale = newScale;
    OnChange();
    // Update UI element
    ui.temperatureScale.SetValue(newScale);
  }

  public void UpdateHumidityScale(int newScale) {
    worldSettings.humidityMap.scale = newScale;
    OnChange();
    // Update UI element
    ui.humidityScale.SetValue(newScale);
  }

  public void UpdateTransitionSmoothness(int value) {
    int newSmoothness = Mathf.Clamp(value, 0, 5);
    biomeTransitionSmoothness = newSmoothness;
    OnChange();
    // Update UI element
    ui.transitionSmoothness.SetValue(value);
  }

  public void UpdateSpawnVegetation(bool value) {
    spawnVegetation = value;
    OnChange();
    // Update UI element
    ui.spawnVegetation.isOn = value;
  }

  public void UpdateSimulateWater(bool value) {
    simulateWater = value;
    OnChange();
    // Update UI element
    ui.simulateWater.isOn = value;
  }

  public void UpdateTerrainDistance(int distance) {
    terrainDistance = distance;
    OnChange();
    // Update UI element
    ui.terrainDistance.SetValue(distance);
  }

  public void UpdateVegetationDistance(int distance) {
    VegetationDistance = distance;
    OnChange();
    // Update UI element
    ui.vegetationDistance.SetValue(distance);
  }

  public void UpdateWaterDistance(int distance) {
    WaterDistance = distance;
    OnChange();
    // Update UI element
    ui.waterDistance.SetValue(distance);
  }

  private void OnChange() {
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
