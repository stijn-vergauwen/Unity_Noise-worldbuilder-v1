using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
  [SerializeField] SceneChangeEventChannelSO sceneChange;

  public void StartGame() {
    sceneChange.RaiseEvent(SceneName.Game);
  }

  public void ExitGame() {
    Application.Quit();
  }
}
