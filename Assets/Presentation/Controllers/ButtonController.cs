using Assets.Model;
using UnityEngine;

namespace Assets.Presentation.Controllers
{
    public class ButtonController : UpdateControllerBase
    {
        private readonly MeshRenderer _meshRenderer;

        private readonly Color _normalColor;

        private readonly Color _pressedColor = new Color(204 / 255f, 204 / 255f, 204 / 255f);
        
        private readonly int _index;
        
        private bool _isPressed;

        public ButtonController(Xdk xdk, string gameObjectName, int index) : base(xdk, gameObjectName)
        {
            _index = index;
            if (TargetGameObject != null)
            {
                _meshRenderer = TargetGameObject.GetComponent<MeshRenderer>();
                _normalColor = _meshRenderer.material.color;
            }
        }

        private bool IsPressed
        {
            get { return _isPressed; }
            set
            {
                if (value != _isPressed)
                {
                    _isPressed = value;
                    UpdatePressedColor();
                }

            }
        }

        private void UpdatePressedColor()
        {
            var color = IsPressed ? _pressedColor : _normalColor;
            _meshRenderer.material.color = color;
        }

        public override void UpdateStatus()
        {
            if (TargetGameObject == null)
            {
                return; // some xdk models might do not have any buttons
            }

            IsPressed = (_index == 1 && Xdk.IsButton1Pressed) || (_index == 2 && Xdk.IsButton2Pressed);
        }
    }
}