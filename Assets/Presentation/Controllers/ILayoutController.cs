using UnityEngine;

namespace Assets.Presentation.Controllers
{
    /// <summary>
    /// All classes deriving from this interface will have only one visible dialog at a time.
    /// </summary>
    public interface ILayoutController
    {
        /// <summary>
        /// Gets or sets the currently visible dialog.
        /// </summary>
        ILayoutChild VisibleDialog { get; set; }
        void Show<T>() where T : class, ILayoutChild;
    }

    public interface ILayoutChild
    {
        Canvas GetCanvasComponent();
        ILayoutController LayoutController { get; set; }
        bool IsEnabled { get; set; }

        bool ForceOnTop { get; set; }
    }
}