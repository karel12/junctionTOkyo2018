using System;
using System.Collections.Generic;
using Assets.Model;
using UnityEngine;

namespace Assets.Presentation.Controllers.Gauges
{
    /// <summary>
    /// The hygrometer indicates the measured humidity.
    /// </summary>
    public class HygrometerGaugeController : MultiValueGaugeController<int>
    {
        public HygrometerGaugeController(Xdk xdk, IEnumerable<Sprite> sprites)
            : base(xdk, sprites, "water_", "HygrometerGauge")
        {

        }

        protected override Func<Xdk, SensorValue<int>> SensorSelector
        {
            get { return xda => xda.Environment.Humidity; }
        }

        protected override float ScaleValue(SensorValue<int> sensorValue)
        {
            return ScaleValue(sensorValue.Value, sensorValue.Min, sensorValue.Max, Sprites.Length - 1);
        }
    }
}
