using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/VegitationSet")]
public class VegitationSetSO : ScriptableObject
{
  // defines a set of nature props (trees, shrubs, rocks) that can be placed

  [SerializeField] string setName;
  [SerializeField] int sizeClass;
  [SerializeField] GameObject[] props;

  public bool CheckSetName(string name) {
    return name == setName;
  }

  public bool CheckSizeClass(int size) {
    return size == sizeClass;
  }

  public GameObject GetRandomProp() {
    return props[Random.Range(0, props.Length)];
  }
}