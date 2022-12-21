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
    mainMenu.gameObject.SetActive(true);
  }

  public void ShowWorldPreview() {
    HideAll();
    mainMenu.gameObject.SetActive(true);
  }

  void HideAll() {
    mainMenu.gameObject.SetActive(false);
    settings.gameObject.SetActive(false);
    worldPreview.gameObject.SetActive(false);
  }
}
