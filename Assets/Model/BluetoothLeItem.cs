
using System;

namespace Assets.Model
{
    /// <summary>
    /// Base class of all derived Bluetooth Low Energy model configuration classes.
    /// </summary>
    public abstract class BluetoothLeItem
    {
        /// <summary>
        /// The globally unique identifier of the item.
        /// </summary>
        public Guid UUID { get; protected set; }

        /// <summary>
        /// Gets or sets a human readably display name of the item. The display name is not collision free. Do not use this information for unique identification. 
        /// </summary>
        public string DisplayName { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BluetoothLeItem"/> class.
        /// </summary>
        /// <param name="uuid">The UUID used to identify the instance of the item.</param>
        /// <param name="displayName">A human readable display name of the item.</param>
        protected BluetoothLeItem(Guid uuid, string displayName)
        {
            UUID = uuid;
            DisplayName = displayName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return UUID.Equals(((BluetoothLeItem) obj).UUID);
        }

        public override int GetHashCode()
        {
            return UUID.GetHashCode();
        }
    }
}
