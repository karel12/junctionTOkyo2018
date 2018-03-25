using System;

namespace Assets.Presentation.Controllers
{
	public class ImprintController : DialogControllerBase
	{
		public ImprintController ()
		{
		}

		public void OnShowImpressum()
		{
		    if (LayoutController.VisibleDialog is DeviceListController)
		    {
		        ForceOnTop = true;
		    }

			ShowDialog();
		}

	}
}

