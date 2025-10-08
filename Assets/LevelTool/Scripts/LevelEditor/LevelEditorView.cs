using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    public class LevelEditorView : MonoBehaviour
    {
        [Header("Select MapSize")] 
        public TMP_InputField WidthMapSize;
        public TMP_InputField HeightMapSize;
        
        [Header("Choose Image Area")]
        public Button ButtonChooseImage;
        public TextMeshProUGUI TruePictureImageTxt;
    }
}
