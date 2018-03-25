namespace Assets.Utils
{
    /// <summary>
    /// This class is used if no log payer is needed, it will never return a result.
    /// Replace instantiation by BluetoothLogFilePlayer to replay a log file.
    /// </summary>
    public class UnusedLogFilePlayer : ILogPlayer
    {
        public string TryPlayNextMessage()
        {
            return null;
        }
    }
}
