using System;

namespace Assets.Model
{
    public enum BluetoothLeDeviceState
    {
        Unknown,
        Initializing,
        Initialized,
        DiscoveringDevices,
        DiscoveredDevices,
        Connecting,
        Connected,
        ReadyForNotifications,
        Notifying
    }

    /// <summary>
    /// This class represents a Bluetooth Low Energy device.
    /// </summary>
    public class BluetoothLeDevice
    {
        public BluetoothLeDevice()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothLeDevice"/> class.
        /// </summary>
        /// <param name="address">The address as transferred by the Bluetooth Low Enery device.</param>
        /// <param name="displayName">The Bluetooth Low Energy display name as transferred by the Bluetooth Low Energy device.</param>
        public BluetoothLeDevice(string address, string displayName)
        {
            Address = address;
            DisplayName = displayName;
            BluetoothState = BluetoothLeDeviceState.Unknown;
            TimeStamp = DateTime.UtcNow;
        }

        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the unique address used by the system to identify the given device.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the description of the last error when the device is in error state.
        /// </summary>
        public string Error { get; set; }

		/// <summary>
		/// Gets or sets the state of the device regarding the Bluetooth Low Energy protocol.
		/// </summary>
		public BluetoothLeDeviceState BluetoothState { get; set; }

        /// <summary>
        /// Gets or sets a timestamp indicating the last time the device was discovered when scanning for devices.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return string.Equals(Address, ((BluetoothLeDevice) obj).Address);
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode();
        }
    }
}
