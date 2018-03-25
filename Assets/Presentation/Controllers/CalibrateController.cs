using System;
using System.Linq;
using Assets.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Presentation.Controllers
{
    public class CalibrateController : DialogControllerBase, IUpdateController
    {
        private RangeInt32SensorValue _currentValue;

        private Func<int, string> _formatValue;

        private Func<int, string> _formatCurrentValue;


        public void OnShowTemperatureCalibration()
        {
            _currentValue = Xdk.Environment.Temperature;
            _formatValue = FormatMiliValue;
            _formatCurrentValue = FormatMiliWith2DecimalPlacesValue;
            InitUI("Temperature");
            ShowDialog();
        }

        public void OnShowHumidityCalibration()
        {
            _currentValue = Xdk.Environment.Humidity;
            _formatValue = FormatIntValue;
            _formatCurrentValue = _formatValue;
            InitUI("Humidity");
            ShowDialog();
        }

        public void OnShowPressureCalibration()
        {
            _currentValue = Xdk.Environment.Pressure;
            _formatValue = FormatIntValue;
            _formatCurrentValue = _formatValue;
            InitUI("Pressure");
            ShowDialog();
        }

        public void OnShowLightCalibration()
        {
            _currentValue = Xdk.LightSensor.Lux;
            _formatValue = FormatIntValue;
            _formatCurrentValue = _formatValue;
            InitUI("Illuminance");
            ShowDialog();
        }


        public void SetSmallRange()
        {
            _currentValue.SetRange(RangeKind.Small);
            UpdateUI();
        }

        public void SetMediumRange()
        {
            _currentValue.SetRange(RangeKind.Medium);
            UpdateUI();
        }

        public void SetBigRange()
        {
            _currentValue.SetRange(RangeKind.Big);
            UpdateUI();
        }

        public void Reset()
        {
            _currentValue.Reset();
            UpdateUI();
        }

        private void InitUI(string title)
        {
            title = string.Format("{0}", title);
            SetUiComponentText(title, "CalibrateLabel");
            UpdateRangeButton(RangeKind.Small, "SmallRangeButtonText");
            UpdateRangeButton(RangeKind.Medium, "MediumRangeButtonText");
            UpdateRangeButton(RangeKind.Big, "BigRangeButtonText");
            UpdateIcon(title);

            UpdateUI();
        }

        private void UpdateIcon(string title)
        {
            var image = GetComponent<Image>("PopupIcon");
            var sprites = Resources.LoadAll<Sprite>(@"Sprites/assets_popup_50x50");
            image.sprite = sprites.FirstOrDefault(s => s.name.Contains(title.ToLowerInvariant()));
        }

        private void UpdateRangeButton(RangeKind rangeKind, string buttonName)
        {
            var value = _currentValue.GetRangeValue(rangeKind);
            var text = string.Format("{0} {1}", "\u00B1", _formatValue(value));
            SetUiComponentText(text, buttonName);
        }

        private void UpdateUI()
        {
            SetUiComponentText(_formatValue(_currentValue.Value), "CurrentValueLabel");
            UpdateMinMaxLabels();
            UpdateMaxSlider();
            UpdateMinSlider();
        }

        private void UpdateMaxSlider()
        {
            UpdateSlider("MaxSlider", _currentValue.Max);
        }

        private void UpdateMinSlider()
        {
            UpdateSlider("MinSlider", _currentValue.Min);
        }

        private void UpdateMinMaxLabels()
        {
            SetUiComponentText(_formatValue(_currentValue.Min), "MinValueLabel");
            SetUiComponentText(_formatValue(_currentValue.Max), "MaxValueLabel");
        }

        private bool _isUpdatingSlider;

        private void UpdateSlider(string sliderName, int value)
        {
            var slider = GetComponent<Slider>(sliderName);
            _isUpdatingSlider = true;
            try
            {
                slider.minValue = _currentValue.DefaultMin;
                slider.maxValue = _currentValue.DefaultMax;
                slider.value = value;
            }
            finally
            {
                _isUpdatingSlider = false;
            }
        }

        public void OnMinChanged(Slider slider)
        {
            if (_isUpdatingSlider)
            {
                return;
            }
            _currentValue.Min = (int)Math.Round(slider.value);
            if (_currentValue.Min >= _currentValue.Max)
            {
                _currentValue.Max = _currentValue.Min;
                UpdateMaxSlider();
            }
            UpdateMinMaxLabels();
        }

        public void OnMaxChanged(Slider slider)
        {
            if (_isUpdatingSlider)
            {
                return;
            }
            _currentValue.Max = (int)Math.Round(slider.value);
            if (_currentValue.Max <= _currentValue.Min)
            {
                _currentValue.Min = _currentValue.Max;
                UpdateMinSlider();
            }

            UpdateMinMaxLabels();
        }

        private string FormatIntValue(int value)
        {
            return string.Format("{0} {1}", value, _currentValue.Unit);
        }

        private string FormatMiliValue(int value)
        {
            return string.Format("{0} {1}", value / 1000, _currentValue.Unit);
        }

        private string FormatMiliWith2DecimalPlacesValue(int value)
        {
            return string.Format("{0:0.00} {1}", value / 1000f, _currentValue.Unit);
        }

        public void UpdateStatus()
        {
            if (IsEnabled)
            {
                SetUiComponentText(_formatCurrentValue(_currentValue.Value), "CurrentValueLabel");
            }

        }
    }
}