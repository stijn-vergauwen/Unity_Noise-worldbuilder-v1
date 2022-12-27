using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PositionWidget : MonoBehaviour
{
  [SerializeField] PlayerPerspective player;

  [SerializeField] TextMeshProUGUI xPositionText;
  [SerializeField] TextMeshProUGUI zPositionText;

  void OnEnable() {
    player.OnPositionUpdate += UpdateWidget;
  }

  void OnDisable() {
    player.OnPositionUpdate -= UpdateWidget;
  }

  public void ToggleWidget(bool value) {
    gameObject.SetActive(value);
  }

  void UpdateWidget(Vector3 newPosition) {
    xPositionText.SetText("X: " + newPosition.x.ToString("F2"));
    zPositionText.SetText("Z: " + newPosition.z.ToString("F2"));
  }
}
