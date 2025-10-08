using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    public class ImageUtils : MonoBehaviour
    {
        public static ColorType[,] QuantizeTexture(Texture2D inputTexture, int[] pixelCountByColor)
        {
            int w = inputTexture.width;
            int h = inputTexture.height;
            ColorType[,] result = new ColorType[w, h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Color inputColor = inputTexture.GetPixel(x, y);
                    string colorString = ColorUtility.ToHtmlStringRGB(inputColor);
                    (ColorConfig resultColor, float minDistance) = FindClosestColor(inputColor);
                    string outputColorString = ColorUtility.ToHtmlStringRGB(resultColor.detectColor);
                    if (minDistance > 0.0001)
                    {
                        Debug.LogWarning(
                            $"FindClosestColor - x: {x} - y: {y} | INPUT COLOR: {colorString} - OUTPUT COLOR: {resultColor.colorType}-#{outputColorString} - MIN DISTANCE: {minDistance}");
                    }


                    result[x, y] = resultColor.colorType;

                    for (int i = 0; i < ColorReference.Instance.ColorConfigs.Count; i++)
                    {

                        if (result[x, y] == ColorReference.Instance.ColorConfigs[i].colorType)
                        {
                            pixelCountByColor[i]++;
                        }

                    }
                }
            }

            return result;
        }

        private static (ColorConfig, float) FindClosestColor(string inputColorString)
        {
            float minDistance = float.MaxValue;
            ColorConfig result = null;
            foreach (var c in ColorReference.Instance.ColorConfigs)
            {
                string fixedColorString = c.DetectColorString.ToLower();
                Color fixedColor = c.detectColor;

                if (inputColorString.Equals(fixedColorString))
                {
                    result = c;
                    minDistance = 0f;
                    break;
                }
                else
                {
                    Color inputColor = Color.black;
                    if (ColorUtility.TryParseHtmlString(inputColorString, out inputColor))
                    {
                        float distance = ColorDistance(inputColor, fixedColor);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            result = c;
                        }
                    }
                    else
                    {
                        Debug.LogError("Invalid hex color string! " + inputColorString);
                    }
                }
            }

            return (result, minDistance);
        }

        private static (ColorConfig, float) FindClosestColor(Color inputColor)
        {
            float minDistance = float.MaxValue;
            ColorConfig result = null;
            foreach (var c in ColorReference.Instance.ColorConfigs)
            {
                Color fixedColor = c.detectColor;
                string fixedColorString = c.DetectColorString.ToLower();

                if (inputColor.Equals(fixedColor))
                {
                    result = c;
                    minDistance = 0f;
                    break;
                }
                else
                {
                    Color defaultColor = Color.black;

                    float distance = ColorDistance(inputColor, fixedColor);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        result = c;
                    }
                }
            }

            return (result, minDistance);
        }

        private static float ColorDistance(Color a, Color b)
        {
            float dr = a.r - b.r;
            float dg = a.g - b.g;
            float db = a.b - b.b;
            return dr * dr + dg * dg + db * db;
        }

        public static Texture2D GenerateTextureFromColorArray(ColorType[,] colorArray)
        {
            int width = colorArray.GetLength(0);
            int height = colorArray.GetLength(1);

            Texture2D tex = new Texture2D(width, height, TextureFormat.ARGB4444, false);
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color drawColor = ColorReference.Instance.GetColor(colorArray[x, y]);
                    tex.SetPixel(x, y, drawColor);
                }
            }

            tex.Apply();
            return tex;
        }
        
        public static List<ColorType> Flatten2DArray(ColorType[,] array, out Vector2 size)
        {
            size.x = array.GetLength(0); // rows
            size.y = array.GetLength(1);  // columns

            List<ColorType> flatList = new List<ColorType>((int)(size.x * size.y));

            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    flatList.Add(array[i, j]);
                }
            }

            return flatList;
        }
    }
}

