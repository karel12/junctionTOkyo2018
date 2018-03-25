using Assets.Core;
using Assets.Model;
using Assets.Utils;
using NUnit.Framework;

namespace VirtualXda.Core.Test.Unit
{
    [TestFixture]
    public class XdaParserTest
    {
        [Test]
        public void WhenSlicingFirstTwoBytes_ShouldReturnCorrectResult()
        {
            // Arrange
            var sevenItems = new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70 };

            // Act & Assert
            sevenItems.Slice(2, result => Assert.That(result.Length == 2 && result[0] == 0x10 && result[1] == 0x20));
        }

        [Test]
        public void WhenSlicingSecondTwoBytes_ShouldReturnCorrectResult()
        {
            // Arrange
            var sevenItems = new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70 };

            // Act & Assert
            sevenItems.Slice(2).Slice(2, result => Assert.That(result.Length == 2 && result[0] == 0x30 && result[1] == 0x40));
        }

        [Test]
        public void WhenSlicingFourBytes_ReturnValueShouldContainRest()
        {
            // Arrange
            var sevenItems = new byte[] { 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70 };

            // Act
            var result = sevenItems.Slice(2);

            // Assert
            Assert.That(result[0] == 0x30);
            Assert.That(result.Length == 5);
        }

        [Test]
        public void WhenCreatingHighPriorityMessage_ShouldBeValid()
        {
            // Arrange
            var xdk = new Xdk();
            xdk.AccelerometerSensor.XAxis.Value = 10;
            xdk.AccelerometerSensor.YAxis.Value = 20;
            xdk.AccelerometerSensor.ZAxis.Value = 30;

            // Act
            byte[] message = xdk.ToHighPriorityMessage();

            // Assert
            var testXdk = new Xdk();
            message.SetHighPriorityValues(testXdk);
            Assert.That(testXdk.AccelerometerSensor.XAxis.Value, Is.EqualTo(xdk.AccelerometerSensor.XAxis.Value));
            Assert.That(testXdk.AccelerometerSensor.YAxis.Value, Is.EqualTo(xdk.AccelerometerSensor.YAxis.Value));
            Assert.That(testXdk.AccelerometerSensor.ZAxis.Value, Is.EqualTo(xdk.AccelerometerSensor.ZAxis.Value));
        }

        [Test]
        public void WhenCreatingLowPriority1Message_ShouldBeValid()
        {
            // Arrange
            var xdk = new Xdk();
            xdk.LightSensor.MilliLux.Value = 10;
            xdk.NoiseSensor.DbSpl.Value = 20;

            // Act
            byte[] message = xdk.ToLowPriorityMessage1();

            // Assert
            var testXdk = new Xdk();
            message.SetLowPriorityValues(testXdk);
            Assert.That(testXdk.LightSensor.MilliLux.Value, Is.EqualTo(xdk.LightSensor.MilliLux.Value));
            Assert.That(testXdk.NoiseSensor.DbSpl.Value, Is.EqualTo(xdk.NoiseSensor.DbSpl.Value));
        }

        [Test]
        public void WhenCreatingLowPriority2Message_ShouldBeValid()
        {
            // Arrange
            var xdk = new Xdk();
            xdk.MagnetometerSensor.Resistance.Value = 10;
            xdk.MagnetometerSensor.XAxis.Value = 20;

            // Act
            byte[] message = xdk.ToLowPriorityMessage2();

            // Assert
            var testXdk = new Xdk();
            message.SetLowPriorityValues(testXdk);
            Assert.That(testXdk.MagnetometerSensor.Resistance.Value, Is.EqualTo(xdk.MagnetometerSensor.Resistance.Value));
            Assert.That(testXdk.MagnetometerSensor.XAxis.Value, Is.EqualTo(xdk.MagnetometerSensor.XAxis.Value));
        }
    }
}
