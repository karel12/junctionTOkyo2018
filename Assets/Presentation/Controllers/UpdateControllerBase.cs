using Assets.Model;
using UnityEngine;

namespace Assets.Presentation.Controllers
{
    /// <summary>
    /// Base class for all controllers, which update UI once per frame
    /// </summary>
    public abstract class UpdateControllerBase : IUpdateController
    {
        private readonly Xdk _xdk;

        private readonly GameObject _targetGameObject;

        protected UpdateControllerBase(Xdk xdk, string gameObjectName)
        {
            this._xdk = xdk;
            _targetGameObject = GameObject.Find(gameObjectName);
        }

        protected Xdk Xdk
        {
            get { return _xdk; }
        }

        protected GameObject TargetGameObject
        {
            get { return _targetGameObject; }
        }

        /// <summary>
        /// Updates the visual state of the GameObject (like gauge) under control.
        /// </summary>
        public abstract void UpdateStatus();

    }
}
