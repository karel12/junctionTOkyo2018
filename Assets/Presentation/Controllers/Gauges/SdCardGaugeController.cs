using System;
using System.Collections.Generic;
using Assets.Model;
using UnityEngine;

namespace Assets.Presentation.Controllers.Gauges
{
    /// <summary>
    /// Indicates whether the SD card is currently inserted or not.
    /// </summary>
    public class SdCardGaugeController : MultiValueGaugeController<byte>
    {
        public SdCardGaugeController(Xdk xdk, IEnumerable<Sprite> sprites)
            : base(xdk, sprites, "sdcard_", "SimCardGauge")
        {
        }

        protected override Func<Xdk, SensorValue<byte>> SensorSelector
        {
            get { return xda => xda.IsSdCardInserted; }
        }

        protected override float ScaleValue(SensorValue<byte> sensorValue)
        {
            return sensorValue.Value == 1 ? 0 : 1;
        }
    }
}
