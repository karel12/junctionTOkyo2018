using Assets.Model;
using UnityEngine;
using UnityEngine.UI;
using System;
using Assets.Utils;

namespace Assets.Presentation.Controllers
{
    public abstract class DialogControllerBase : MonoBehaviour, ILayoutChild
    {
        public Xdk Xdk { get; set; }
        public ILayoutController LayoutController { get; set; }

        public bool IsEnabled
        {
            get { return transform.parent.gameObject.GetComponent<Canvas>().enabled; }
            set { transform.parent.gameObject.GetComponent<Canvas>().enabled = value; }
        }

        public bool ForceOnTop { get; set; }

        public void Start()
        {
            IsEnabled = false;
        }

        protected void ShowDialog()
        {
            Debug.Log("OnShowDialog");
            if (LayoutController != null)
            {
                if (!IsEnabled)
                {
                    LayoutController.VisibleDialog = this;
                }
                else
                {
                    LayoutController.VisibleDialog = null;
                }
            }

            ViewHelpers.RefreshTextToTriggerRendering(this);
        }



        public Canvas GetCanvasComponent() { return transform.parent.gameObject.GetComponent<Canvas>(); }

        protected T GetComponent<T>(string gameObjectName) where T : Component
        {
            var textGameObject = GameObject.Find(gameObjectName);
            if (textGameObject == null)
            {
                Debug.Log("Could not find " + gameObjectName);
                return null;
            }
            return textGameObject.GetComponent<T>();
        }

        protected void SetUiComponentText(string newText, string gameObjectName)
        {
            var component = GetComponent<Text>(gameObjectName);
            if (component != null)
            {
                component.text = newText;
            }
        }

        public virtual void Close()
        {
            ForceOnTop = false;
            LayoutController.VisibleDialog = null;
        }

    }
}