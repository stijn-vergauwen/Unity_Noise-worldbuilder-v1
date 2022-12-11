using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/VegetationSet")]
public class VegetationSetSO : ScriptableObject
{
  // defines a set of nature props (trees, shrubs, rocks) that can be placed

  [SerializeField] string setName;
  [SerializeField] int sizeClass;
  [SerializeField] bool PlaceWithRaycast;
  [SerializeField] GameObject[] props;

  public bool CheckSetName(string name) {
    return name == setName;
  }

  public bool CheckSizeClass(int size) {
    return size == sizeClass;
  }

  public bool CheckNeedsRaycast() {
    return PlaceWithRaycast;
  }

  public GameObject GetRandomProp() {
    return props[Random.Range(0, props.Length)];
  }
}
