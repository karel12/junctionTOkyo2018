using System;
using UnityEngine;

namespace Assets.Model
{
    /// <summary>
    /// Class responsible for computng an accurate device orientation from gyro acc and mag data.
    /// Theory:
    /// Gyro readings are stable but because of integration they suffer from drift. 
    /// In order to reduce gyro drift the orientation can be also computed from accelerometer and magnetometer data (but it is noisy)
    /// This additional accMag data is passed through a complementary filter together with the gyro data and a smooth orientation is fused (low drift and noise)
    /// </summary>
    public class SensorFusion
    {
        private const float GyroRawAngularSpeedFactor = 15267f;
        private bool _initState = true;
        private DateTime _timeStamp;
        private Quaternion _gyroAttitude = Quaternion.identity;
     
        /// <summary>
        /// Calculates the new orientation of the model based on the xdk sensor values.
        /// </summary>
        /// <param name="xdk"></param>
        /// <returns></returns>
        public Quaternion CalculatePosition(Xdk xdk)
        {
            // integrate gyro and compute new attitude
            _gyroAttitude *= GetGyroDeltaRotation(new Vector3(xdk.GyroSensor.YAxis.Value, -1f * xdk.GyroSensor.XAxis.Value, xdk.GyroSensor.ZAxis.Value));

            // build acc and mag vectors
            var accelerometerVector = new Vector3(xdk.AccelerometerSensor.YAxis.Value, -1f * xdk.AccelerometerSensor.XAxis.Value, xdk.AccelerometerSensor.ZAxis.Value);

            // get quaternion attitude from the vector (orinetation from vector)
            var accelerometerAttitude = Quaternion.FromToRotation(new Vector3(0, 0, 1), accelerometerVector);
         
            // complementary filter wit 0.98 as filter coefficient
            const float filterCoefficient = 0.98f;
            var attitude = Quaternion.Euler(ComplimentaryFilter(_gyroAttitude.eulerAngles, accelerometerAttitude.eulerAngles, new Vector3(filterCoefficient, filterCoefficient, filterCoefficient)));

            return attitude;
        }

        /// <summary>
        /// Resets the position of the xdk
        /// </summary>
        public void Reset()
        {
            _gyroAttitude = Quaternion.identity; 
        }

        /// <summary>
        /// Complimetary filter for two vectors
        /// </summary>
        /// <param name="gyro"></param>
        /// <param name="accMag"></param>
        /// <param name="filterCoefficient"></param>
        /// <returns></returns>
        private static Vector3 ComplimentaryFilter(Vector3 gyro, Vector3 accMag, Vector3 filterCoefficient)
        {
            var result = Vector3.zero;
            result.x = gyro.x * filterCoefficient.x + accMag.x * (1 - filterCoefficient.x);
            result.y = gyro.y * filterCoefficient.y + accMag.y * (1 - filterCoefficient.y);
            result.z = gyro.z * filterCoefficient.z + accMag.z * (1 - filterCoefficient.z);
            return result;
        }

        /// <summary>
        /// Integrae gyro angular rotation over time to get rotation
        /// </summary>
        /// <param name="gyroSpeedVector"></param>
        /// <returns></returns>
        private Quaternion GetGyroDeltaRotation(Vector3 gyroSpeedVector)
        {
            // initialisation of the gyroscope based rotation matrix
            if (_initState)
            {
                _timeStamp = DateTime.Now;
                _initState = false;
                return Quaternion.identity;
            }

            var current = DateTime.Now;
            var deltaTime = (float)(current - _timeStamp).TotalMilliseconds;
            _timeStamp = current;
            var deltaRotation = gyroSpeedVector * deltaTime / GyroRawAngularSpeedFactor;
            return Quaternion.Euler(deltaRotation);
        }

    }
}