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
    SetStartScene();
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

  void SetStartScene() {
    SceneName startScene;
    if(!CheckLoadedScene(out startScene)) {
      SceneManager.LoadScene("Main menu", LoadSceneMode.Additive);
      StartCoroutine(ActivateSceneAfterDelay("Main menu"));

    } else {
      activeScene = startScene;
    }
  }

  bool CheckLoadedScene(out SceneName loadedScene) {
    loadedScene = (
      SceneManager.GetSceneByName("Full world").IsValid() ?
      SceneName.Game :
      SceneName.MainMenu
    );
    return SceneManager.sceneCount > 1;
  }

}

public enum SceneName {MainMenu, Game}
