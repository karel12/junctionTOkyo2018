using Assets.Model;
using Assets.Utils;
using UnityEngine;

namespace Assets.Presentation.Controllers.Gauges
{
    /// <summary>
    /// Base class for all gauge controllers, indicating a sensor state.
    /// </summary>
    public abstract class GaugeController : UpdateControllerBase
    {        
        protected readonly Sprite[] Sprites;
        
        protected GaugeController(Xdk xdk, Sprite[] sprites, string gaugeName) : base(xdk, gaugeName)
        {
            this.Sprites = sprites;
        }
       
        protected float ScaleValue(float val, float min, float max)
        {
            return ScaleValue(val, min, max, Sprites.Length - 1);
        }

        protected float ScaleValue(float val, float min, float max, float scaleMax)
        {
            float value = val.Limit(min, max);
            return Mathf.FloorToInt(1.0f * (value - min) * scaleMax / (max - min));
        }
    }
}
