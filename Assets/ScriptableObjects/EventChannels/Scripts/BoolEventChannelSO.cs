using UnityEngine;
using System;

[CreateAssetMenu(menuName = "EventChannels/BoolChannel")]
public class BoolEventChannelSO : ScriptableObject
{
  public Action<bool> OnEventRaised;

  public void RaiseEvent(bool value) {
    if(OnEventRaised != null) OnEventRaised.Invoke(value);
  }
}
