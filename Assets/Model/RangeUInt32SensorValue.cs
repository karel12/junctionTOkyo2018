using System;

namespace Assets.Model
{
    public class RangeUInt32SensorValue : RangeSensorValue<UInt32>
    {
        public RangeUInt32SensorValue(uint defaultMinValue, uint defaultMaxValue, string unit) : base(defaultMinValue, defaultMaxValue, unit)
        {
        }

        protected override uint Average(uint minValue, uint maxValue)
        {
            return (minValue + maxValue)/2;
        }

        protected override uint Plus(uint value1, uint value2)
        {
            return value1 + value2;
        }

        protected override uint Minus(uint value1, uint value2)
        {
            return value1 - value2;
        }
    }
}