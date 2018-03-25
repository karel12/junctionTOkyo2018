namespace Assets.Model
{
    /// <summary>
    /// Represents a single sensor value which is only valid within a given range.
    /// </summary>
    /// <typeparam name="T">The value type</typeparam>
    public class SensorValue<T>
    {
        /// <summary>
        /// Gets or sets the value for the sensor.
        /// </summary>
        public T Value { get; set; }
        
        /// <summary>
        /// Gets or sets the valid minimum of the value.
        /// </summary>
        public T Min { get; set; }

        /// <summary>
        /// Gets or sets the valid maximum of the value.
        /// </summary>
        public T Max { get; set; }
    }
}
