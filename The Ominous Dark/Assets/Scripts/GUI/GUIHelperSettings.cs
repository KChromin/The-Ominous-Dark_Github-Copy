using System.Globalization;
using UnityEngine.UIElements;

namespace NOS.GUI.Settings
{
    public class GUIHelperSettings
    {
        public void UpdateSlider(float newValue, Slider sliderReference)
        {
            UpdateSliderValueText(sliderReference, sliderReference.Q<Label>(""));
        }
        
        public void UpdateSliderValueText(Slider slider, Label valueText)
        {
            valueText.text = slider.value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
