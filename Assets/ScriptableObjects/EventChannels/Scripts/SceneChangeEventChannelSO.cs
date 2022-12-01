using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "EventChannels/SceneChangeChannel")]
public class SceneChangeEventChannelSO : ScriptableObject
{
  public Action<Scene> OnSceneChangeRequested;

  public void RaiseEvent(Scene sceneToLoad) {
    if(OnSceneChangeRequested != null) {
      OnSceneChangeRequested.Invoke(sceneToLoad);

    } else {
      Debug.LogWarning("A scene change was requested but not picked up. Check why there is no SceneManager loaded and listening.");
    }
  }
}
