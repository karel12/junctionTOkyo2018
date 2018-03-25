
namespace Assets.Plugins
{
    /// <summary>
    /// Implementors of this class can call the platform specific bluetooth adapter.
    /// </summary>
    public interface IBluetoothLeCommander
    {
        /// <summary>
        /// Logs the specified message to the device.
        /// </summary>
        void Log (string message);

        /// <summary>
        /// Initializes the adapter.
        /// </summary>
        void Initialize ();

        /// <summary>
        /// Deinitializes the adapter
        /// </summary>
        void DeInitialize ();

        /// <summary>
        /// Initiates the scanning for peripheral devices.
        /// </summary>
        void ScanForPeripheralsWithServices (string[] serviceUuiDs);

        /// <summary>
        /// Stops the scanning for peripheral devices.
        /// </summary>
        void StopScan ();

        /// <summary>
        /// Initiates the connection to the given device
        /// </summary>
        void ConnectToPeripheral (string deviceAddress);

        /// <summary>
        /// Initiates the disconnection from the given device
        /// </summary>
        void DisconnectPeripheral(string deviceAddress);

        /// <summary>
        /// Initiates a read request to read a value from the given device.
        /// </summary>
        void ReadCharacteristic (string deviceAddress, string service, string characteristic);
        
        /// <summary>
        /// Initiates a write request to send a value to the given device.
        /// </summary>
        void WriteCharacteristic(string deviceAddress, string service, string characteristic, byte[] data, int length);
        
        /// <summary>
        /// Subscribes to notifications of the given characteristics.
        /// </summary>
        void SubscribeCharacteristic(string deviceAddress, string service, string characteristic);
        
        /// <summary>
        /// Unsubscribe to the given characteristic notifications.
        /// </summary>
        /// <param name="deviceAddress"></param>
        /// <param name="service"></param>
        /// <param name="characteristic"></param>
        void UnSubscribeCharacteristic(string deviceAddress, string service, string characteristic);
    }
}