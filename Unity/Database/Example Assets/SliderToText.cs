
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

namespace VRCDatabase.Example
{

    public class SliderToText : UdonSharpBehaviour
    {
        public Slider sliderUI;
        private Text textSliderValue;

        void Start()
        {
            textSliderValue = GetComponent<Text>();
            ShowSliderValue();
        }

        public void ShowSliderValue()
        {
            textSliderValue.text = sliderUI.value.ToString();
        }
    }

}