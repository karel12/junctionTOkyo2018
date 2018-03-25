using System;
using UnityEngine;

namespace Assets.Utils
{
    public static class Constants
    {
        public const string Log = "Log";

        public const char MessageSeparator = '~';

        public const string CentralInitialized = "Initialized";
        
        public const string CentralDeInitialized = "DeInitialized";
        
        public const string CentralError = "Error";
        
        public const string CentralDiscoveredPeripheral = "DiscoveredPeripheral";
        
        public const string CentralConnectedPeripheral = "ConnectedPeripheral";
        
        public const string CentralDisconnectedPeripheral = "DisconnectedPeripheral";
        
        public const string CentralDiscoveredService = "DiscoveredService";
        
        public const string CentralDiscoveredCharacteristic = "DiscoveredCharacteristic";
        
        public const string CentralDidReadCharacteristic = "DidReadCharacteristic";
        
        public const string CentralDidWriteCharacteristic = "DidWriteCharacteristic";
        
        public const string CentralDidUpdateValueForCharacteristic = "DidUpdateValueForCharacteristic";
        
        public const string HighPriorityCharacteristic = "c2967211-7ba4-11e4-82f8-0800200c9a66";
        
        public const string LowPriorityCharacteristic = "c2967212-7ba4-11e4-82f8-0800200c9a66";

        public static TimeSpan DiscoveredDeviceTimeout = TimeSpan.FromSeconds(2);

        public static string LogFilePath
        {
            get { return Application.persistentDataPath + "/ble.log"; }
        }
    }
}