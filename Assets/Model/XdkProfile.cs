using System;
using Assets.Utils;

namespace Assets.Model
{
    /// <summary>
    /// Configure the XDK specific Bluetooth Low Energy profile here.
    /// </summary>
    public class XdkProfile : BluetoothLeProfile
    {
        private XdkProfile()
        { }

        /// <summary>
        /// Creates this instance, using the given hard coded configuration.
        /// </summary>
        /// <returns></returns>
        public static BluetoothLeProfile Create()
        {
            return new XdkProfile()

                .WithService(new BluetoothLeService(new Guid("c2967210-7ba4-11e4-82f8-0800200c9a66"), "XDK data rate service")
                    .WithCharacteristic(new BluetoothLeCharacteristic<Xdk>(new Guid("c2967211-7ba4-11e4-82f8-0800200c9a66"), "High priority array")
                        .WithSetter((xdk, value) => value.SetHighPriorityValues(xdk))
                        .IsNotified(true))
                    .WithCharacteristic(new BluetoothLeCharacteristic<Xdk>(new Guid("c2967212-7ba4-11e4-82f8-0800200c9a66"), "Low priority array")
                        .WithSetter((xdk, value) => value.SetLowPriorityValues(xdk))
                        .IsNotified(true)))

                .WithService(new BluetoothLeService(new Guid("55b741d0-7ada-11e4-82f8-0800200c9a66"), "XDK Control Service")
                    .WithCharacteristic(new BluetoothLeCharacteristic<Xdk>(new Guid("55b741d1-7ada-11e4-82f8-0800200c9a66"), "Start Sensor Sampling and Notifications")
                        .OfType(CharacteristicType.IsStartingNotifications))
                    .WithCharacteristic(new BluetoothLeCharacteristic<Xdk>(new Guid("55b741d2-7ada-11e4-82f8-0800200c9a66"), "Send Sampling Time")
                        .OfType(CharacteristicType.IsSendingSamplingTime))
                    .WithCharacteristic(new BluetoothLeCharacteristic<Xdk>(new Guid("55b741d3-7ada-11e4-82f8-0800200c9a66"), "Reboot XDK")
                        .OfType(CharacteristicType.IsResettingXdk))
                    .WithCharacteristic(new BluetoothLeCharacteristic<Xdk>(new Guid("55b741d4-7ada-11e4-82f8-0800200c9a66"), "Get Firmware Version")
                        .WithSetter((xdk, value) => xdk.FirmwareVersion = value.TryGetVersionString())
                        .OfType(CharacteristicType.IsReadingFirmwareVersion)));
        }
    }
}
