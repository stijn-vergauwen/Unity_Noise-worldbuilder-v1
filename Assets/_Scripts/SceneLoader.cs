using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
  [SerializeField] float changeSceneFadeDuration;

  [Header("Channels")]
  [SerializeField] SceneChangeEventChannelSO sceneChange;
  [SerializeField] ScreenFadeEventChannelSO screenFade;

  SceneName activeScene;
  bool isChangingScenes;

  void OnEnable() {
    sceneChange.OnSceneChangeRequested += ChangeScene;
  }

  void OnDisable() {
    sceneChange.OnSceneChangeRequested -= ChangeScene;
  }

  void Start() {
    SetStartScene();
  }

  void ChangeScene(SceneName sceneToLoad) {
    if(sceneToLoad == activeScene || isChangingScenes) return;

    string sceneName = (
      sceneToLoad == SceneName.MainMenu ?
      "Main menu" : "Full world"
    );
    
    StartCoroutine(ChangeSceneRoutine(sceneToLoad, sceneName));
  }

  IEnumerator ChangeSceneRoutine(SceneName sceneToLoad, string sceneName) {
    isChangingScenes = true;
    screenFade.RaiseEvent(true, changeSceneFadeDuration);
    yield return new WaitForSeconds(changeSceneFadeDuration);

    UnloadCurrentScene();
    SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    StartCoroutine(ActivateSceneAfterDelay(sceneName));
    activeScene = sceneToLoad;

    screenFade.RaiseEvent(false, changeSceneFadeDuration);
    isChangingScenes = false;
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
