using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Slider : MonoBehaviour
{
  [SerializeField] TextMeshProUGUI numberText;
  [SerializeField] MinMax valueRange;

  private int currentValue;

  public UnityEvent<int> OnValueChanged;

  public void ChangeValue(int newValue) {
    currentValue = Mathf.Clamp(newValue, (int)valueRange.min, (int)valueRange.max);
    UpdateText();
  }

  public void UpdateText() {
    numberText.SetText(currentValue.ToString("000"));
  }
}
