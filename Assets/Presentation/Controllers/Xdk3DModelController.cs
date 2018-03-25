using System.Collections.Generic;
using Assets.Model;
using Assets.Presentation.Controllers.Gauges;
using UnityEngine;

namespace Assets.Presentation.Controllers
{
    public class Xdk3DModelController : UpdateControllerBase
    {
        private readonly List<IUpdateController> _controllers = new List<IUpdateController>();

        private readonly Sprite[] _gaugeSprites;

        private readonly Xdk _xdk;
        
        private readonly Xdk3DModel _model;
        
        private bool _isEnabled;


        public Xdk3DModelController(Xdk3DModel model, Sprite[] gaugeSprites, Xdk xdk) : base(xdk, model.Name)
        {
            _gaugeSprites = gaugeSprites;
            _xdk = xdk;
            _model = model;
            AddControllers();
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    TargetGameObject.SetActive(_isEnabled);                    
                }
            }
        }

        public void Hide()
        {
            _isEnabled = false;
            TargetGameObject.SetActive(false);    
        }

        public Xdk3DModel Model
        {
            get { return _model; }
        }

        protected void AddControllers()
        {
            _controllers.Add(new RotationGaugeController(_xdk, _model.Name));
            _controllers.Add(new LightGaugeController(_xdk, _gaugeSprites));
            _controllers.Add(new HygrometerGaugeController(_xdk, _gaugeSprites));
            _controllers.Add(new NoiseGaugeController(_xdk, _gaugeSprites));
            _controllers.Add(new SdCardGaugeController(_xdk, _gaugeSprites));
            _controllers.Add(new ThermometerGaugeController(_xdk));
            _controllers.Add(new ManometerGaugeController(_xdk));
            _controllers.Add(new MagnetometerGaugeController(_xdk));
            
            for (int i = 0; i < Xdk.LedCount; i++)
            {
                var ledController = new LedController(_xdk, "Led" + i, i);
                _controllers.Add(ledController);
            }

            var buttonController = new ButtonController(_xdk, "Button1", 1);
            _controllers.Add(buttonController);

            buttonController = new ButtonController(_xdk, "Button2", 2);
            _controllers.Add(buttonController);
        }

        public override void UpdateStatus()
        {
            if (IsEnabled)
            {
                _controllers.ForEach(controller => controller.UpdateStatus());
            }
        }
    }
}
