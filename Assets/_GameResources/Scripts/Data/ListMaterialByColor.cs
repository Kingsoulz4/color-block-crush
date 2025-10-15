using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using ColorBlockCrush.Tools;

namespace ColorBlockCrush
{

    [CreateAssetMenu(fileName = "ListMaterialByColor", menuName = "ScriptableObject/ListMaterialByColor", order = 1)]
    public class ListMaterialByColor : ScriptableObject
    {
        public SerializedDictionary<ColorType, Material> listMaterial;

        public Material GetMaterial(ColorType colorType)
        {
            if (listMaterial.ContainsKey(colorType))
            {
                return listMaterial[colorType];
            }
            return null;
        }
    }
}
