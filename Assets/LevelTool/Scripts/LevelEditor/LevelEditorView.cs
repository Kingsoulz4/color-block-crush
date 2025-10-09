using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    public class LevelEditorView : MonoBehaviour
    {
        [Header("Map")] 
        public Button buttonCreateMap;
        public TMP_InputField WidthMapSize;
        public TMP_InputField HeightMapSize;
        public DragCellMapSelection dragCellMapSelection;

        public Button buttonSetColor;
        public Button buttonClearAll;
        public Button buttonDel;

        [Header("Properties")] 
        public ToggleGroupListener propertyGroup;
        
        [Header("Choose Image Area")]
        public Button ButtonChooseImage;
        public TextMeshProUGUI TruePictureImageTxt;
        
        [Header("Tank")]
        public List<TankLineEditorView> tankLineViews;
    }
}
