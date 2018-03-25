using System;
using UnityEngine;

namespace Assets.Model
{
    public enum CharacteristicType
    {
        None,
        IsStartingNotifications,
        IsSendingSamplingTime,
        IsReadingFirmwareVersion,
        IsResettingXdk
    }

    /// <summary>
    /// This class represents a Bluetooth Low Energy characteristic and can be used to configure a service.
    /// </summary>
    /// <typeparam name="TDevice">The type of the device.</typeparam>
    public class BluetoothLeCharacteristic<TDevice> : BluetoothLeCharacteristic
    {
        private Action<TDevice, byte[]> _setterAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothLeCharacteristic{TDevice}"/> class, which defines a Blueooth Low Energy characteristic.
        /// </summary>
        /// <param name="uuid">The UUID.</param>
        /// <param name="displayName">The display name.</param>
        public BluetoothLeCharacteristic(Guid uuid, string displayName) : base(uuid, displayName)
        {
        }

        /// <summary>
        /// This adds the setter action to the characteristic which can be used to map a value update of this characteristic to the device model instance.
        /// </summary>
        /// <param name="setter">The setter.</param>
        /// <returns></returns>
        public BluetoothLeCharacteristic WithSetter(Action<TDevice, byte[]> setter)
        {
            _setterAction = setter;
            return this;
        }

        /// <summary>
        /// Uses the setter action as defined by the <see cref="WithSetter"/> method to update the value of the characteristic.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="message">The message.</param>
        public override void SetValue(object device, byte[] message)
        {
            _setterAction((TDevice)device, message);
        }
    }

    /// <summary>
    /// Base class of any characteristic which can be used to generalize all access to characteristics.
    /// </summary>
    public abstract class BluetoothLeCharacteristic : BluetoothLeItem
    {
        protected BluetoothLeCharacteristic(Guid uuid, string displayName) : base(uuid, displayName)
        {
        }

        /// <summary>
        /// Gets or sets the service used by this characteristic.
        /// </summary>
        public BluetoothLeService Service { get; set; }

        /// <summary>
        /// Uses the setter action as defined by the WithSetter method of the concrete instance to update the value of the characteristic.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <param name="message">The message.</param>
        public abstract void SetValue(object device, byte[] message);

        public BluetoothLeCharacteristic IsNotified(bool isReceivingNotifications)
        {
			IsReceivingNotifications = isReceivingNotifications;
            return this;
        }

        /// <summary>
        /// Sets the Type of characteristic. This identifies special characteristics required by the system, e.g. to start notifications or send specific values.
        /// </summary>
        public BluetoothLeCharacteristic OfType(CharacteristicType characteristicType)
        {
            Type = characteristicType;
            return this;
        }

        public CharacteristicType Type { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this characteristic will be subscribed to a notification.
        /// </summary>
        public bool IsReceivingNotifications { get; private set; }
    }

    /// <summary>
    /// An instance of this is used when the received characteristic is not defined in <see cref="BluetoothLeProfile"/>
    /// </summary>
    public class BluetoothLeCharacteristicUnknown : BluetoothLeCharacteristic
    {
        public BluetoothLeCharacteristicUnknown(Guid uuid, string displayName) : base(uuid, displayName)
        {
        }

        public override void SetValue(object device, byte[] message)
        {
            Debug.Log("A setter of an unknown characteristic was called for value " + Convert.ToBase64String(message));
        }
    }
}