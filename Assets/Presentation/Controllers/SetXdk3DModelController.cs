using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Presentation.Controllers
{
    public class SetXdk3DModelController : DialogControllerBase
    {
        private bool _toggleButtonUpdated;
        private bool _updatingSpace;
        
        public Action<string> SetXdk3DModelAction { get; set; }
        
        public Func<string> GetXdk3DModelAction { get; set; }

        

        public void SetXdk3DModel(string xdk3DModelName)
        {
            SetXdk3DModelAction(xdk3DModelName);            
        }

        public void Show()
        {
            UpdateToggleButtons();
            UpdateSpace();
            UpdateOffset();
            ShowDialog();
        }

        private void UpdateSpace()
        {
            _updatingSpace = true;
            var inputField = GameObject.Find("RotateXInputField").GetComponent<InputField>();
            inputField.text = Xdk.Orientation.SpaceConversion.eulerAngles.x.ToString(CultureInfo.InvariantCulture);

            inputField = GameObject.Find("RotateYInputField").GetComponent<InputField>();
            inputField.text = Xdk.Orientation.SpaceConversion.eulerAngles.y.ToString(CultureInfo.InvariantCulture);

            inputField = GameObject.Find("RotateZInputField").GetComponent<InputField>();
            inputField.text = Xdk.Orientation.SpaceConversion.eulerAngles.z.ToString(CultureInfo.InvariantCulture);

            _updatingSpace = false;
        }

        public void UpdateOffset()
        {
            _updatingSpace = true;

            var offset = Xdk.Orientation.Offset;

            if (offset.HasValue)
            {
                var offsetAngles = offset.Value.eulerAngles;
                var inputField = GameObject.Find("OffsetXInputField").GetComponent<InputField>();
                inputField.text = offsetAngles.x.ToString(CultureInfo.InvariantCulture);

                inputField = GameObject.Find("OffsetYInputField").GetComponent<InputField>();
                inputField.text = offsetAngles.y.ToString(CultureInfo.InvariantCulture);

                inputField = GameObject.Find("OffsetZInputField").GetComponent<InputField>();
                inputField.text = offsetAngles.z.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                var inputField = GameObject.Find("OffsetXInputField").GetComponent<InputField>();
                inputField.text = string.Empty;

                inputField = GameObject.Find("OffsetYInputField").GetComponent<InputField>();
                inputField.text = string.Empty;

                inputField = GameObject.Find("OffsetZInputField").GetComponent<InputField>();
                inputField.text = string.Empty;
            }

            _updatingSpace = false;
        }

        private void UpdateToggleButtons()
        {
            if (_toggleButtonUpdated)
            {
                return;
            }
            var modelName = GetXdk3DModelAction();
            var toggle = GetToggle(modelName == Xdk3DModelManager.DefaultModelName ? "Toggle.Model1" : "Toggle.Model2");
            toggle.isOn = true;
            _toggleButtonUpdated = true;
        }

        private Toggle GetToggle(string toggleName)
        {
            return GameObject.Find(toggleName).GetComponent<Toggle>();
        }

        public void ShowOrientation(bool show)
        {
            var parent = GameObject.Find("PositionValueLabel");
            if (!show)
            {
                var text = parent.GetComponentInChildren<Text>();
                if (text != null)
                {
                    text.text = null;
                }
            }
            Xdk.ShowOrientation = show;            
        }

        public void AllowReset(bool allow)
        {
            Xdk.Orientation.AllowReset = allow;
            Xdk.Orientation.Reset(!allow);
            UpdateOffset();
        }


        public void RotateX(string angle)
        {
            if (_updatingSpace) return;
            var inputField = GameObject.Find("RotateXInputField").GetComponent<InputField>();
            var text = inputField.text;
            int value;
            if (int.TryParse(text, out value))
            {
                Xdk.Orientation.RotateX(value);
            }
        }

        public void RotateY(string angle)
        {
            if (_updatingSpace) return;
            var inputField = GameObject.Find("RotateYInputField").GetComponent<InputField>();
            var text = inputField.text;
            int value;
            if (int.TryParse(text, out value))
            {
                Xdk.Orientation.RotateY(value);
            }
        }

        public void RotateZ(string angle)
        {
            if (_updatingSpace) return;
            var inputField = GameObject.Find("RotateZInputField").GetComponent<InputField>();
            var text = inputField.text;
            int value;
            if (int.TryParse(text, out value))
            {
                Xdk.Orientation.RotateZ(value);
            }
        }

        public void OffsetX(string angle)
        {
            if (_updatingSpace) return;
            var inputField = GameObject.Find("OffsetXInputField").GetComponent<InputField>();
            var text = inputField.text;
            int value;
            if (int.TryParse(text, out value))
            {
                Xdk.Orientation.OffsetX(value);
            }
        }

        public void OffsetY(string angle)
        {
            if (_updatingSpace) return;
            var inputField = GameObject.Find("OffsetYInputField").GetComponent<InputField>();
            var text = inputField.text;
            int value;
            if (int.TryParse(text, out value))
            {
                Xdk.Orientation.OffsetY(value);
            }
        }

        public void OffsetZ(string angle)
        {
            if (_updatingSpace) return;
            var inputField = GameObject.Find("OffsetZInputField").GetComponent<InputField>();
            var text = inputField.text;
            int value;
            if (int.TryParse(text, out value))
            {
                Xdk.Orientation.OffsetZ(value);
            }
        }






    }

}