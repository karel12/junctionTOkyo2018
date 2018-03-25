using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Presentation.Controllers.Gauges
{
    /// <summary>
    /// This controller visualizes values using a set of images.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MultiValueGaugeController<T> : GaugeController
    {
        private readonly Image _gaugeImage;
        private int _currentValue;

        protected abstract Func<Xdk, SensorValue<T>> SensorSelector { get; }

        protected MultiValueGaugeController(Xdk xdk, IEnumerable<Sprite> sprites, string spriteNamePattern, string gaugeObjectName) :
            base(xdk, sprites.Where(s => s.name.StartsWith(spriteNamePattern)).ToArray(), gaugeObjectName)
        {
            _gaugeImage = TargetGameObject.GetComponent<Image>();
        }

        public override void UpdateStatus()
        {
            var sensorValue = SensorSelector(Xdk);

            
            
            var scaledValue = Mathf.FloorToInt(ScaleValue(sensorValue));
            if (scaledValue == _currentValue) return;

            if (scaledValue >= 0 && scaledValue < Sprites.Length)
            {
                _gaugeImage.sprite = Sprites[scaledValue];
                _currentValue = scaledValue;
            }
            else
            {
                Debug.Log("Invalid sprite index: " + scaledValue + GetType());
            }
        }

        protected abstract float ScaleValue(SensorValue<T> sensorValue);
    }
}
