using System;
using Assets.Model;
using UnityEngine;

namespace Assets.Presentation.Controllers
{
    public class LedController : UpdateControllerBase
    {
        
        private readonly Led _led;
        
        private readonly MeshRenderer _meshRenderer;       
        
        private readonly Color _offColor;
        
        private bool _isOn;

        public LedController(Xdk xdk, string gameObjectName, int ledIndex) : base(xdk, gameObjectName)
        {
            _led = Xdk.Leds[ledIndex];
            if (TargetGameObject != null)
            {
                _meshRenderer = TargetGameObject.GetComponent<MeshRenderer>();
                _offColor = _meshRenderer.material.color;
            }
        }

        public override void UpdateStatus()
        {
            if (TargetGameObject == null)
            {
                return; // some xdk models might do not have any leds
            }
            
            bool isSwitchedOn;

            switch (Xdk.BluetoothState)
            {
                case BluetoothLeDeviceState.Unknown:
                case BluetoothLeDeviceState.Initializing:
                case BluetoothLeDeviceState.Initialized:
                case BluetoothLeDeviceState.DiscoveringDevices:
                case BluetoothLeDeviceState.DiscoveredDevices:
                case BluetoothLeDeviceState.Connecting:
                    isSwitchedOn = false;
                    break;

                case BluetoothLeDeviceState.Connected:
                case BluetoothLeDeviceState.ReadyForNotifications:
                case BluetoothLeDeviceState.Notifying:
                    isSwitchedOn = _led.IsOn;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(Xdk.BluetoothState.ToString());
            }
            
            if (_isOn != isSwitchedOn)
            {
                _isOn = isSwitchedOn;
                UpdateLedLight();
            }
            

        }

        private void UpdateLedLight()
        {
            if (_isOn)
            {
                _meshRenderer.material.color = _led.Color.ToUnityColor();
            }
            else
            {
                _meshRenderer.material.color = _offColor;
            }
        }
    }
}