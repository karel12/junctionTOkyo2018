using Assets.Model;
using UnityEngine;

namespace Assets.Presentation.Controllers
{
    public class WaitController : MonoBehaviour, ILayoutChild
    {

        public Xdk Xdk { get; set; }

        public bool IsEnabled
        {
            get { return transform.parent.gameObject.GetComponent<Canvas>().enabled; }
            set { transform.parent.gameObject.GetComponent<Canvas>().enabled = value; }
        }

        public bool ForceOnTop { get; set; }

        public ILayoutController LayoutController { get; set; }

        protected void Update()
        {
            if (Xdk == null)
            {
                return;
            }
            if (Xdk.BluetoothState == BluetoothLeDeviceState.Connecting || Xdk.BluetoothState == BluetoothLeDeviceState.Connected)
            {
                LayoutController.VisibleDialog = this;
            }
            else if (IsEnabled)
            {
                LayoutController.VisibleDialog = null;
            }
        }

        public Canvas GetCanvasComponent()
        {
            return transform.parent.gameObject.GetComponent<Canvas>();
        }
    }
}
