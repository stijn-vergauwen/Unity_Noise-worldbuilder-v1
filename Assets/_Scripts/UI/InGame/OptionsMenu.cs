using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour
{
  [SerializeField] GameObject menu;

  [Header("Channels")]
  [SerializeField] SceneChangeEventChannelSO sceneChange;

  bool menuIsActive = false;

  public UnityEvent<bool> OnToggleMenu;

  void Start() {
    SetMenuActive(false);
  }

  void Update() {
    if(Input.GetKeyDown(KeyCode.Escape)) {
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
    OnToggleMenu?.Invoke(value);
  }
  
  public void ReturnToMainMenu() {
    sceneChange.RaiseEvent(SceneName.MainMenu);
  }

  bool MouseOnUI() {
    return EventSystem.current.IsPointerOverGameObject();
  }
}
