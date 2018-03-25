using Assets.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Presentation.Controllers.Gauges
{
    /// <summary>
    /// The magnetometer gauge indicates the measured direction and strength of the magnetic field.
    /// </summary>
    public class MagnetometerGaugeController : GaugeController
    {
        private readonly Image _indicator;
        private readonly Image _compassNeedle;

        public MagnetometerGaugeController(Xdk xdk)
            : base(xdk, null, "MagnetometerGauge")
        {
            _indicator = GameObject.Find("MagneticFieldStrengthIndicator").GetComponent<Image>();
            _compassNeedle = GameObject.Find("CompassNeedle").GetComponent<Image>();
            UseTiltCompensation = true;
        }
        
        private bool UseTiltCompensation { get; set; }

        public override void UpdateStatus()
        {
            var compassOrientation = UseTiltCompensation ? GetTiltCompensatedHeading() : GetHeadingAngle();

            _compassNeedle.rectTransform.rotation = Quaternion.Slerp(_compassNeedle.rectTransform.rotation,
                Quaternion.Euler(0, 0, compassOrientation), Time.deltaTime * 2.0f);

            // for field strength using vector magnitude instead of resistance which seems to always be constant
            var barValue = new Vector3(Xdk.MagnetometerSensor.XAxis.Value, Xdk.MagnetometerSensor.YAxis.Value, Xdk.MagnetometerSensor.ZAxis.Value).magnitude;

            //var barValueMax = new Vector3(Xdk.MagnetometerSensor.Resistance.Max, Xdk.MagnetometerSensor.Resistance.Max, Xdk.MagnetometerSensor.Resistance.Max).magnitude;
            //var barValueMin = new Vector3(Xdk.MagnetometerSensor.Resistance.Min, Xdk.MagnetometerSensor.Resistance.Min, Xdk.MagnetometerSensor.Resistance.Min).magnitude;

            //Debug.Log("Magnetometer field: " + barValue);
            var scaledBar = ScaleValue(barValue, Xdk.MagnetometerSensor.Resistance.Min, Xdk.MagnetometerSensor.Resistance.Max, 123);

            _indicator.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, scaledBar);
        }
        
        /// <summary>
        /// Compensate heading calclation using accelerometer pitch and roll
        /// </summary>
        /// <returns>Heading angle of the compass</returns>
        private float GetTiltCompensatedHeading()
        {
            var accVector = new Vector3(Xdk.AccelerometerSensor.XAxis.Value, Xdk.AccelerometerSensor.YAxis.Value, Xdk.AccelerometerSensor.ZAxis.Value);
            var accOrientation = Quaternion.FromToRotation(new Vector3(0, 0, 1), accVector).eulerAngles;
            var pitch = accOrientation.x;
            var roll = accOrientation.y;

            var magVector = new Vector3(Xdk.MagnetometerSensor.XAxis.Value, Xdk.MagnetometerSensor.YAxis.Value, Xdk.MagnetometerSensor.ZAxis.Value);

            var ayf = roll * Mathf.Deg2Rad;
            var axf = pitch * Mathf.Deg2Rad;
            var xh = magVector.x * Mathf.Cos(ayf) + magVector.y * Mathf.Sin(ayf) * Mathf.Sin(axf) - magVector.z * Mathf.Cos(axf) * Mathf.Sin(ayf);
            var yh = magVector.y * Mathf.Cos(axf) + magVector.z * Mathf.Sin(axf);

            var heading = Mathf.Atan2(yh, xh) * Mathf.Rad2Deg;
            if (heading > 0)
            {
                heading = heading - 360.0f;
            }
            heading += 360.0f;

            return - heading;
        }
        
        /// <summary>
        /// Use magnetometer data only.
        /// </summary>
        /// <returns>Heading angle of the compass</returns>
        private float GetHeadingAngle()
        {
            var x = Xdk.MagnetometerSensor.XAxis.Value;
            var y = Xdk.MagnetometerSensor.YAxis.Value;

            float result;
            if (y > 0)
            {
                result = 90.0f - Mathf.Atan(1.0f * x / y) * Mathf.Rad2Deg;
            }
            else if (y < 0)
            {
                result = 270.0f - Mathf.Atan(1.0f*x/y)*Mathf.Rad2Deg;
            }
            else if (x < 0)
            {
                result = 180.0f;
            }
            else
            {
                result = 0f;
            }
            return - result;
        }
    }
}
