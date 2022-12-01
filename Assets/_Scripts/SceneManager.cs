using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
  [SerializeField] SceneChangeEventChannelSO sceneChange;

  void OnEnable() {
    sceneChange.OnSceneChangeRequested += LoadScene;
  }

  void OnDisable() {
    sceneChange.OnSceneChangeRequested -= LoadScene;
  }

  void LoadScene(Scene sceneToLoad) {
    print("Load scene:" + sceneToLoad);
  }

}

public enum Scene {MainMenu, Game, WorldPreview}
