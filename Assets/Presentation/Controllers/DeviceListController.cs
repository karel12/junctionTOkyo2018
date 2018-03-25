using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Core;
using Assets.Model;
using Assets.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Presentation.Controllers
{
    /// <summary>
    /// Controls the behavior of the device list dialog (Scanning for Devices dialog).
    /// </summary>
    public class DeviceListController : MonoBehaviour, ILayoutChild
    {
        private readonly Dictionary<BluetoothLeDevice, GameObject> _deviceComponents = new Dictionary<BluetoothLeDevice, GameObject>();

        public BluetoothLeMessageHandler DeviceHandler { get; set; }

        public Xdk Xdk { get; set; }

        public ILayoutController LayoutController { get; set; }

        public bool IsEnabled
        {
            get { return transform.parent.gameObject.GetComponent<Canvas>().enabled; }
            set { transform.parent.gameObject.GetComponent<Canvas>().enabled = value; }
        }

        public bool ForceOnTop { get; set; }

        public void ClearDeviceList()
        {
            _deviceComponents.Clear();
        }

        public void UpdateDevices(List<BluetoothLeDevice> devices)
        {
            foreach (var deviceToAdd in devices.Except(_deviceComponents.Keys))
            {
                AddDeviceComponent(deviceToAdd);
            }

            foreach (var deviceComponentToDelete in _deviceComponents.Where(c => devices.All(d => d.Address != c.Key.Address)).ToList())
            {
                RemoveComponent(deviceComponentToDelete);
            }

            UpdateButtonPositions();
        }

        public void Start()
        {
            ViewHelpers.RefreshTextToTriggerRendering(this);
        }

        /// <summary>
        ///  Update is called once per frame
        /// </summary>
        public void Update()
        {


            if (LayoutController == null || DeviceHandler == null)
                return;

            if (Xdk.BluetoothState == BluetoothLeDeviceState.DiscoveringDevices || Xdk.BluetoothState == BluetoothLeDeviceState.DiscoveredDevices)
            {
                LayoutController.VisibleDialog = this;
                switch (Xdk.BluetoothState)
                {
                    case BluetoothLeDeviceState.Initialized:
                        ClearDeviceList();
                        break;
                    case BluetoothLeDeviceState.DiscoveredDevices:
                        UpdateDevices(DeviceHandler.GetRecentlyDiscoveredDevices());
                        break;
                }
            }
            else if (IsEnabled)
            {
                IsEnabled = false;
            }


        }

        public Canvas GetCanvasComponent() { return transform.parent.gameObject.GetComponent<Canvas>(); }
        public GameObject ListContentView
        {
            get { return GameObject.Find("DeviceListContent"); }
        }

        public void OnConnect(string deviceAddress)
        {
            if (DeviceHandler != null)
            {
                Debug.Log("Connecting to " + deviceAddress);
                Xdk.Reset();
                DeviceHandler.ConnectToPeripheral(deviceAddress);
            }
        }

        private void UpdateButtonPositions()
        {
            float y = 0;

            var originalButton = GameObject.Find("DeviceButton0");
            var tansf = originalButton.GetComponent<RectTransform>();
            var height = tansf.rect.height * tansf.localScale.y;


            var listRectTransform = ListContentView.GetComponent<RectTransform>();
            listRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * (_deviceComponents.Count + 3));

            foreach (var deviceComponent in _deviceComponents)
            {
                deviceComponent.Value.transform.localPosition = originalButton.transform.localPosition + new Vector3(0, y, 0);

                y -= height + (height / 5);
            }

            ViewHelpers.RefreshTextToTriggerRendering(this);
        }

        private void AddDeviceComponent(BluetoothLeDevice deviceToAdd)
        {
            var originalButton = GameObject.Find("DeviceButton0");
            SetButtonVisibility(originalButton, false);

            GameObject newButton = (GameObject)Instantiate(originalButton);
            newButton.transform.SetParent(ListContentView.transform, false);
            //newButton.transform.position = originalButton.transform.position + new Vector3(0, -ListEntryHeight * this.deviceComponents.Count, 0);
            Text textComponent = newButton.transform.Find("Text").GetComponent<Text>();
            textComponent.text = deviceToAdd.DisplayName + Environment.NewLine + deviceToAdd.Address;
            SetButtonVisibility(newButton, true);

            var component = newButton.GetComponent<Button>();
            component.onClick.AddListener(() => OnConnect(deviceToAdd.Address));
            _deviceComponents.Add(deviceToAdd, newButton);


        }

        private void RemoveComponent(KeyValuePair<BluetoothLeDevice, GameObject> deviceComponentToDelete)
        {
            _deviceComponents.Remove(deviceComponentToDelete.Key);
            Destroy(deviceComponentToDelete.Value);
            if (!_deviceComponents.Any())
            {
                var originalButton = GameObject.Find("DeviceButton0");
                SetButtonVisibility(originalButton, true);
            }
        }

        private void SetButtonVisibility(GameObject button, bool isVisible)
        {
            var imageComponent = button.GetComponent<Image>();
            imageComponent.enabled = isVisible;
            var textComponent = button.GetComponent<Text>();
            if (textComponent != null)
                textComponent.enabled = isVisible;
        }
    }
}
