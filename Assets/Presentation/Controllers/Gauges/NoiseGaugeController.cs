using System;
using System.Collections.Generic;
using Assets.Model;
using UnityEngine;

namespace Assets.Presentation.Controllers.Gauges
{
    /// <summary>
    /// This controller indicates the measured noise level.
    /// </summary>
    public class NoiseGaugeController : MultiValueGaugeController<byte>
    {
        public NoiseGaugeController(Xdk xdk, IEnumerable<Sprite> sprites) : base(xdk, sprites, "speaker_", "NoiseGauge")
        {
            
        }

        protected override Func<Xdk, SensorValue<byte>> SensorSelector
        {
            get { return xda => xda.NoiseSensor.DbSpl; }
        }

        protected override float ScaleValue(SensorValue<byte> sensorValue)
        {
            return ScaleValue(sensorValue.Value, sensorValue.Min, sensorValue.Max);
        }
    }
}
