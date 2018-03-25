using System;
using System.IO;
using UnityEngine;

namespace Assets.Utils
{
    /// <summary>
    /// Allows to record all messages to a log file.
    /// </summary>
    public class BluetoothLogRecorder
    {
        private readonly long _startTime = DateTime.Now.Ticks;

        public BluetoothLogRecorder()
        {
            Debug.Log("Logger using path: " + Constants.LogFilePath);

            using (File.CreateText(Constants.LogFilePath))
            {
            }
        }

        /// <summary>
        /// Gets or sets whether messages are recorded or not.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Records the given message.
        /// </summary>
        public void RecordMessage(string message)
        {
            if (!IsEnabled)
            {
                return;
            }

            try
            {
                using (var sw = File.AppendText(Constants.LogFilePath))
                {
                    sw.WriteLine("[{0}]{1}", DateTime.Now.Ticks - _startTime, message);
                }
            }
            catch (Exception ex)
            {
                Debug.Log("Save log message: " + ex);
            }
        }
    }
}
