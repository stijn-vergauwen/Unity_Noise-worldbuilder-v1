using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour
{
  [SerializeField] GameObject menu;
  [SerializeField] SceneChangeEventChannelSO sceneChange;

  bool menuIsActive = false;

  void Start() {
    SetMenuActive(false);
  }

  void Update() {
    if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.O)) {
      SetMenuActive(!menuIsActive);
    }

    if(menuIsActive &&
      Input.GetMouseButtonDown(0) &&
      !MouseOnUI()
    ) {
      SetMenuActive(false);
    }
  }

  void SetMenuActive(bool value) {
    menu.SetActive(value);
    menuIsActive = value;
    Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
  }
  
  public void ReturnToMainMenu() {
    sceneChange.RaiseEvent(SceneName.MainMenu);
  }

  bool MouseOnUI() {
    return EventSystem.current.IsPointerOverGameObject();
  }
}
