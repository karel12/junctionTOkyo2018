using System;
using System.Collections.Generic;
using System.Linq;
namespace Assets.Model
{
    /// <summary>
    /// The represents a Bluetooth Low Energy profile which contains a set of services for a device.
    /// </summary>
    public abstract class BluetoothLeProfile
    {
        private IDictionary<Guid, BluetoothLeCharacteristic> _characteristics;

        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothLeProfile"/> class which contains a set of services for a device.
        /// </summary>
        protected BluetoothLeProfile()
        {
            Services = new Dictionary<Guid, BluetoothLeService>();
        }

        /// <summary>
        /// Gets all Bluetooth Low Energy services configured for this profile instance.
        /// </summary>
        public IDictionary<Guid, BluetoothLeService> Services { get; private set; }

        /// <summary>
        /// Gets all Bluetooth Low Energy characteristics configured for this profile instance.
        /// </summary>
        public IDictionary<Guid, BluetoothLeCharacteristic> Characteristics
        {
            get { return _characteristics ?? (_characteristics = Services.SelectMany(s => s.Value.Characteristics.Values).ToDictionary(c => c.UUID)); }
        }

        /// <summary>
        /// Gets all Bluetooth Low Energy services configured for this profile instance, filtered by its <see cref="CharacteristicType"/>.
        /// </summary>
        /// <param name="characteristicType">Type of the characteristic.</param>
        /// <returns></returns>
        public BluetoothLeCharacteristic GetCharacteristicOfType(CharacteristicType characteristicType)
        {
            return Characteristics.Values.FirstOrDefault(c => c.Type == characteristicType);
        }

        /// <summary>
        /// Adds the given service to the current profile.
        /// </summary>
        /// <param name="service">The Bluetooth Low Energy service instance.</param>
        /// <returns></returns>
        public BluetoothLeProfile WithService(BluetoothLeService service)
        {
            Services.Add(service.UUID, service);
            return this;
        }

        /// <summary>
        /// Gets the characteristic as defined by the UUID.
        /// </summary>
        /// <param name="characteristicUuid">The characteristic UUID.</param>
        public BluetoothLeCharacteristic GetCharacteristic(string characteristicUuid)
        {
            var characteristicId = new Guid(characteristicUuid);
            BluetoothLeCharacteristic characteristic;
            if (!Characteristics.TryGetValue(characteristicId, out characteristic))
            {
                return new BluetoothLeCharacteristicUnknown(characteristicId, "Unknown characteristic");
            }
            return characteristic;
        }
    }
}
