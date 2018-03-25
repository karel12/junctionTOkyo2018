using Assets.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Presentation.Controllers.Gauges
{
    /// <summary>
    /// The manometer indicates the measured air pressure.
    /// </summary>
    public class ManometerGaugeController : GaugeController
    {
        private readonly Image _manometerNeedle;
        public ManometerGaugeController(Xdk xdk)
            : base(xdk, null, "ManometerGauge")
        {
            _manometerNeedle = GameObject.Find("ManometerNeedle").GetComponent<Image>();
        }

        public override void UpdateStatus()
        {
            var value = 90f - ScaleValue(Xdk.Environment.Pressure.Value, Xdk.Environment.Pressure.Min, Xdk.Environment.Pressure.Max, 180);

            _manometerNeedle.rectTransform.rotation = Quaternion.Slerp(_manometerNeedle.rectTransform.rotation,
                Quaternion.Euler(0, 0, value), Time.deltaTime * 2.0f);
        }
    }
}
