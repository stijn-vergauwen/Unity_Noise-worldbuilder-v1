using System;
using UnityEngine;

public class CharacterInput : MonoBehaviour
{
  // access point to characters input through public variables and events

  public Vector3 movementInput = Vector3.zero;
  public Vector2 turnInput = Vector2.zero;

  // events, param true = keyDown, false = keyUp
  public Action OnJump;
  public Action<bool> OnSprint;

  public Action<bool> OnInteract;
  public Action<bool> OnPrimary;
  public Action<bool> OnSecondary;
}
