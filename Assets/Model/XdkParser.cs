using System;
using System.IO;

namespace Assets.Model
{
    /// <summary>
    /// This class is used to apply XDK specific message parsing to given byte arrays retreived from characteristics.
    /// </summary>
    public static class XdkParser
    {
        private const byte Message1Identifier = 0x01;
        private const byte Message2Identifier = 0x02;

        /// <summary>
        /// Sets the high priority sensor values which are supposed to be updated frequently.
        /// </summary>
        /// <param name="message">The message payload.</param>
        /// <param name="xdk">The XDK device model instance.</param>
        public static void SetHighPriorityValues(this byte[] message, Xdk xdk)
        {
            if (message == null || message.Length < 12)
            {
                return;
            }

            using (var memoryStream = new MemoryStream(message) {Position = 0})
            using (var stream = new BinaryReader(memoryStream))
            {
                if (xdk.UseBuiltInSensorFusion)
                {
                    xdk.Orientation.W =  stream.ReadSingle();
                    xdk.Orientation.X =  stream.ReadSingle();
                    xdk.Orientation.Y =  stream.ReadSingle();
                    xdk.Orientation.Z =  stream.ReadSingle();
                }
                else
                {
                    xdk.AccelerometerSensor.XAxis.Value = stream.ReadInt16();
                    xdk.AccelerometerSensor.YAxis.Value = stream.ReadInt16();
                    xdk.AccelerometerSensor.ZAxis.Value = stream.ReadInt16();
                    xdk.GyroSensor.XAxis.Value = stream.ReadInt16();
                    xdk.GyroSensor.YAxis.Value = stream.ReadInt16();
                    xdk.GyroSensor.ZAxis.Value = stream.ReadInt16();
                    xdk.CalculatePositionUsingSensorFusion();
                }
            }
            
        }

        /// <summary>
        /// Retreives a high priority message from the current state of the XDK device model.
        /// </summary>
        /// <param name="xdk">The XDK device model instance.</param>
        public static byte[] ToHighPriorityMessage(this Xdk xdk)
        {
            using (var memoryStream = new MemoryStream())
            {
                
                using (var stream = new BinaryWriter(memoryStream))
                {
                    if (xdk.UseBuiltInSensorFusion)
                    {
                        stream.Write(xdk.Orientation.W);
                        stream.Write(xdk.Orientation.X);
                        stream.Write(xdk.Orientation.Y);
                        stream.Write(xdk.Orientation.Z);
                    }
                    else
                    {
                        stream.Write(xdk.AccelerometerSensor.XAxis.Value);
                        stream.Write(xdk.AccelerometerSensor.YAxis.Value);
                        stream.Write(xdk.AccelerometerSensor.ZAxis.Value);
                        stream.Write(xdk.GyroSensor.XAxis.Value);
                        stream.Write(xdk.GyroSensor.YAxis.Value);
                        stream.Write(xdk.GyroSensor.ZAxis.Value);
                    }

                    
                    memoryStream.Position = 0;
                    return memoryStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Updates the low priority values of the given message in the given instance of the XDK.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="xdk">The XDK.</param>
        public static void SetLowPriorityValues(this byte[] message, Xdk xdk)
        {
            if (message == null || message.Length == 0)
            {
                return;
            }

			using (var memoryStream = new MemoryStream(message) {Position = 0})
            using (var stream = new BinaryReader(memoryStream))
            {
                byte messageId = stream.ReadByte();
                switch (messageId)
                {
                    case Message1Identifier:
                        SetLowPriorityMessage1Values(stream, xdk);
                        break;
                    case Message2Identifier:
                        SetLowPriorityMessage2Values(stream, xdk);
                        break;
                }
            }
        }

        [Flags]
        private enum BitMask
        {
            Button1 = 0x01,
            Button2 = 0x02,
        }

        private static void SetLowPriorityMessage1Values(BinaryReader stream, Xdk xdk)
        {
            xdk.LightSensor.Lux.Value = Convert.ToInt32(stream.ReadUInt32()) / 1000;
            xdk.NoiseSensor.DbSpl.Value = stream.ReadByte();
            xdk.Environment.Pressure.Value = Convert.ToInt32(stream.ReadUInt32());
            xdk.Environment.Temperature.Value = stream.ReadInt32();
            xdk.Environment.Humidity.Value = Convert.ToInt32(stream.ReadUInt32());
            xdk.IsSdCardInserted.Value = stream.ReadByte();
            byte readByte = stream.ReadByte();
            var bitMask = (BitMask)readByte;
            xdk.IsButton1Pressed = (bitMask & BitMask.Button1) == BitMask.Button1;
            xdk.IsButton2Pressed = (bitMask & BitMask.Button2) == BitMask.Button2;
        }

        /// <summary>
        /// Retreives a low priority message 1 from the current state of the XDK device model.
        /// </summary>
        /// <param name="xdk">The XDK device model instance.</param>
        public static byte[] ToLowPriorityMessage1(this Xdk xdk)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var stream = new BinaryWriter(memoryStream))
                {
                    stream.Write(Message1Identifier);
                    stream.Write(Convert.ToUInt32(xdk.LightSensor.Lux.Value * 1000));
                    stream.Write(xdk.NoiseSensor.DbSpl.Value);
                    stream.Write(Convert.ToUInt32(xdk.Environment.Pressure.Value));
                    stream.Write(xdk.Environment.Temperature.Value);
                    stream.Write(Convert.ToUInt32(xdk.Environment.Humidity.Value));
                    stream.Write(xdk.IsSdCardInserted.Value);
                    byte buttonByte = 0;
                    SetBitInByte(ref buttonByte, 0, xdk.IsButton1Pressed);
                    SetBitInByte(ref buttonByte, 1, xdk.IsButton2Pressed);
                    stream.Write(buttonByte);

                    memoryStream.Position = 0;
                    return memoryStream.ToArray();
                }
            }
        }

        private static void SetLowPriorityMessage2Values(BinaryReader stream, Xdk xdk)
        {
            xdk.MagnetometerSensor.XAxis.Value = stream.ReadInt16();
            xdk.MagnetometerSensor.YAxis.Value = stream.ReadInt16();
            xdk.MagnetometerSensor.ZAxis.Value = stream.ReadInt16();
            xdk.MagnetometerSensor.Resistance.Value = stream.ReadInt16();

            byte readByte = stream.ReadByte();

            for (int i = 0; i < Xdk.LedCount; i++)
            {
                xdk.Leds[i].IsOn = GetBitInByte(readByte, i);
            }

            if (xdk.UseBuiltInSensorFusion)
            {
                xdk.AccelerometerSensor.XAxis.Value = stream.ReadInt16();
                xdk.AccelerometerSensor.YAxis.Value = stream.ReadInt16();
                xdk.AccelerometerSensor.ZAxis.Value = stream.ReadInt16();
            }


        }

        /// <summary>
        /// Retreives a low priority message 2 from the current state of the XDK device model.
        /// </summary>
        /// <param name="xdk">The XDK device model instance.</param>
        public static byte[] ToLowPriorityMessage2(this Xdk xdk)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var stream = new BinaryWriter(memoryStream))
                {
                    stream.Write(Message2Identifier);
                    stream.Write(xdk.MagnetometerSensor.XAxis.Value);
                    stream.Write(xdk.MagnetometerSensor.YAxis.Value);
                    stream.Write(xdk.MagnetometerSensor.ZAxis.Value);
                    stream.Write(xdk.MagnetometerSensor.Resistance.Value);

                    byte ledByte = 0;
                    for (int i = 0; i < Xdk.LedCount; i++)
                    {
                        SetBitInByte(ref ledByte, i, xdk.Leds[i].IsOn);
                    }
                    stream.Write(ledByte);
                    memoryStream.Position = 0;
                    return memoryStream.ToArray();
                }
            }
        }

        private static void SetBitInByte(ref byte byteValue, int pos, bool value)
        {
            if (value)
            {
                // left-shift 1, then bitwise OR
                byteValue = (byte)(byteValue | (1 << pos));
            }
            else
            {
                // left-shift 1, then take complement, then bitwise AND
                byteValue = (byte)(byteValue & ~(1 << pos));
            }
        }

        private static bool GetBitInByte(byte byteValue, int pos)
        {
            //left-shift 1, then bitwise AND, then check for non-zero
            return ((byteValue & (1 << pos)) != 0);
        }

    }
}
