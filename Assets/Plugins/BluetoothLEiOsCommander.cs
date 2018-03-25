using System;
using System.Runtime.InteropServices;

namespace Assets.Plugins
{
    /// <summary>
    /// Bluetooth bridge for iOS platform
    /// </summary>
    public class BluetoothLEiOsCommander : IBluetoothLeCommander
    {
        public enum CBCharacteristicProperties
        {
            CBCharacteristicPropertyBroadcast = 0x01,
            CBCharacteristicPropertyRead = 0x02,
            CBCharacteristicPropertyWriteWithoutResponse = 0x04,
            CBCharacteristicPropertyWrite = 0x08,
            CBCharacteristicPropertyNotify = 0x10,
            CBCharacteristicPropertyIndicate = 0x20,
            CBCharacteristicPropertyAuthenticatedSignedWrites = 0x40,
            CBCharacteristicPropertyExtendedProperties = 0x80,
            CBCharacteristicPropertyNotifyEncryptionRequired = 0x100,
            CBCharacteristicPropertyIndicateEncryptionRequired = 0x200,
        };

        public enum CBAttributePermissions
        {
            CBAttributePermissionsReadable = 0x01,
            CBAttributePermissionsWriteable = 0x02,
            CBAttributePermissionsReadEncryptionRequired = 0x04,
            CBAttributePermissionsWriteEncryptionRequired = 0x08,
        };

        [DllImport("__Internal")]
        private static extern void _BtleAdapterLog(string message);

        [DllImport("__Internal")]
        private static extern void _BtleAdapterInitialize();

        [DllImport("__Internal")]
        private static extern void _BtleAdapterDeInitialize();

        [DllImport("__Internal")]
        private static extern void _BtleAdapterScanForPeripheralsWithServices(string serviceUuiDsString);

        [DllImport("__Internal")]
        private static extern void _BtleAdapterStopScan();

        [DllImport("__Internal")]
		private static extern void _BtleAdapterConnectToPeripheral(string deviceAddress);

        [DllImport("__Internal")]
		private static extern void _BtleAdapterDisconnectPeripheral(string deviceAddress);

        [DllImport("__Internal")]
		private static extern void _BtleAdapterReadCharacteristic(string deviceAddress, string serviceUuid, string characteristicUuid);

        [DllImport("__Internal")]
		private static extern void _BtleAdapterWriteCharacteristic(string deviceAddress, string serviceUuid, string characteristicUuid, byte[] data, int length);

        [DllImport("__Internal")]
		private static extern void _BtleAdapterSubscribeCharacteristic(string deviceAddress, string serviceUuid, string characteristicUuid);

        [DllImport("__Internal")]
		private static extern void _BtleAdapterUnSubscribeCharacteristic(string deviceAddress, string serviceUuid, string characteristicUuid);

        public void Log(string message)
        {
            _BtleAdapterLog(message);
        }

        public void Initialize()
        {
            _BtleAdapterInitialize();
        }

        public void DeInitialize()
        {
            _BtleAdapterDeInitialize();
        }

        public void ScanForPeripheralsWithServices(string[] serviceUuiDs)
        {
            string serviceUuiDsString = serviceUuiDs != null ? string.Join("|", serviceUuiDs) : null;
            _BtleAdapterScanForPeripheralsWithServices(serviceUuiDsString);
        }

        public void StopScan()
        {
            _BtleAdapterStopScan();
        }

        public void ConnectToPeripheral(string deviceAddress)
        {
            _BtleAdapterConnectToPeripheral(deviceAddress);
        }

        public void DisconnectPeripheral(string deviceAddress)
        {
            _BtleAdapterDisconnectPeripheral(deviceAddress);
        }

        public void ReadCharacteristic(string deviceAddress, string serviceUuid, string characteristicUuid)
        {
			_BtleAdapterReadCharacteristic(deviceAddress, serviceUuid, characteristicUuid);
        }

		public void WriteCharacteristic(string deviceAddress, string serviceUuid, string characteristicUuid, byte[] data, int length)
        {
			_BtleAdapterWriteCharacteristic(deviceAddress, serviceUuid, characteristicUuid, data, length);
        }

		public void SubscribeCharacteristic(string deviceAddress, string serviceUuid, string characteristicUuid)
        {
			_BtleAdapterSubscribeCharacteristic(deviceAddress, serviceUuid, characteristicUuid);
        }

		public void UnSubscribeCharacteristic(string deviceAddress, string serviceUuid, string characteristicUuid)
        {
			_BtleAdapterUnSubscribeCharacteristic(deviceAddress, serviceUuid, characteristicUuid);
        }
    }
}
