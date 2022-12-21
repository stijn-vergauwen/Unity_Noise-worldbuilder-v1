using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/WorldSettings")]
public class WorldSettingsSO : ScriptableObject
{
  WorldSettings currentSettings;

  public void SetWorldSettings(WorldSettings newSettings) {
    currentSettings = newSettings;
  }

  public WorldSettings GetWorldSettings() {
    return currentSettings;
  }
}
