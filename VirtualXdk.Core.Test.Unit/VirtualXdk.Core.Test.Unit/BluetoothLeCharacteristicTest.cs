using System;
using Assets.Model;
using Assets.Utils;
using NUnit.Framework;

namespace VirtualXdk.Core.Test.Unit
{
    [TestFixture]
    public class BluetoothLeCharacteristicTest
    {
        [Test]
        public void WhenSettingByteValueCharacteristic_ShouldSetByteValueInDeviceModel()
        {
            // Arrange

            var guid = Guid.NewGuid();
            var characteristic = new BluetoothLeCharacteristic<Xdk>(guid, "TestValue").WithSetter((Xdk, value) => Xdk.NoiseSensor.DbSpl.Value = value.TryGetByte());
            var testXdk = new Xdk();
            var byteArray = new byte[] {8};
            
            // Act
            characteristic.SetValue(testXdk, byteArray);

            // Assert
            Assert.That(testXdk.NoiseSensor.DbSpl.Value, Is.EqualTo(8));
        }

        [Test]
        public void WhenSettingInt16ValueCharacteristic_ShouldSetInt16ValueInDeviceModel()
        {
            // Arrange

            var guid = Guid.NewGuid();
            var characteristic = new BluetoothLeCharacteristic<Xdk>(guid, "TestValue").WithSetter((Xdk, value) => Xdk.GyroSensor.XAxis.Value = value.TryGetInt16());
            var testXdk = new Xdk();
            const Int16 intValue = Int16.MaxValue;
            var byteArray = BitConverter.GetBytes(intValue);

            // Act
            characteristic.SetValue(testXdk, byteArray);

            // Assert
            Assert.That(testXdk.GyroSensor.XAxis.Value, Is.EqualTo(intValue));
        }

        [Test]
        public void WhenSettingUInt32ValueCharacteristic_ShouldSetUInt32ValueInDeviceModel()
        {
            // Arrange

            var guid = Guid.NewGuid();
            var characteristic = new BluetoothLeCharacteristic<Xdk>(guid, "TestValue").WithSetter((Xdk, value) => Xdk.LightSensor.MilliLux.Value = value.TryGetUInt32());
            var testXdk = new Xdk();
            const UInt32 intValue = UInt32.MaxValue;
            var byteArray = BitConverter.GetBytes(intValue);

            // Act
            characteristic.SetValue(testXdk, byteArray);

            // Assert
            Assert.That(testXdk.LightSensor.MilliLux.Value, Is.EqualTo(intValue));
        }
    }
}
