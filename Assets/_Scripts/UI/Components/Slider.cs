using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Slider : MonoBehaviour
{
  [SerializeField] UnityEngine.UI.Slider slider;
  [SerializeField] TextMeshProUGUI numberText;

  private int currentValue;

  public UnityEvent<int> OnValueChanged;

  public void SlideValue(float value) {
    currentValue = (int)value;
    UpdateText((int)value);
  }

  public void StopSlide() {
    OnValueChanged?.Invoke(currentValue);
  }

  public void SetValue(int value) {
    currentValue = value;
    slider.value = value;
    UpdateText(value);
  }

  public void UpdateText(int value) {
    numberText.SetText(value.ToString("##0"));
  }
}
