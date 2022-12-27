using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NumberInput : MonoBehaviour
{
  [SerializeField] TMP_InputField numberText;
  [SerializeField] MinMax valueRange;

  private int currentValue;

  public UnityEvent<int> OnValueChanged;

  public void ChangeValue(int newValue) {
    currentValue = Mathf.Clamp(newValue, (int)valueRange.min, (int)valueRange.max);
    OnValueChanged?.Invoke(currentValue);
    UpdateText();
  }

  public void ChangeValue(string newValue) {
    int parsedValue;
    if(int.TryParse(newValue, out parsedValue)) {
      ChangeValue(parsedValue);
    } else {
      ChangeValue(1);
    }
  }

  public void SetValue(int value) {
    currentValue = value;
    UpdateText();
  }

  public void UpdateText() {
    numberText.text = currentValue.ToString("##0");
  }
}
