using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickablePlant : MonoBehaviour, IInteractable
{
  [SerializeField] GameObject destroyEffect;

  public void Activate() {
    Destroy(Instantiate(destroyEffect, transform.position, Quaternion.identity), 10);
    Destroy(gameObject);
  }

  public bool ShouldShowTooltip() {
    return false;
  }
}
