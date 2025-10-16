using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush.Tools
{
    public class UiLine : MonoBehaviour
    {
        public Canvas canvas; 
        public RectTransform parent;
        public RectTransform lineRect;
        [Header("Config")] public float thickness = 3f;
        public ItemTankLineElementView elementConnectA;
        public ItemTankLineElementView elementConnectB;

        void Reset()
        {
            canvas = GetComponentInParent<Canvas>();
            parent = (RectTransform)(transform as RectTransform)?.parent;
            
            if (!lineRect)
            {
                var go = new GameObject("UILine", typeof(RectTransform), typeof(Image));
                go.transform.SetParent(transform, false);
                lineRect = go.GetComponent<RectTransform>();
                var img = go.GetComponent<Image>();
                img.raycastTarget = false;
                img.color = Color.white;
            }
        }

        public void SetElementConnect(ItemTankLineElementView configA, ItemTankLineElementView configB)
        {
            elementConnectA = configA;
            elementConnectB = configB;
        }

        public void UpdatePosFollowElementEditorView()
        {
            SetPoints(elementConnectA.GetComponent<RectTransform>(), elementConnectB.GetComponent<RectTransform>());
        }
        
        public void SetPoints(RectTransform a, RectTransform b)
        {
            var cam = canvas ? canvas.worldCamera : null;
            parent = (RectTransform)(transform as RectTransform)?.parent;

            Vector2 pa, pb;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent,
                RectTransformUtility.WorldToScreenPoint(cam, a.position),
                cam, out pa);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent,
                RectTransformUtility.WorldToScreenPoint(cam, b.position),
                cam, out pb);

            SetPoints(pa, pb);
        }
        
        public void SetPoints(Vector2 a, Vector2 b)
        {
            lineRect.anchorMin = lineRect.anchorMax = new Vector2(0.5f, 0.5f);
            lineRect.pivot = new Vector2(0f, 0.5f);

            Vector2 d = b - a;
            float len = d.magnitude;
            float angle = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;

            lineRect.sizeDelta = new Vector2(len, thickness);
            lineRect.anchoredPosition = a;
            lineRect.localRotation = Quaternion.Euler(0, 0, angle);
        }
        
        public void SetColor(Color c)
        {
            var img = lineRect.GetComponent<Image>();
            if (img) img.color = c;
        }
    }
}