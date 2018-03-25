namespace Assets.Model
{
    public class RangeInt32SensorValue : RangeSensorValue<int>
    {
        public RangeInt32SensorValue(int defaultMinValue, int defaultMaxValue, string unit) : base(defaultMinValue, defaultMaxValue, unit)
        {
        }

        protected override int Average(int minValue, int maxValue)
        {
            return (minValue + maxValue) / 2;
        }

        protected override int Plus(int value1, int value2)
        {
            return value1 + value2;
        }

        protected override int Minus(int value1, int value2)
        {
            return value1 - value2;
        }
    }
}