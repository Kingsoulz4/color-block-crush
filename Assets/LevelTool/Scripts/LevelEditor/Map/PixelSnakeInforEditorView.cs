using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace ColorBlockCrush.Tools
{
    public class PixelSnakeInforEditorView : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform headRectTransform;
        [SerializeField] private TextMeshProUGUI pixelSankeHealth;
        [SerializeField] private int pixelSnakeId;
        [SerializeField] private Image bodyBg;
        [SerializeField] private Image headBg;
        
        public void UpdateInfor(int id, RectTransform selectionParent, IList<RectTransform> cells, 
            RectTransform headCell, Canvas rootCanvas, int health, ColorType colorType)
        {
            pixelSnakeId = id;
            UiBoundsFitter.FitFrameToTargets(rectTransform, selectionParent,
                cells, Vector2.zero, rootCanvas);
            UiBoundsFitter.FitFrameToTargets(headRectTransform, rectTransform,
                new[] { headCell }, Vector2.zero, rootCanvas);
            pixelSankeHealth.text = health.ToString();
            Color color = ColorReference.Instance.GetColor(colorType);
            bodyBg.color = color;
            headBg.color = color;
        }
        
        public int GetPixcelSnakeId() =>  pixelSnakeId;
    }
}
