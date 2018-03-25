using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Presentation.Controllers
{
    public class Xdk3DModelManager
    {
        private readonly List<Xdk3DModel> _xdk3DModels;
        
        private readonly List<Xdk3DModelController> _controllers;

        private Xdk3DModel _xdk3DModel;

        private const string ModelNameKey = "ModelName";

        public const string DefaultModelName = "xdk";

        public Xdk3DModelManager()
        {
            _controllers = new List<Xdk3DModelController>();
            _xdk3DModels = new List<Xdk3DModel>();
            CreateXdk3DModels();
        }

        private void CreateXdk3DModels()
        {            
            Xdk3DModels.Add(new Xdk3DModel { Name = DefaultModelName, DescriptionImageName = @"Sprites/schrift_200"});
            Xdk3DModels.Add(new Xdk3DModel { Name = "xdk.3DModel2" , DescriptionImageName = @"Sprites/white_label_200"});
        }

        public IEnumerable<IUpdateController> CreateControllers(Sprite[] gaugeSprites, Xdk xdk)
        {
            foreach (var model in _xdk3DModels)
            {
                var modelControler = new Xdk3DModelController(model, gaugeSprites, xdk);
                _controllers.Add(modelControler);
            }
            return _controllers.OfType<IUpdateController>();
        }

        public Xdk3DModel Xdk3DModel
        {
            get { return _xdk3DModel; }
            set
            {
                if (_xdk3DModel != value)
                {
                    EnableXdk3DModelController(false);
                    _xdk3DModel = value;
                    SaveXdk3ModelName();
                    UpdateDescription();
                    EnableXdk3DModelController(true);
                }
            }
        }

        private void UpdateDescription()
        {
            var image = GameObject.Find("Xdk3DModelDescription").GetComponent<Image>();
            var sprite = Resources.Load<Sprite>(Xdk3DModel.DescriptionImageName);
            image.sprite = sprite;

        }

        public List<Xdk3DModel> Xdk3DModels
        {
            get { return _xdk3DModels; }
        }

        public Func<string> GetXdk3DModelByName
        {
            get { return GetXdk3ModelNameFromSettings; }
        }

        public void Open()
        {
            HideAllXdk3Models();
            var modelNameFromSettings = GetXdk3ModelNameFromSettings();
            var model = Xdk3DModels.FirstOrDefault(m => m.Name == modelNameFromSettings);
            if (model == null)
            {
                model = Xdk3DModels.First();
                if (!string.IsNullOrEmpty(modelNameFromSettings))
                {
                    Debug.LogWarning(string.Format("No {0} 3D model found", modelNameFromSettings));
                }
            }
            Xdk3DModel = model;
        }

        private string GetXdk3ModelNameFromSettings()
        {
            return PlayerPrefs.GetString(ModelNameKey);
        }

        private void SaveXdk3ModelName()
        {
            if (Xdk3DModel != null)
            {
                PlayerPrefs.SetString(ModelNameKey, Xdk3DModel.Name);    
            }
        }

        public void SetXdk3DModelByName(string xdk3DModelName)
        {
            var model = Xdk3DModels.First(m => m.Name == xdk3DModelName);
            Xdk3DModel = model;
        }


        private void EnableXdk3DModelController(bool isEnabled)
        {
            if (_xdk3DModel != null)
            {
                var xdk3DModelController = _controllers.First(c => c.Model == _xdk3DModel);
                xdk3DModelController.IsEnabled = isEnabled;
            }
        }

        private void HideAllXdk3Models()
        {
            foreach (var xdk3DModelController in _controllers)
            {
                xdk3DModelController.Hide();
            }
            
        }
    }
}