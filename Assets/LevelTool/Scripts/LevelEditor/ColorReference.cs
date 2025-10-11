using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [CreateAssetMenu(fileName = "ColorReference", menuName = "Custom/ColorReference")]
    
    public class ColorReference : ScriptableObject
    {
        public List<ColorConfig> ColorConfigs;
        
        public static ColorReference Instance => Resources.Load<ColorReference>("ColorReference");

        public Color GetColor(ColorType colorType)
        {
            return ColorConfigs.FirstOrDefault(colorConfig => colorConfig.colorType == colorType).color;
        }
    }

    [Serializable]
    public class ColorConfig
    {
        public ColorType colorType;
        public Color color;
        public Color detectColor;
        
        public string DetectColorString => ColorUtility.ToHtmlStringRGB(color);
    }
}
