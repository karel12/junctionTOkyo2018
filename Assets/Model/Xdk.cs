using System;
using UnityEngine;

namespace Assets.Model
{
    /// <summary>
    /// The device model representing the physical XDK.
    /// </summary>
    public class Xdk : BluetoothLeDevice
    {
        public const string AppProtocolVersion = "fsdfsdf";

        public bool UseBuiltInSensorFusion = true;

        private readonly SensorFusion _sensorFusion = new SensorFusion();

        private bool _isButton1Pressed;

        public Accelerometer AccelerometerSensor { get; private set; }

        public GyroSensor GyroSensor { get; private set; }

        public LightSensor LightSensor { get; private set; }

        public NoiseSensor NoiseSensor { get; private set; }

        public Magnetometer MagnetometerSensor { get; private set; }

        public EnvironmentSensor Environment { get; private set; }

        public Quaternion CalculatedAttitude { get; private set; }

        public Orientation Orientation { get; private set; }

        public Quaternion Attitude
        {
            get { return UseBuiltInSensorFusion ? Orientation.ToQuaternion() : CalculatedAttitude; }
        }


        /// <summary>
        /// Gets or sets the firmware version of the device.
        /// </summary>
        public string FirmwareVersion { get; set; }

        public bool IsProtocolSupported
        {
            get
            {
                if (FirmwareVersion == null) // not connected yet
                {
                    return true;
                }
                return FirmwareVersion == AppProtocolVersion;
            }
        }

        public bool ProtololWasChecked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the sd card is inserted.
        /// </summary>
        /// <value><c>1</c> if the sd card inserted; otherwise, <c>1</c>.</value>
        public SensorValue<byte> IsSdCardInserted { get; set; }

        /// <summary>
        /// Gets or sets the time between notifications in milliseconds.
        /// </summary>
        public SensorValue<uint> NotificationSamplingTime { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that Button1 is currently pressed on physical XDK
        /// </summary>
        public bool IsButton1Pressed
        {
            get { return _isButton1Pressed; }
            set
            {
                if (_isButton1Pressed != value)
                {
                    _isButton1Pressed = value;
                    if (_isButton1Pressed)
                    {
                        ResetOrientation();
                    }
                }
            }
        }

        public void ResetOrientation()
        {
            Orientation.Reset(false);
            if (OnResetOrientation != null)
            {
                OnResetOrientation();
            }
        }

        public bool IsButton2Pressed { get; set; }

        public const int LedCount = 3;

        public Led[] Leds { get; private set; }

        public bool ShowOrientation { get; set; }

        public Action OnResetOrientation { get; set; }


        public Xdk()
        {
            AccelerometerSensor = new Accelerometer();
            GyroSensor = new GyroSensor();
            LightSensor = new LightSensor();
            NoiseSensor = new NoiseSensor();
            MagnetometerSensor = new Magnetometer();
            Environment = new EnvironmentSensor();
            DisplayName = "XDK";
            NotificationSamplingTime = new SensorValue<uint> { Value = 100, Min = 20, Max = 1000 };
            IsSdCardInserted = new SensorValue<byte> { Min = 0, Max = 1 };
            CalculatedAttitude = new Quaternion();
            Orientation = new Orientation();
            Leds = new Led[LedCount];
            for (int i = 0; i < LedCount; i++)
            {
                Leds[i] = new Led();
            }
            Leds[0].Color = LedColor.Yellow;
            Leds[1].Color = LedColor.Orange;
            Leds[2].Color = LedColor.Red;
        }

        public void Reset()
        {
            if (UseBuiltInSensorFusion)
            {
                Orientation.Reset(true);
            }
            else
            {
                _sensorFusion.Reset();
            }

        }

        public void CalculatePositionUsingSensorFusion()
        {
            CalculatedAttitude = _sensorFusion.CalculatePosition(this);
        }

        public void ResetProtocolVersion()
        {
            FirmwareVersion = null;
        }
    }

    public class Orientation
    {
        private Quaternion? _offset;

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Orientation()
        {
            SpaceConversion = Quaternion.Euler(0, 0, 90);
                // new Quaternion(Angle, 0, 0, -Angle);
            AllowReset = true;
        }

        public Quaternion ToQuaternion()
        {
            if (CoordinatesAreValid())
            {
                if (!_offset.HasValue)
                {
                    Reset(false);
                }

                var quaternionFromXdk = new Quaternion(X, Y, Z, W);
                var quaternion = ConvertToUnity(quaternionFromXdk);
                if (_offset.HasValue)
                {
                    return _offset.Value * quaternion;
                }
                return quaternion;
            }
            return Quaternion.identity;
        }


        // space conversion between XDK and Unity coordinate systems:
        // we rotate X, Y, Z around (0, 0, 1) vector  about  pi/2 degrees
        public Quaternion SpaceConversion { get; set; }

        private Quaternion ConvertToUnity(Quaternion quaternion)
        {
            return quaternion * SpaceConversion;
        }

        private bool CoordinatesAreValid()
        {
            return !float.IsNaN(X) && !float.IsNaN(Y) && !float.IsNaN(Z) && !float.IsNaN(W);
        }

        private bool CoordinatesAreNotZero()
        {            
            // ReSharper disable CompareOfFloatsByEqualityOperator - rounding errors are ok here
            return X != 0f && Y != 0f && Z != 0f && W != 0f;
            // ReSharper enable CompareOfFloatsByEqualityOperator
        }

        public void Reset(bool removeOffset)
        {
            if (removeOffset)
            {
                _offset = null;
            }
            else
            {                
                if (CoordinatesAreValid() && CoordinatesAreNotZero() && AllowReset)
                {
                    var orientationNow = ConvertToUnity(new Quaternion(X, Y, Z, W));
                    _offset = Quaternion.identity * Quaternion.Inverse(orientationNow);
                }
            }
        }

        public bool AllowReset { get; set; }

        public Quaternion? Offset
        {
            get { return _offset; }
        }

        public override string ToString()
        {
            var quat = ToQuaternion();

            return string.Format("xdk:   {0} : {4}\n" +
                                 "space: {1} : {5}\n" +
                                 "reset: {2}\n" +
                                 "unity: {3} : {6}",
                                 new Quaternion(X, Y, Z, W),
                                 SpaceConversion,
                                 _offset.HasValue ? _offset.ToString() : "not active",
                                 quat,
                                 new Quaternion(X, Y, Z, W).eulerAngles,
                                 SpaceConversion.eulerAngles,
                                 quat.eulerAngles
                                 );
        }

        public void RotateZ(int angle)
        {
            SpaceConversion = Quaternion.Euler(SpaceConversion.eulerAngles.x, SpaceConversion.eulerAngles.y, angle);
        }


        public void RotateX(int value)
        {
            SpaceConversion = Quaternion.Euler(value, SpaceConversion.eulerAngles.y, SpaceConversion.eulerAngles.z);
        }

        public void RotateY(int value)
        {
            SpaceConversion = Quaternion.Euler(SpaceConversion.eulerAngles.x, value, SpaceConversion.eulerAngles.z);
        }

        public void OffsetX(int value)
        {
            if (!AllowReset)
            {
                return;
            }
            var currentOffset = _offset == null ? Quaternion.identity.eulerAngles : _offset.Value.eulerAngles;
            _offset = Quaternion.Euler(value, currentOffset.y, currentOffset.z);
        }

        public void OffsetY(int value)
        {
            if (!AllowReset)
            {
                return;
            }
            var currentOffset = _offset == null ? Quaternion.identity.eulerAngles : _offset.Value.eulerAngles;
            _offset = Quaternion.Euler(currentOffset.x, value, currentOffset.z);
        }

        public void OffsetZ(int value)
        {
            if (!AllowReset)
            {
                return;
            }
            var currentOffset = _offset == null ? Quaternion.identity.eulerAngles : _offset.Value.eulerAngles;
            _offset = Quaternion.Euler(currentOffset.x, currentOffset.y, value);
        }


    }

    public enum LedColor
    {
        Red,
        Green,
        Yellow,
        Orange
    }

    public static class LedColorExtensions
    {
        private static readonly Color GreenColor = new Color(127 / 255f, 1, 0);

        private static readonly Color RedColor = new Color(1, 69 / 255f, 0);

        private static readonly Color YellowColor = new Color(1, 1, 0);
        
        private static readonly Color OrangeColor = new Color(1, 140 / 255f, 0);

        public static Color ToUnityColor(this LedColor ledColor)
        {
            switch (ledColor)
            {
                case LedColor.Yellow:
                    return YellowColor;                

                case LedColor.Orange:
                    return OrangeColor;

                case LedColor.Red:
                    return RedColor;

                case LedColor.Green:
                    return GreenColor;
                
                default:
                    throw new ArgumentOutOfRangeException(ledColor.ToString());
            }
        }
    }

    public class Led
    {
        public bool IsOn { get; set; }
        public LedColor Color { get; set; }
    }

    public class Accelerometer
    {
        public Accelerometer()
        {
            XAxis = new SensorValue<short>();
            YAxis = new SensorValue<short>();
            ZAxis = new SensorValue<short>();
        }
        public SensorValue<Int16> XAxis { get; set; }
        public SensorValue<Int16> YAxis { get; set; }
        public SensorValue<Int16> ZAxis { get; set; }
    }

    public class GyroSensor
    {

        public GyroSensor()
        {
            XAxis = new SensorValue<short>();
            YAxis = new SensorValue<Int16>();
            ZAxis = new SensorValue<Int16>();
        }

        public SensorValue<Int16> XAxis { get; set; }
        public SensorValue<Int16> YAxis { get; set; }
        public SensorValue<Int16> ZAxis { get; set; }
    }

    public class LightSensor
    {
        public LightSensor()
        {
            Lux = new RangeInt32SensorValue(0, 2000, "lux");
            Lux.AddRange(RangeKind.Small, 1);
            Lux.AddRange(RangeKind.Medium, 10);
            Lux.AddRange(RangeKind.Big, 100);
        }

        public RangeInt32SensorValue Lux { get; set; }
    }

    public class NoiseSensor
    {
        public NoiseSensor()
        {
            DbSpl = new SensorValue<byte> { Min = 0, Max = 100 };
        }

        public SensorValue<byte> DbSpl { get; set; }
    }

    public class Magnetometer
    {
        public Magnetometer()
        {
            XAxis = new SensorValue<Int16>();
            YAxis = new SensorValue<Int16>();
            ZAxis = new SensorValue<Int16>();
            Resistance = new SensorValue<Int16> { Min = 0, Max = 2000 };
        }

        public SensorValue<Int16> XAxis { get; set; }
        public SensorValue<Int16> YAxis { get; set; }
        public SensorValue<Int16> ZAxis { get; set; }
        public SensorValue<Int16> Resistance { get; set; }
    }

    public class EnvironmentSensor
    {
        public EnvironmentSensor()
        {
            Pressure = new RangeInt32SensorValue(80000, 120000, "Pa");
            Pressure.AddRange(RangeKind.Small, 20);
            Pressure.AddRange(RangeKind.Medium, 500);
            Pressure.AddRange(RangeKind.Big, 10000);


            Temperature = new RangeInt32SensorValue(-10000, 50000, "°C"); // in millidegs
            Temperature.AddRange(RangeKind.Small, 1000);
            Temperature.AddRange(RangeKind.Medium, 5000);
            Temperature.AddRange(RangeKind.Big, 10000);

            Humidity = new RangeInt32SensorValue(0, 100, "%");
            Humidity.AddRange(RangeKind.Small, 5);
            Humidity.AddRange(RangeKind.Medium, 15);
            Humidity.AddRange(RangeKind.Big, 40);

        }

        public RangeInt32SensorValue Pressure { get; set; }
        public RangeInt32SensorValue Temperature { get; set; }
        public RangeInt32SensorValue Humidity { get; set; }
    }
}
