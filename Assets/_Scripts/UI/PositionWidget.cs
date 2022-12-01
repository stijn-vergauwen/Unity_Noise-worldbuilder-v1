using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PositionWidget : MonoBehaviour
{
  [SerializeField] CharacterController character;

  [SerializeField] TextMeshProUGUI xPositionText;
  [SerializeField] TextMeshProUGUI zPositionText;

  void OnEnable() {
    character.OnPositionUpdate += UpdateWidget;
  }

  void OnDisable() {
    character.OnPositionUpdate -= UpdateWidget;
  }

  void UpdateWidget(Vector3 newPosition) {
    xPositionText.SetText("X: " + newPosition.x.ToString("F2"));
    zPositionText.SetText("Z: " + newPosition.z.ToString("F2"));
  }
}
