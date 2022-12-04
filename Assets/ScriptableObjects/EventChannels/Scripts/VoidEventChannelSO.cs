using UnityEngine;
using System;

[CreateAssetMenu(menuName = "EventChannels/VoidChannel")]
public class VoidEventChannelSO : ScriptableObject
{
  public Action OnEventRaised;

  public void RaiseEvent() {
    if(OnEventRaised != null) OnEventRaised.Invoke();
  }
}
