using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush.Tools
{
    public class LevelEditorModel : MonoBehaviour
    {
        [Header("Map")] 
        public int maxMapSize = 50;
        public int minMapSize = 20;
        
        public GameObject gridCellMapViewPrefab;
        public RectTransform gridContainer;
        public GridLayoutGroup gridLayoutGroup;

        [Header("Tank")] 
        public GameObject tankLineElementPrefab;
        public GameObject itemTunnelQueuePrefab;

    }
}
