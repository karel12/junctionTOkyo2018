using System;
using Assets.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Presentation.Controllers
{
    /// <summary>
    /// Controls the settings dialog.
    /// </summary>
    public class SettingsController : DialogControllerBase
    {

        private const string DeviceNameLabel = "DeviceNameValueLabel";
        private const string DeviceAddressLabel = "DeviceAddressValueLabel";
        private const string DeviceSoftwareVersionLabel = "DeviceSoftwareVersionValueLabel";
        private const string DeviceSamplingRateLabel = "RefreshRateValueLabel";
        private const string RefreshRateSlider = "RefreshRateSlider";

        public void OnShowSettings()
        {
            ShowDialog();

            SetUiComponentText(Xdk.DisplayName, DeviceNameLabel);
            SetUiComponentText(Xdk.Address, DeviceAddressLabel);
            SetUiComponentText(Xdk.FirmwareVersion, DeviceSoftwareVersionLabel);
            SetUiComponentText(string.Format("{0:0} Hz", 1000f / Xdk.NotificationSamplingTime.Value), DeviceSamplingRateLabel);
            UpdateSlider();
        }

        private bool _isUpdatingSlider;

        private void UpdateSlider()
        {
            var slider = GetComponent<Slider>(RefreshRateSlider);
            _isUpdatingSlider = true;
            try
            {
                slider.minValue = 1000f / Xdk.NotificationSamplingTime.Max;
                slider.maxValue = 1000f / Xdk.NotificationSamplingTime.Min;
                slider.value = 1000f / Xdk.NotificationSamplingTime.Value;
            }
            finally
            {
                _isUpdatingSlider = false;
            }
        }

        
        public void OnShowDeviceList()
        {
            Debug.Log("OnShowDeviceList");
            DeviceHandler.Disconnect();
        }

        public void OnResetXdk()
        {
            Debug.Log("OnResetXdk");
            DeviceHandler.SendReset();
        }

        public void OnResetApp()
        {
            Debug.Log("OnResetApp");
            Xdk.ResetOrientation();
        }

        public void OnSamplingRateChanged(Slider slider)
        {
            if (_isUpdatingSlider)
            {
                return;
            }
            
            var milliseconds = (uint)(1000f / slider.value);

            Debug.Log("OnSamplingRateChanged " + slider.value);
            SetUiComponentText(string.Format("{0:0} Hz", slider.value), DeviceSamplingRateLabel);

            if (Xdk != null && DeviceHandler != null)
            {
                milliseconds = Math.Max(Xdk.NotificationSamplingTime.Min, milliseconds);
                milliseconds = Math.Min(Xdk.NotificationSamplingTime.Max, milliseconds);
                Xdk.NotificationSamplingTime.Value = milliseconds;
                DeviceHandler.SendSamplingTime(milliseconds);
            }
        }

        public BluetoothLeMessageHandler DeviceHandler { get; set; }

    }
}
