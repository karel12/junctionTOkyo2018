using Assets.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Presentation.Controllers.Gauges
{
    /// <summary>
    /// Indicates the measured temperature.
    /// </summary>
    public class ThermometerGaugeController : GaugeController
    {
        
        private readonly Image _indicator;
        
        public ThermometerGaugeController(Xdk xdk)
            : base(xdk, null, "ThermometerGauge")
        {
            _indicator = GameObject.Find("TemperatureIndicator").GetComponent<Image>();
        }

        public override void UpdateStatus()
        {
             var sensorValue = ScaleValue(
                    Xdk.Environment.Temperature.Value,
                    Xdk.Environment.Temperature.Min,
                    Xdk.Environment.Temperature.Max, 96);                 

            _indicator.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sensorValue);
        }
    }
}
