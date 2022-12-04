using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
  [SerializeField] RawImage overlay;
  [SerializeField] ScreenFadeEventChannelSO screenFade;

  Color overlayColor;

  void OnEnable() {
    screenFade.OnScreenFade += ToggleFade;
  }

  void OnDisable() {
    screenFade.OnScreenFade -= ToggleFade;
  }

  void Start() {
    overlayColor = overlay.color;
  }

  void ToggleFade(bool fadeScreen, float transitionLength) {
    if(fadeScreen) {
      StartCoroutine(ScreenFadeTransition(0, 1, transitionLength));

    } else {
      StartCoroutine(ScreenFadeTransition(1, 0, transitionLength));
    }
  }

  IEnumerator ScreenFadeTransition(float startOpacity, float endOpacity, float duration) {
    float elapsedTime = 0;
    while(elapsedTime < duration) {
      SetOverlayOpacity(Mathf.Lerp(startOpacity, endOpacity, elapsedTime / duration));

      elapsedTime += Time.deltaTime;
      yield return null;
    }
    SetOverlayOpacity(endOpacity);
  }

  void SetOverlayOpacity(float opacity) {
    Color adjustedColor = overlayColor;
    adjustedColor.a = opacity;
    overlay.color = adjustedColor;
  }
}
