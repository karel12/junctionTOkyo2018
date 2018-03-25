using System;
using System.Collections.Generic;
using Assets.Model;
using UnityEngine;

namespace Assets.Presentation.Controllers.Gauges
{
    /// <summary>
    /// The light gauge indicates the measured brightness.
    /// </summary>
    public class LightGaugeController : MultiValueGaugeController<int>
    {
        public LightGaugeController(Xdk xdk, IEnumerable<Sprite> sprites) : base(xdk, sprites, "light_", "LightGauge")
        {
            
        }

        protected override Func<Xdk, SensorValue<int>> SensorSelector
        {
            get { return xda => xda.LightSensor.Lux; }
        }

        protected override float ScaleValue(SensorValue<int> sensorValue)
        {
            return ScaleValue(sensorValue.Value, sensorValue.Min, sensorValue.Max, Sprites.Length - 1);
            //return ScaleValue(sensorValue.Value, sensorValue.Min, sensorValue.Max);
        }
    }
}
