using UnityEngine;
using System;

[CreateAssetMenu(menuName = "EventChannels/ScreenFadeChannel")]
public class ScreenFadeEventChannelSO : ScriptableObject
{
  public Action<bool, float> OnScreenFade;

  public void RaiseEvent(bool fadeScreen, float transitionLengthSeconds) {
    if(OnScreenFade != null) OnScreenFade.Invoke(fadeScreen, transitionLengthSeconds);
  }
}
