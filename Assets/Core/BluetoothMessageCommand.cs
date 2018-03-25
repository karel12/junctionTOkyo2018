using Assets.Model;

namespace Assets.Core
{
    public class BluetoothMessageCommand
    {
        public BluetoothLeCharacteristic Characteristic { get; private set; }
        public byte[] Data { get; set; }
        public int Length { get; set; }
        public bool IsSent { get; set; }
        public bool IsWriteCommand { get; set; }

        public BluetoothMessageCommand(BluetoothLeCharacteristic characteristic, byte[] data, int length)
        {
            Characteristic = characteristic;
            Data = data;
            Length = length;
            IsWriteCommand = true;
            IsSent = false;
        }

        public BluetoothMessageCommand(BluetoothLeCharacteristic characteristic)
        {
            Characteristic = characteristic;
            IsWriteCommand = false;
            IsSent = false;
        }
    }
}