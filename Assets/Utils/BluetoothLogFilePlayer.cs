using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Utils
{
    /// <summary>
    /// Plays messages previously recorded from the platform specific plugins.
    /// </summary>
    public class BluetoothLogFilePlayer : ILogPlayer
    {
        private long _startTime = DateTime.Now.Ticks;
        private Queue<KeyValuePair<long, string>> _playQueue;

        public BluetoothLogFilePlayer()
        {
            Debug.Log("Logger using path: " + Constants.LogFilePath);
            InitLogPlayer();
        }

        private void InitLogPlayer()
        {
            try
            {
                _playQueue = new Queue<KeyValuePair<long, string>>(ReadLog());
                _startTime = DateTime.Now.Ticks;
            }
            catch (Exception ex)
            {
                Debug.Log("Read ex: " + ex);
            }
        }

        private IEnumerable<KeyValuePair<long, string>> ReadLog()
        {
            using (var reader = new StreamReader(Constants.LogFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    var timestamp = line.Substring(1, line.IndexOf(']') - 1);
                    var data = line.Substring(line.IndexOf(']') + 1);
                    yield return new KeyValuePair<long, string>(long.Parse(timestamp), data);
                }
            }
        }

        /// <summary>
        /// Returns any message that has been recorded for the current elapsed time.
        /// </summary>
        public string TryPlayNextMessage()
        {
            if (!_playQueue.Any())
                return null;

            var candidate = _playQueue.Peek();
            if (candidate.Key < DateTime.Now.Ticks - _startTime)
            {
                return _playQueue.Dequeue().Value;
            }

            return null;
        }
    }
}
