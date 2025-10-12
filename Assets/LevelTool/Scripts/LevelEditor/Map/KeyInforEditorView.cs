using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    public class KeyInforEditorView : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private int keyId;

        public void UpdateInfor(int id, RectTransform selectionParent, IList<RectTransform> cells, 
            Canvas rootCanvas)
        {
            keyId = id;
            UiBoundsFitter.FitFrameToTargets(rectTransform, selectionParent,
                cells, Vector2.zero, rootCanvas);
        }
        
        public int GetKeyId() => keyId;
    }
}
