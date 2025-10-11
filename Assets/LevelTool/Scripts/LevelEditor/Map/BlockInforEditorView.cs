using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush.Tools
{
    public class BlockInforEditorView : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TextMeshProUGUI blockHealthTextValue;
        [SerializeField] private Image blockBg;
        [SerializeField] private int blockId;

        public void UpdateInfor(int id, RectTransform selectionParent, IList<RectTransform> cells, 
            Canvas rootCanvas, int health, ColorType colorType)
        {
            blockId = id;
            UiBoundsFitter.FitFrameToTargets(rectTransform, selectionParent,
                cells, Vector2.zero, rootCanvas);
            blockHealthTextValue.text = health.ToString();
            blockBg.color = ColorReference.Instance.GetColor(colorType);
        }

        public int GetKeyId() => blockId;
    }
}
