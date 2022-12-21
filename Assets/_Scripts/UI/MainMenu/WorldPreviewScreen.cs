using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPreviewScreen : MonoBehaviour
{
  [SerializeField] WorldSettingsSO worldSettingsData;

  [Header("Channels")]
  [SerializeField] SceneChangeEventChannelSO sceneChangeChannel;

  WorldSettings selectedSettings;

  public void StartGame() {
    worldSettingsData.SetWorldSettings(selectedSettings);
    sceneChangeChannel.RaiseEvent(SceneName.Game);
  }
}
