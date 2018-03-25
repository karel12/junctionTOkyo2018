using System.Linq;
using Assets.Utils;
using UnityEngine;

namespace Assets.Core
{
    /// <summary>
    /// The message router receives all messages from the platform specific plugins and routes them to the <see cref="BluetoothLeMessageHandler"/>
    /// </summary>
    public class BluetoothLeMessageRouter : MonoBehaviour
    {
        private BluetoothLeMessageHandler _handler;
        private ILogPlayer _logPlayer;
        private BluetoothLogRecorder _logRecorder;

        /// <summary>
        /// Gets or sets the handler that processes the incoming messages and events.
        /// </summary>
        public BluetoothLeMessageHandler Handler
        {
            get
            {
                return _handler;
            }
            set
            {
                _handler = value;
            }
        }

        /// <summary>
        /// This is called when the Unity application starts. It is used for initialization of the current state.
        /// </summary>
        public void Start()
        {
			bool isRunningInEditor = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor;
            _logPlayer = !isRunningInEditor ? (ILogPlayer) new UnusedLogFilePlayer() : new MockLogFilePlayer();
            _logRecorder = new BluetoothLogRecorder{IsEnabled = false};
        }

        /// <summary>
        /// This is called once each frame and updates the current state.
        /// </summary>
        public void Update()
        {
            string message;

            //read all pending messages
            while ((message = _logPlayer.TryPlayNextMessage()) != null)
            {
                //Debug.Log(message);
                OnBluetoothMessage(message);
            }
        }

        /// <summary>
        /// Processes the messages received from the platform specific Bluetooth plugins.
        /// </summary>
        /// <param name="message">The message.</param>
        public void OnBluetoothMessage(string message)
        {
            if (message == null)
            {
                return;
            }

            _logRecorder.RecordMessage(message);

            string[] parts = message.Split(new[] { Constants.MessageSeparator });

            if (!parts.Any())
            {
                return;
            }

            string command = parts[0];
            string[] arguments = parts.Skip(1).ToArray();

			//Debug.Log (command);

            switch (command)
            {
                case Constants.Log:
                    _handler.OnLog(arguments[0]);
                    break;
                case Constants.CentralInitialized:
                    _handler.OnPluginInitialized();
                    break;
                case Constants.CentralDeInitialized:
                    _handler.OnPluginDeInitialized();
                    break;
                case Constants.CentralError:
                    _handler.OnPluginError(arguments.Length > 0 ? arguments[0] : string.Empty);
                    break;
                case Constants.CentralDiscoveredPeripheral:
                    _handler.OnCentralDiscoveredDevice(arguments[0], arguments[1]);
                    break;
                case Constants.CentralConnectedPeripheral:
                    _handler.OnCentralConnectedPeripheral(arguments[0]);
                    break;
                case Constants.CentralDisconnectedPeripheral:
                    _handler.OnCentralDisconnectedDevice(arguments[0]);
                    break;
                case Constants.CentralDiscoveredService:
                    _handler.OnCentralDiscoveredService(arguments[0]);
                    break;
                case Constants.CentralDiscoveredCharacteristic:
                    _handler.OnCentralDiscoveredCharacteristic(arguments[0]);
                    break;
                case Constants.CentralDidReadCharacteristic:
                    _handler.OnCentralDidReadCharacteristic(arguments[0], arguments[1]);
                    break;
                case Constants.CentralDidUpdateValueForCharacteristic:
                    _handler.OnCentralDidReadCharacteristic(arguments[0], arguments[1]);
                    break;
                case Constants.CentralDidWriteCharacteristic:
                    _handler.OnCentralDidWriteCharacteristic(arguments[0]);
                    break;
            }
        }
    }
}