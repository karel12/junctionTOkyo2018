using System;
using System.Collections.Generic;

namespace Assets.Model
{
    /// <summary>
    /// Represents a single sensor value which range can be changed according to given range type and average value
    /// </summary>
    /// <typeparam name="T">The type of value</typeparam>
    public abstract class RangeSensorValue<T> : SensorValue<T> where T : IComparable<T>
    {
        private readonly IDictionary<RangeKind, T> _ranges = new Dictionary<RangeKind, T>();

        private readonly T _defaultMin;
        private readonly T _defaultMax;
        private readonly string _unit;

        /// <summary>
        /// Constructs instance of <see>RangeSensorValue</see> class        
        /// </summary>
        /// <param name="defaultMinValue">Default minimal value</param>
        /// <param name="defaultMaxValue">Default maximal value</param>
        /// <param name="unit">The unit of value (for example %)</param>
        protected RangeSensorValue(T defaultMinValue, T defaultMaxValue, string unit)
        {
            Min = defaultMinValue;
            Max = defaultMaxValue;
            _defaultMin = defaultMinValue;
            _defaultMax = defaultMaxValue;
            _unit = unit;
        }

        /// <summary>
        /// Default minimal value
        /// </summary>
        public T DefaultMin
        {
            get { return _defaultMin; }
        }

        /// <summary>
        /// Default maximal value
        /// </summary>
        public T DefaultMax
        {
            get { return _defaultMax; }
        }

        /// <summary>
        /// The unit of value.
        /// </summary>
        public string Unit
        {
            get { return _unit; }
        }

        /// <summary>
        /// Sets Min and Max properties according to given range type and current value
        /// </summary>
        /// <param name="rangeKind">The type of range</param>        
        public void SetRange(RangeKind rangeKind)
        {
            var rangeValue = _ranges[rangeKind];
            Min = Minus(Value, rangeValue);
            Max = Plus(Value, rangeValue);
            // check if we are outside the (DefaultMin, DefaultMax)
            if (Min.CompareTo(DefaultMin) < 0)
            {
                Min = DefaultMin;
            }
            if (Max.CompareTo(DefaultMax) > 0)
            {
                Max = DefaultMax;
            }
        }


        /// <summary>
        /// Reset Min und Max properties to default values
        /// </summary>
        public void Reset()
        {
            Min = DefaultMin;
            Max = DefaultMax;
        }

        /// <summary>
        /// Adds a range to the maintained ranges.
        /// </summary>
        /// <param name="rangeKind">The kind of range</param>
        /// <param name="rangeValue">The value of range</param>
        public void AddRange(RangeKind rangeKind, T rangeValue)
        {
            _ranges.Add(rangeKind, rangeValue);
        }

        /// <summary>
        /// Gets the value of given range
        /// </summary>
        /// <param name="rangeKind">The range</param>
        /// <returns></returns>
        public T GetRangeValue(RangeKind rangeKind)
        {
            return _ranges[rangeKind];
        }


        // in C# generic cannot be constrained by operators (they are internally static functions)
        // so we need to perform the math in the derived classes

        protected abstract T Average(T minValue, T maxValue);

        protected abstract T Plus(T value1, T value2);

        protected abstract T Minus(T value1, T value2);


    }
}