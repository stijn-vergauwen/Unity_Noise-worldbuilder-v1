using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageController : MonoBehaviour
{
  [SerializeField] MainMenu mainMenu;
  [SerializeField] SettingsScreen settings;
  [SerializeField] WorldPreviewScreen worldPreview;

  public void ShowMainMenu() {
    HideAll();
    mainMenu.gameObject.SetActive(true);
  }

  public void ShowSettings() {
    HideAll();
    settings.gameObject.SetActive(true);
  }

  public void ShowWorldPreview() {
    HideAll();
    worldPreview.gameObject.SetActive(true);
  }

  void Start() {
    ShowMainMenu();
  }

  void Update() {
    if(Input.GetKeyDown(KeyCode.Escape) && !mainMenu.gameObject.activeSelf) {
      ShowMainMenu();
    }
  }

  void HideAll() {
    mainMenu.gameObject.SetActive(false);
    settings.gameObject.SetActive(false);
    worldPreview.gameObject.SetActive(false);
  }
}
