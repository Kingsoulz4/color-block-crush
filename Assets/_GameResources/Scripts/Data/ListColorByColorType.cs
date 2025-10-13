using AYellowpaper.SerializedCollections;
using ColorBlockCrush.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    [CreateAssetMenu(fileName = "ListColorByColorType", menuName = "ScriptableObjects/ListColorByColorType", order = 1)]
    public class ListColorByColorType : ScriptableObject
    {
        public SerializedDictionary<ColorType, Color> listColor;

        [HideInInspector]
        [SerializeField] private ListMaterialByColor m_listMatColor;

        [ContextMenu("Get Color")]
        private void GetColor()
        {
            if(m_listMatColor)
            {
                foreach(var item in m_listMatColor.listMaterial)
                {
                    listColor[item.Key] = item.Value.color;
                }
            }
        }
    }
}
