using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Model;
using Assets.Plugins;
using UnityEngine;

namespace Assets.Core
{
    /// <summary>
    /// Executes actions as a reaction to events.
    /// </summary>
    public class BluetoothLeMessageHandler
    {
     
        private readonly IBluetoothLeCommander _commander;
        private readonly Dictionary<string, BluetoothLeDevice> _discoveredDeviceList;
        private readonly HashSet<BluetoothLeCharacteristic> _discoveredCharacteristics;
        private readonly BluetoothLeProfile _profile;
        private readonly BluetoothLeDevice _deviceModel;
        private readonly BluetoothMessageCommandQueue _pendingCommands;

        public BluetoothLeMessageHandler(BluetoothLeDevice deviceModel, BluetoothLeProfile profile, IBluetoothLeCommander commander)
        {
            _discoveredDeviceList = new Dictionary<string, BluetoothLeDevice>();
            _discoveredCharacteristics = new HashSet<BluetoothLeCharacteristic>();
            _pendingCommands = new BluetoothMessageCommandQueue();
            _deviceModel = deviceModel;
            _profile = profile;
            _commander = commander;
        }

        /// <summary>
        /// Gets the list of devices discovered so far.
        /// </summary>
        public Dictionary<string, BluetoothLeDevice> DiscoveredDeviceList
        {
            get { return _discoveredDeviceList; }
        }

        public void Update()
        {
            switch (_deviceModel.BluetoothState)
            {
                case BluetoothLeDeviceState.Unknown:
                    Initialize();
                    break;
            }
        }

        /// <summary>
        /// Initializes the message handler, this is usually called on app startup.
        /// </summary>
        public void Initialize()
        {
            _commander.Initialize();
            _deviceModel.BluetoothState = BluetoothLeDeviceState.Initializing;
        }

        /// <summary>
        /// Scans for Bluetooth Low Energy peripherals, confined to the given services.
        /// </summary>
        /// <param name="serviceUuids">The service uuids which should be provided by the given peripheral or null if no filtering should occur.</param>
        public void ScanForPeripheralsWithServices(string[] serviceUuids)
        {
            _discoveredDeviceList.Clear();
            _discoveredCharacteristics.Clear();
            _deviceModel.BluetoothState = BluetoothLeDeviceState.DiscoveringDevices;
            _commander.ScanForPeripheralsWithServices(serviceUuids);
        }

        /// <summary>
        /// Deinitializes the message handler to free resources and disconnect the device. This is usually called when the app is closed.
        /// </summary>
        public void DeInitialize()
        {
            _commander.DeInitialize();
        }

        /// <summary>
        /// Sends the command to the device to start notifications.
        /// </summary>
        public void RequestSendStartNotifications()
        {
            OnLog("SendStartNotifications");
            var startNotificationCharacteristic = _profile.GetCharacteristicOfType(CharacteristicType.IsStartingNotifications);
            _pendingCommands.Enqueue(new BluetoothMessageCommand(startNotificationCharacteristic, new byte[] { 0x01 }, 1));
            ProcessCommands();
        }

        private void ProcessCommands()
        {
            var commandToSend = _pendingCommands.TryDequeueUnsent();

            if (commandToSend != null)
            {
                if (commandToSend.IsWriteCommand)
                {
                    _commander.WriteCharacteristic(
                        _deviceModel.Address,
                        commandToSend.Characteristic.Service.UUID.ToString(),
                        commandToSend.Characteristic.UUID.ToString(),
                        commandToSend.Data,
                        commandToSend.Length);
                }
                else
                {
                    _commander.ReadCharacteristic(
                        _deviceModel.Address,
                        commandToSend.Characteristic.Service.UUID.ToString(),
                        commandToSend.Characteristic.UUID.ToString()
                        );
                }
                commandToSend.IsSent = true;
            }
        }

        public void RequestFirmwareVersion()
        {
            OnLog("ReadVersion");
            var readVersionCharacteristic = _profile.GetCharacteristicOfType(CharacteristicType.IsReadingFirmwareVersion);
            _pendingCommands.Enqueue(new BluetoothMessageCommand(readVersionCharacteristic));
            ProcessCommands();
        }

        /// <summary>
        /// Sends a command to reset the device.
        /// </summary>
        public void SendReset()
        {
            OnLog("SendReset");
            var resetCharacteristic = _profile.GetCharacteristicOfType(CharacteristicType.IsResettingXdk);
            _pendingCommands.Enqueue(new BluetoothMessageCommand(resetCharacteristic, new byte[] { 0x01 }, 1));
            ProcessCommands();
        }

        /// <summary>
        /// Sends the sampling time for the notifications.
        /// </summary>
        /// <param name="samplingTimeMilliseconds">Time to wait between notifications.</param>
        public void SendSamplingTime(UInt32 samplingTimeMilliseconds)
        {
            OnLog("SendSamplingRate " + samplingTimeMilliseconds);
            var sendSamplingRateCharacteristic = _profile.GetCharacteristicOfType(CharacteristicType.IsSendingSamplingTime);
            _pendingCommands.Enqueue(new BluetoothMessageCommand(sendSamplingRateCharacteristic, BitConverter.GetBytes(samplingTimeMilliseconds), 4));
            ProcessCommands();
        }

        /// <summary>
        /// Connect to the given device address
        /// </summary>
        public void ConnectToPeripheral(string deviceAddress)
        {
            _commander.ConnectToPeripheral(deviceAddress);
            _deviceModel.Address = deviceAddress;
            _deviceModel.BluetoothState = BluetoothLeDeviceState.Connecting;
        }

        /// <summary>
        /// Disconnects the central from the current peripheral device.
        /// </summary>
        public void Disconnect()
        {
            _commander.DisconnectPeripheral(_deviceModel.Address);
        }

        /// <summary>
        /// Add log message passed by the platform specific Bluetooth Low Energy plugin.
        /// </summary>
        public void OnLog(string logMessage)
        {
            Debug.Log(logMessage);
        }

        /// <summary>
        /// Called when an error occurred in the platform specific Bluetooth Low Energy plugin.
        /// </summary>
        public void OnPluginError(string error)
        {
            Debug.Log(error);
            if (_deviceModel != null)
            {
                _deviceModel.Error = error;
            }
        }

        /// <summary>
        /// Called when the plugin was initialized and is ready.
        /// </summary>
        public void OnPluginInitialized()
        {
            Debug.Log("OnPluginInitialized");
            _deviceModel.BluetoothState = BluetoothLeDeviceState.Initialized;
            _deviceModel.Error = null;
            ScanForPeripheralsWithServices(null);
        }

        /// <summary>
        /// Called when the plugin was deinitialized.
        /// </summary>
        public void OnPluginDeInitialized()
        {
            Debug.Log("OnPluginDeInitialized");
            _deviceModel.BluetoothState = BluetoothLeDeviceState.Unknown;
            DiscoveredDeviceList.Clear();
            _discoveredCharacteristics.Clear();
            _pendingCommands.Clear();
        }

        /// <summary>
        /// Called when a Bluetooth Low Energy peripheral device was discovered.
        /// </summary>
        /// <param name="deviceAddress">The device address.</param>
        /// <param name="deviceName">Name of the device.</param>
        public void OnCentralDiscoveredDevice(string deviceAddress, string deviceName)
        {
            _deviceModel.BluetoothState = BluetoothLeDeviceState.DiscoveredDevices;
            BluetoothLeDevice device;
            if (!_discoveredDeviceList.TryGetValue(deviceAddress, out device))
            {
                Debug.Log("Discovered peripheral:" + deviceName + ", " + deviceAddress);
                device = new BluetoothLeDevice(deviceAddress, deviceName);
                _discoveredDeviceList[deviceAddress] = device;
            }
            device.TimeStamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Called when when a Bluetooth Low Energy peripheral device was connected.
        /// </summary>
        /// <param name="deviceAddress">The device address.</param>
        public void OnCentralConnectedPeripheral(string deviceAddress)
        {
            OnLog("OnDeviceConnectedPeripheral: " + deviceAddress);
            var connectedDevice = GetDevice(deviceAddress);
            if (connectedDevice == null)
            {
                return;
            }
            _deviceModel.DisplayName = connectedDevice.DisplayName;
            _deviceModel.Address = connectedDevice.Address;
            _deviceModel.BluetoothState = BluetoothLeDeviceState.Connected;
            _pendingCommands.Clear();
        }

        /// <summary>
        /// Called when a Bluetooth Low Energy peripheral device was disconnected.
        /// </summary>
        /// <param name="deviceAddress">The device address.</param>
        public void OnCentralDisconnectedDevice(string deviceAddress)
        {
            OnLog("OnDeviceDisconnected: " + deviceAddress);
            _discoveredCharacteristics.Clear();
            _discoveredDeviceList.Clear();
            _deviceModel.DisplayName = null;
            _deviceModel.Address = null;
            _deviceModel.BluetoothState = BluetoothLeDeviceState.Initialized;
            ScanForPeripheralsWithServices(null);
        }

        /// <summary>
        /// Called when a Bluetooth Low Energy service is discovered, containing a set of characteristics.
        /// </summary>
        /// <param name="serviceUuid">The service UUID.</param>
        public void OnCentralDiscoveredService(string serviceUuid)
        {
            OnLog("OnDeviceDiscoveredService: " + serviceUuid);
            Guid serviceId = new Guid(serviceUuid);
            BluetoothLeService service;
            if (!_profile.Services.TryGetValue(serviceId, out service))
            {
                OnLog(string.Format("Device Service not recognized: {0}", serviceUuid));
            }
        }

        /// <summary>
        /// Called when a Bluetooth Low Energy characteristic is discovered. A characteristic contains the payload that is transferred using the Bluetooth protocol.
        /// When all characteristicts receiving notifications are discovered, a message will be sent to the device to start sending the Bluetooth Low Energy notifications.
        /// </summary>
        /// <param name="characteristicUuid">The characteristic UUID.</param>
        public void OnCentralDiscoveredCharacteristic(string characteristicUuid)
        {
            var characteristic = _profile.GetCharacteristic(characteristicUuid);
            _discoveredCharacteristics.Add(characteristic);

            if (characteristic.IsReceivingNotifications)
            {
                string deviceAddress = _deviceModel.Address;
                string serviceId = characteristic.Service.UUID.ToString();

                _commander.SubscribeCharacteristic(deviceAddress, serviceId, characteristicUuid);
            }

            if (IsReadyToStartNotifications() && _deviceModel.BluetoothState != BluetoothLeDeviceState.ReadyForNotifications)
            {
                _deviceModel.BluetoothState = BluetoothLeDeviceState.ReadyForNotifications;
                RequestFirmwareVersion();
                RequestSendStartNotifications();

                var xdk = _deviceModel as Xdk;
                if (xdk != null)
                {
                    SendSamplingTime(xdk.NotificationSamplingTime.Value);
                }
            }
        }

        /// <summary>
        /// Called when a characteristic was successfully transferred and read from the device. 
        /// The value as specified by the characteristic will be updated in the device model.
        /// WHen reliable message handling is required, this indicates that the read command can be removed from the outbound queue.
        /// </summary>
        /// <param name="characteristicUuid">The characteristic UUID.</param>
        /// <param name="value">The value that was read from the device.</param>
        public void OnCentralDidReadCharacteristic(string characteristicUuid, string value)
        {
            var characteristic = _profile.GetCharacteristic(characteristicUuid);
            _deviceModel.BluetoothState = BluetoothLeDeviceState.Notifying;
            characteristic.SetValue(_deviceModel, Convert.FromBase64String(value));
            _pendingCommands.TryRemoveLastCommandWithCharacteristic(characteristic.UUID);
            ProcessCommands();
        }

        /// <summary>
        /// Called when a characteristic value was successfully sent to the device. 
        /// When reliable message handling is required, this indicates that a pending write command can be removed from the inbound queue.
        /// </summary>
        /// <param name="characteristicUuid">The characteristic UUID.</param>
        public void OnCentralDidWriteCharacteristic(string characteristicUuid)
        {
            Debug.Log("OnCentralDidWriteCharacteristic");
            var characteristic = _profile.GetCharacteristic(characteristicUuid);
            _pendingCommands.TryRemoveLastCommandWithCharacteristic(characteristic.UUID);
            ProcessCommands();
        }

        public List<BluetoothLeDevice> GetRecentlyDiscoveredDevices()
        {
            return DiscoveredDeviceList.Values.ToList();
            //return this.DiscoveredDeviceList.Values.Where(d => DateTime.UtcNow - d.TimeStamp < Constants.DiscoveredDeviceTimeout).ToList();
        }

        private BluetoothLeDevice GetDevice(string deviceAddress)
        {
            if (!DiscoveredDeviceList.ContainsKey(deviceAddress))
            {
                Debug.Log("Not found: " + deviceAddress + " available: " + DiscoveredDeviceList.FirstOrDefault());
                return null;
            }

            return DiscoveredDeviceList[deviceAddress];
        }

        private bool IsReadyToStartNotifications()
        {
            return _discoveredCharacteristics.Count(c => c.IsReceivingNotifications) == _profile.Characteristics.Values.Count(c => c.IsReceivingNotifications)
                   && _discoveredCharacteristics.Any(c => c.Type == CharacteristicType.IsStartingNotifications);
        }
    }
}
