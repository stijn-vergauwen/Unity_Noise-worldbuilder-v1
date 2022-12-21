using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SettingsScreen : MonoBehaviour
{
  [SerializeField] Settings settings;

  [Header("UI Elements")]
  [SerializeField] Toggle fullScreenToggle;

  bool isFullScreen;

  void Awake() {
    isFullScreen = Screen.fullScreen;
    fullScreenToggle.isOn = isFullScreen;
  }

  public void SetFullScreen(bool fullScreen) {
    isFullScreen = fullScreen;
  }

  public void ApplySettings() {
    settings.UpdateScreen(isFullScreen);
  }
}
