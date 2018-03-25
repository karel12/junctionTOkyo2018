using Assets.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Presentation.Controllers.Gauges
{
    /// <summary>
    /// Visualizes the orientation of the physical device model.
    /// </summary>
    public class RotationGaugeController : GaugeController
    {

        public RotationGaugeController(Xdk xdk, string threeDModelName)
            : base(xdk, null, threeDModelName)
        {
        }


        public override void UpdateStatus()
        {
            // Gyroscope:
            //Debug.Log("gyro Z: " + Xdk.GyroSensor.ZAxis.Value);
            //var gyroscopeVector = new Vector3(Xdk.GyroSensor.YAxis.Value / 1000f, Xdk.GyroSensor.XAxis.Value / 1000f, Xdk.GyroSensor.ZAxis.Value / 1000f);
            //targetGameObject.transform.rotation *= Quaternion.Euler(gyroscopeVector);
            var currentTransform = TargetGameObject.transform;
            TargetGameObject.transform.rotation = Quaternion.Slerp (currentTransform.rotation, Xdk.Attitude, Time.deltaTime * 10f);

            if (Xdk.ShowOrientation)
            {
                var textGameObject = GameObject.Find("PositionValueLabel");
                textGameObject.GetComponent<Text>().text = Xdk.Orientation.ToString();                
            }


        } 
    }
}
