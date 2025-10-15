using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using ColorBlockCrush.Tools;

namespace ColorBlockCrush
{

    [CreateAssetMenu(fileName = "ListMaterialsByColor", menuName = "ScriptableObject/ListMaterialsByColor", order = 1)]
    public class ListMaterialsByColor : ScriptableObject
    {
        public SerializedDictionary<ColorType, List<Material>> listMaterial;

        public Material GetMaterial(ColorType colorType, int index)
        {
            if (listMaterial.ContainsKey(colorType))
            {
                var materials = listMaterial[colorType];
                if (materials != null && materials.Count > 0)
                {
                    return materials[index];
                }
            }
            return null;
        }
    }
}
