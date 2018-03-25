using Assets.Core;
using Assets.Model;

namespace Assets.Presentation.Controllers
{
    public class ProtocolCheckController : DialogControllerBase, IUpdateController
    {
        public BluetoothLeMessageHandler DeviceHandler { get; set; }


        public void UpdateStatus()
        {
            //if (!Xdk.IsProtocolSupported && !IsEnabled && !Xdk.ProtololWasChecked)
            //{
            //    Xdk.ProtololWasChecked = true;
            //    SetWarningText();
            //    ShowDialog();
            //}
        }

        protected void SetWarningText()
        {
            var text = string.Format("XDK Protocol version {0} is not supported.\nExpected version {1}", Xdk.FirmwareVersion, Xdk.AppProtocolVersion);
            SetUiComponentText(text, "WarningText");
        }

        public override void Close()
        {
            base.Close();
            DeviceHandler.Disconnect();
            Xdk.ResetProtocolVersion();
        }
    }
}
