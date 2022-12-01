using System;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
  // central character class, holds state, other components communicate through this one

  // not sure how to decouple the input class, multiple places need access to it. i've made the class a bit more abstract tho
  public CharacterInput input;

  // components
  public Transform pivot;
  public Transform itemAnchor;
  public Rigidbody rb;
  public Camera cam;

  // state
  public bool isGrounded = true;
  IGrabbable currentlyGrabbedItem;

  // interactable targets
  IInteractable interactableTarget;
  IGrabbable grabbableTarget;
  Transform targetTransform;

  // events
  public Action<Vector3> OnPositionUpdate;

  void OnEnable() {
    input.OnInteract += OnInteract;
  }

  void OnDisable() {
    input.OnInteract -= OnInteract;
  }
  
  void Update() {
    UpdateTarget();
  }

  void UpdateTarget() {
    // This whole interaction code can def be much simpler, this is my first attempt though so fine for now

    RaycastHit hitInfo;
    if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, 2)) {
      if(hitInfo.transform != targetTransform) {

        // if interactable found
        IInteractable interactable = hitInfo.transform.GetComponentInParent<IInteractable>();
        if(interactable != null) {
          interactableTarget = interactable;
          targetTransform = hitInfo.transform;
          return;

        } else {
          interactableTarget = null;
          targetTransform = null;
        }

        // if grabbable found
        IGrabbable grabbable = hitInfo.transform.GetComponentInParent<IGrabbable>();
        if(grabbable != null) {
          grabbableTarget = grabbable;
          targetTransform = hitInfo.transform;
          return;

        } else {
          grabbableTarget = null;
          targetTransform = null;
        }
      }

    } else {
      // if nothing hit
      interactableTarget = null;
      grabbableTarget = null;
      targetTransform = null;
    }
  }

  void OnInteract(bool state) {
    if(currentlyGrabbedItem != null) {
      currentlyGrabbedItem.Drop();
      currentlyGrabbedItem = null;

    } else if(interactableTarget != null) {
      interactableTarget.Activate();

    } else if(grabbableTarget != null) {
      grabbableTarget.Grab(itemAnchor);
      currentlyGrabbedItem = grabbableTarget;
    }
  }

  public void RaisePositionEvent() {
    OnPositionUpdate?.Invoke(rb.position);
  }

  // void OnDrawGizmos() {
  //   Gizmos.color = Color.blue;
  //   Gizmos.DrawRay(cam.transform.position, cam.transform.forward * 2);
  // }
}
