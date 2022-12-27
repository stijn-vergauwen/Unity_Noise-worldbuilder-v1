using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
  public void UpdateScreen(bool isFullScreen) {
    if(Screen.fullScreen != isFullScreen) {
      Screen.fullScreen = isFullScreen;

      if(isFullScreen) {
        Resolution highestRes = Screen.resolutions[Screen.resolutions.Length - 1];
        Screen.SetResolution(highestRes.width, highestRes.height, FullScreenMode.FullScreenWindow);
      }
    }
  }
}
