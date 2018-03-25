#if UNITY_ANDROID
using UnityEngine;

namespace Assets.Plugins
{
    /// <summary>
    /// Bluetooth bridge for Android platform
    /// </summary>
    public class BluetoothLeAndroidCommander : IBluetoothLeCommander
    {
        private static AndroidJavaObject activityContext;
        private static AndroidJavaObject android;

        /// <summary>
        /// Log the specified message.
        /// </summary>
        /// <param name="message">The message to add to the log.</param>
        public void Log(string message)
        {
            CallOnUiThread("log", message);
        }

        /// <summary>
        /// Initialize bluetooth adapter in either central or peripheral mode
        /// </summary>
        public void Initialize()
        {
            RegisterAndroidContext();
            CallOnUiThread("initialize");
        }

        /// <summary>
        /// Initialize and inject current Android Activity and Context.
        /// </summary>
        private static void RegisterAndroidContext()
        {
            using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                Debug.Log("activityClass intialized");
                activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
                if (activityContext == null)
                {
                    Debug.Log("unable to initialize activity");
                }
            }
		
            using (var pluginClass = new AndroidJavaClass("com.zuehlke.btle.BtleAdapter"))
            {
                Debug.Log("pluginClass intialized");
                android = pluginClass.CallStatic<AndroidJavaObject>("getInstance");
                android.Call("setActivity", activityContext);
            }
        }

        /// <summary>
        /// Deinitialize the bluetooth adapter
        /// </summary>
        public void DeInitialize()
        {
            StopScan();
            CallOnUiThread("deInitialize");
        }

        public void ScanForPeripheralsWithServices(string[] serviceUuiDs)
        {
            string serviceUuiDsString = serviceUuiDs != null ? string.Join("|", serviceUuiDs) : null;
            CallOnUiThread("scanForPeripheralsWithServices", serviceUuiDsString);
        }

        public void RetrieveListOfPeripheralsWithServices(string[] serviceUuiDs)
        {
            string serviceUuiDsString = serviceUuiDs != null ? string.Join("|", serviceUuiDs) : null;
            CallOnUiThread("retrieveListOfPeripheralsWithServices", serviceUuiDsString);
        }

        public void StopScan()
        {
            CallOnUiThread("stopScan");
        }

        public void ConnectToPeripheral(string deviceAddress)
        {
            CallOnUiThread("connectToPeripheral", deviceAddress);
        }

        public void DisconnectPeripheral(string deviceAddress)
        {
            CallOnUiThread("disconnectPeripheral", deviceAddress);
        }

        public void DiscoverServices(string deviceAddress)
        {
            CallOnUiThread("discoverServices", deviceAddress);
        }

        public void ReadCharacteristic(string deviceAddress, string service, string characteristic)
        {
            CallOnUiThread("readCharacteristic", deviceAddress, service, characteristic);
        }

        public void WriteCharacteristic(string deviceAddress, string service, string characteristic, byte[] data, int length)
        {
            CallOnUiThread("writeCharacteristic", deviceAddress, service, characteristic, data);
        }

        public void SubscribeCharacteristic(string deviceAddress, string service, string characteristic)
        {
            CallOnUiThread("subscribeCharacteristic", deviceAddress, service, characteristic);
        }

        public void UnSubscribeCharacteristic(string deviceAddress, string service, string characteristic)
        {
            CallOnUiThread("unSubscribeCharacteristic", deviceAddress, service, characteristic);
        }

        private static void CallOnUiThread(string method)
        {
            activityContext.Call("runOnUiThread", new AndroidJavaRunnable(() => android.Call(method)));
        }

        private static void CallOnUiThread<T>(string method, T arg1)
        {
            activityContext.Call("runOnUiThread", new AndroidJavaRunnable(() => android.Call(method, arg1)));
        }

        private static void CallOnUiThread<T1, T2>(string method, T1 arg1, T2 arg2)
        {
            activityContext.Call("runOnUiThread", new AndroidJavaRunnable(() => android.Call(method, arg1, arg2)));
        }

        private static void CallOnUiThread<T1, T2, T3>(string method, T1 arg1, T2 arg2, T3 arg3)
        {
            activityContext.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                string methodName = method;
                T1 arg1Value = arg1;
                T2 arg2Value = arg2;
                T3 arg3Value = arg3;
                //android.Call("log", methodName + ": "  + arg1Value + "|" + arg2Value + "|" +  arg3Value);
                android.Call(methodName, arg1Value, arg2Value, arg3Value);
            }));
        }

        private static void CallOnUiThread<T1, T2, T3, T4>(string method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            activityContext.Call("runOnUiThread", new AndroidJavaRunnable(() => android.Call(method, arg1, arg2, arg3, arg4)));
        }

        private static void CallOnUiThread<T1, T2, T3, T4, T5>(string method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            activityContext.Call("runOnUiThread", new AndroidJavaRunnable(() => android.Call(method, arg1, arg2, arg3, arg4, arg5)));
        }
    }
}
#endif