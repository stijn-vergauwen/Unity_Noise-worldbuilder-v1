using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickablePlant : MonoBehaviour, IInteractable
{
  public void Activate() {
    Destroy(gameObject);
  }

  public bool ShouldShowTooltip() {
    return false;
  }
}
