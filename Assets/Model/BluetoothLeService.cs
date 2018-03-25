using System;
using System.Collections.Generic;

namespace Assets.Model
{
    /// <summary>
    /// Represents a Bluetooth Low Energy Service containing a set of characteristics.
    /// </summary>
    public class BluetoothLeService : BluetoothLeItem
    {
        /// <summary>
        /// Gets all Bluetooth Low Energy characteristics configured for this service.
        /// </summary>
        public Dictionary<Guid, BluetoothLeCharacteristic> Characteristics { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothLeService"/> class, which contains a set of characteristics.
        /// </summary>
        /// <param name="uuid">The UUID used to identify the instance of the item.</param>
        /// <param name="displayName">A human readable display name of the item.</param>
        public BluetoothLeService(Guid uuid, string displayName) : base(uuid, displayName)
        {
            Characteristics = new Dictionary<Guid, BluetoothLeCharacteristic>();
        }

        /// <summary>
        /// Adds the given Bluetooth Low Enery characteristic to this service.
        /// </summary>
        /// <param name="characteristic">The characteristic.</param>
        /// <returns></returns>
        public BluetoothLeService WithCharacteristic(BluetoothLeCharacteristic characteristic)
        {
            Characteristics.Add(characteristic.UUID, characteristic);
            characteristic.Service = this;
            return this;
        }
    }
}
