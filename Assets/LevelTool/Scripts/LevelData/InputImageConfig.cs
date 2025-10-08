using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    [Serializable]
    public class InputImageConfig
    {
        public string SavePath;
        public Vector2 ImageSize;
        public List<ColorType> ColorFlatMap;
        public int[] pixelCountByColor;    

        public InputImageConfig DeepCopy()
        {
            InputImageConfig copy = new InputImageConfig();
            copy.SavePath = this.SavePath;
            copy.ImageSize = this.ImageSize;
            copy.ColorFlatMap = new List<ColorType>(ColorFlatMap);
            copy.pixelCountByColor = (int[])pixelCountByColor.Clone();        
            return copy;
        }
    }
}
