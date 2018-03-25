using Assets.Plugins;
using UnityEngine;

namespace Assets.Utils
{
    /// <summary>
    /// This class is a dummy implementation used by the Unity editor on a desktop.
    /// </summary>
    public class BluetoothLeCommanderMock : IBluetoothLeCommander
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }

        public void Initialize()
        {
            Debug.Log("Initialize");
        }

        public void DeInitialize()
        {
            Debug.Log("DeInitialize");
        }

        public void ScanForPeripheralsWithServices(string[] serviceUuiDs)
        {
            Debug.Log("ScanForPeripheralsWithServices");
        }

        public void StopScan()
        {
            Debug.Log("StopScan");
        }

        public void ConnectToPeripheral(string deviceAddress)
        {
            Debug.Log("ConnectToPeripheral");
        }

        public void DisconnectPeripheral(string deviceAddress)
        {
            Debug.Log("DisconnectPeripheral");
        }

        public void ReadCharacteristic(string deviceAddress, string service, string characteristic)
        {
            Debug.Log("ReadCharacteristic");
        }

        public void WriteCharacteristic(string deviceAddress, string service, string characteristic, byte[] data, int length)
        {
            Debug.Log("WriteCharacteristic");
        }

        public void SubscribeCharacteristic(string deviceAddress, string service, string characteristic)
        {
            Debug.Log("SubscribeCharacteristic");
        }

        public void UnSubscribeCharacteristic(string deviceAddress, string service, string characteristic)
        {
            Debug.Log("UnSubscribeCharacteristic");
        }

        public void UpdateCharacteristicValue(string uuid, byte[] data, int length)
        {
            Debug.Log("UpdateCharacteristicValue");
        }
    }
}
