using System;
using Assets.Model;

namespace Assets.Utils
{
    /// <summary>
    /// Used to simulate snesor messages when no Bluetooth device is available.
    /// </summary>
    public class MockLogFilePlayer : ILogPlayer
    {
        private DateTime _timeStamp = DateTime.Now;
        private readonly TimeSpan _messageTimeSpan = TimeSpan.FromMilliseconds(125);
        private readonly Xdk _mockXdk = new Xdk();
        
        // private readonly Random _random = new Random();
        
        private int _messagePhase;
        private int _currentValue;

        public string TryPlayNextMessage()
        {
            if (DateTime.Now - _timeStamp > _messageTimeSpan)
            {
                return PlayNextMessage();
            }
			return null;
		}

        private string PlayNextMessage()
        {
            SetCurrentValue(_mockXdk.AccelerometerSensor.XAxis, -100, 100);
            SetCurrentValue(_mockXdk.AccelerometerSensor.YAxis, -100, 100);
            SetCurrentValue(_mockXdk.AccelerometerSensor.ZAxis, -100, 100);
            SetCurrentValue(_mockXdk.GyroSensor.XAxis, -100, 100);
            SetCurrentValue(_mockXdk.GyroSensor.YAxis, -100, 100);
            SetCurrentValue(_mockXdk.GyroSensor.ZAxis, -100, 100);

            _mockXdk.Orientation.X = 0f;
            _mockXdk.Orientation.Y = 0f;
            _mockXdk.Orientation.Z = 0f;
            _mockXdk.Orientation.W = 0f;

            SetCurrentValue(_mockXdk.MagnetometerSensor.XAxis, -100, 100);
            SetCurrentValue(_mockXdk.MagnetometerSensor.YAxis, -100, 100);
            SetCurrentValue(_mockXdk.MagnetometerSensor.ZAxis, -100, 100);
            SetCurrentValue(_mockXdk.MagnetometerSensor.Resistance);
            SetCurrentValue(_mockXdk.LightSensor.Lux);
            SetCurrentValue(_mockXdk.NoiseSensor.DbSpl);
            _mockXdk.Environment.Temperature.Value = 23500;
            //SetCurrentValue(mockXdk.Environment.Temperature);
            _mockXdk.Environment.Humidity.Value = 50;
            //SetCurrentValue(mockXdk.Environment.Humidity);
            SetCurrentValue(_mockXdk.Environment.Pressure);
            SetLedsValues(_mockXdk);
            SetButtonValues(_mockXdk);
            _mockXdk.FirmwareVersion = "2";

            byte[] messagePayload;
            if (_messagePhase == 1)
            {
                messagePayload = _mockXdk.ToLowPriorityMessage1();
				_mockXdk.IsSdCardInserted.Value = (_mockXdk.IsSdCardInserted.Value == 0 ? (byte)1 : (byte)0);
            }
            else if (_messagePhase == 3)
            {
                _mockXdk.IsSdCardInserted.Value = 1;
                messagePayload = _mockXdk.ToLowPriorityMessage2();
            }
            else
            {
                messagePayload = _mockXdk.ToHighPriorityMessage();
            }

            var message = string.Concat(
                Constants.CentralDidUpdateValueForCharacteristic,
                Constants.MessageSeparator,
                (_messagePhase == 0) ? Constants.HighPriorityCharacteristic : Constants.LowPriorityCharacteristic,
                Constants.MessageSeparator,
                Convert.ToBase64String(messagePayload));

            _messagePhase++;
            if (_messagePhase > 3)
            {
                _messagePhase = 0;
                _currentValue++;
            }

            _timeStamp = DateTime.Now;

            return message;
        }

        private void SetButtonValues(Xdk xdk)
        {
            var currentSecond = DateTime.Now.Second;
            xdk.IsButton2Pressed = currentSecond % 2 == 0;
        }

        private void SetLedsValues(Xdk xdk)
        {
            var currentSecond = DateTime.Now.Second;
            for (int i = 0; i < Xdk.LedCount; i++)
            {
                xdk.Leds[i].IsOn = currentSecond % (i + 2) == 0;
            }            
        }

        private void SetCurrentValue<T>(SensorValue<T> sensorValue)
        {
            var changeType = Convert.ChangeType(sensorValue.Min, typeof(int));
            if (changeType != null)
            {
                int min = (int)changeType;
                // ReSharper disable once PossibleNullReferenceException
                int max = (int)Convert.ChangeType(sensorValue.Max, typeof(int));
                SetCurrentValue(sensorValue, min, max + 1);
            }
        }

        private void SetCurrentValue<T>(SensorValue<T> sensorValue, int min, int max)
        {
            float current = (_currentValue % 11) * ((max - min) / 10) + min;
            sensorValue.Value = (T)Convert.ChangeType(current, typeof(T));
        }

        //private void SetRandomValue<T>(SensorValue<T> sensorValue)
        //{
        //    int min = (int)Convert.ChangeType(sensorValue.Min, typeof (int));
        //    int max = (int)Convert.ChangeType(sensorValue.Max, typeof (int));
        //    SetRandomValue(sensorValue, min, max);
        //}

        //private void SetRandomValue<T>(SensorValue<T> sensorValue, int min, int max)
        //{
        //    sensorValue.Value = (T)Convert.ChangeType(_random.Next(min, max), typeof(T));
        //}
    }
}
