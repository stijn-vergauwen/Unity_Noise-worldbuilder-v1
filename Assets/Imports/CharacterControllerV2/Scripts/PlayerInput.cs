using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : CharacterInput
{
  // Uses Input class directly for player input
  
  void Start() {
    Cursor.lockState = CursorLockMode.Locked;
  }

  void Update() {
    // movement & looking
    movementInput.x = Input.GetAxisRaw("Horizontal");
    movementInput.z = Input.GetAxisRaw("Vertical");

    turnInput.x = Input.GetAxis("Mouse X");
    turnInput.y = Input.GetAxis("Mouse Y");

    // key presses
    if(Input.anyKeyDown) {
      if(Input.GetKeyDown(KeyCode.Space)) {
        OnJump?.Invoke();

      } else if(Input.GetKeyDown(KeyCode.LeftShift)) {
        OnSprint?.Invoke(true);

      } else if(Input.GetKeyDown(KeyCode.E)) {
        OnInteract?.Invoke(true);
        
      } else if(Input.GetMouseButtonDown(0)) {
        OnPrimary?.Invoke(true);
        
      } else if(Input.GetMouseButtonDown(1)) {
        OnSecondary?.Invoke(true);
        
      }
    }

    // key releases
    if(Input.GetKeyUp(KeyCode.LeftShift)) {
      OnSprint?.Invoke(false);

    } else if(Input.GetMouseButtonUp(1)) {
      OnSecondary?.Invoke(false);
      
    }
  }
}
