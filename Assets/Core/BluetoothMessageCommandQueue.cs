using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Core
{
    /// <summary>
    /// Command queue to handle asynchronous command chaining.
    /// </summary>
    public class BluetoothMessageCommandQueue
    {
        private List<BluetoothMessageCommand> commandQueue = new List<BluetoothMessageCommand>();

		/// <summary>
		/// Enqueue the specified command and remove all commands for same characteristic.
		/// </summary>
		/// <param name="command">Command.</param>
        public void Enqueue(BluetoothMessageCommand command)
        {
			commandQueue.RemoveAll (c => c.Characteristic.UUID == command.Characteristic.UUID);
            commandQueue.Insert(0, command);
        }

		/// <summary>
		/// Tries to dequeue unsent commands.
		/// </summary>
		/// <returns>The dequeue unsent.</returns>
        public BluetoothMessageCommand TryDequeueUnsent()
        {
            var lastUnsentCommand = commandQueue.LastOrDefault(c => !c.IsSent);
            if (lastUnsentCommand != null)
            {
                commandQueue.Remove(lastUnsentCommand);
                return lastUnsentCommand;
            }
            return null;
        }

		/// <summary>
		/// Tries the remove last command with the given characteristic from the queue (e.g. when the command has been sent).
		/// </summary>
		/// <param name="characteristicUuid">Characteristic UUID.</param>
        public void TryRemoveLastCommandWithCharacteristic(Guid characteristicUuid)
        {
            if (!commandQueue.Any())
            {
                return;
            }

            var lastCommand = commandQueue.LastOrDefault(c => c.Characteristic.UUID == characteristicUuid);
            int index = commandQueue.LastIndexOf(lastCommand);
            if (index >= 0)
            {
                commandQueue.RemoveAt(index);
            }
        }

		/// <summary>
		/// Clear the command queue.
		/// </summary>
        public void Clear()
        {
            commandQueue.Clear();
        }

		/// <summary>
		/// Gets true if the command queue has any elements.
		/// </summary>
		public bool Any()
		{
			return commandQueue.Any ();
		}
    }
}
