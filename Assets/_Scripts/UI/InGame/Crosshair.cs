using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
  [SerializeField] Image crosshairImage;
  [SerializeField] CharacterController characterController;
  [SerializeField] PlayerPerspective playerPerspective;

  bool isVisible = true;
  bool isFaded = true;

  public void ToggleCrosshair(bool value) {
    isVisible = value;
  }

  private void Update() {
    isFaded = !characterController.hasInteractableTarget;
    UpdateVisibility();
  }

  private void UpdateVisibility() {
    if(isVisible &&
      playerPerspective.currentPerspective == PlayerPerspective.Perspective.FirstPerson
    ) {
      SetOpacity(isFaded ? .3f : 1);

    } else {
      SetOpacity(0);
    }
  }

  private void SetOpacity(float opacity) {
    Color color = crosshairImage.color;
    color.a = opacity;
    crosshairImage.color = color;
  }
}
