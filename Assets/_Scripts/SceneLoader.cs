using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
  [SerializeField] SceneChangeEventChannelSO sceneChange;

  SceneName activeScene;

  void OnEnable() {
    sceneChange.OnSceneChangeRequested += LoadScene;
  }

  void OnDisable() {
    sceneChange.OnSceneChangeRequested -= LoadScene;
  }

  void Start() {
    SetActiveScene();
  }

  void LoadScene(SceneName sceneToLoad) {
    if(sceneToLoad == activeScene) return;

    print("Load scene: " + sceneToLoad);

    string sceneName = (
      sceneToLoad == SceneName.MainMenu ?
      "Main menu" : "Full world"
    );
    
    UnloadCurrentScene();

    SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    StartCoroutine(ActivateSceneAfterDelay(sceneName));
    activeScene = sceneToLoad;
  }
  
  void UnloadCurrentScene() {
    string sceneName = (
      activeScene == SceneName.MainMenu ?
      "Main menu":
      "Full world"
    );

    SceneManager.UnloadSceneAsync(sceneName);
  }

  IEnumerator ActivateSceneAfterDelay(string sceneName) {
    yield return new WaitForSeconds(.05f);
    SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
  }



  void SetActiveScene() {
    activeScene = (
      SceneManager.GetSceneByName("Main menu").IsValid() ?
      SceneName.MainMenu :
      SceneName.Game
    );
  }

}

public enum SceneName {MainMenu, Game}
