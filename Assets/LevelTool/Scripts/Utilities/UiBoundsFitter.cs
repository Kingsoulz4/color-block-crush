using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush.Tools
{
    public static class UiBoundsFitter
    {
        public static void FitFrameToTargets(RectTransform selectTarget, RectTransform selectionParent,
            IList<RectTransform> cellRects, Vector2 padding, Canvas rootCanvas = null)
        {
            PrepareTarget(selectTarget);
            
            if (rootCanvas != null)
                FitByRelativeBounds(selectTarget, cellRects, selectionParent, padding);
            else
            {
                FitByScreenSpace(selectTarget, cellRects, selectionParent, padding);
            }
        }

        // ----------------- Helpers -----------------

        private static void PrepareTarget(RectTransform target)
        {
            target.anchorMin = target.anchorMax = new Vector2(0.5f, 0.5f);
            target.pivot = new Vector2(0.5f, 0.5f);
            target.anchoredPosition3D = Vector3.zero;
            target.localRotation = Quaternion.identity;
            target.localScale = Vector3.one;
        }

        private static Canvas GetRootCanvas(RectTransform rt)
        {
            var c = rt.GetComponentInParent<Canvas>();
            return c ? c.rootCanvas : null;
        }

        private static bool ShareSameRootCanvas(IList<RectTransform> items, RectTransform targetParent)
        {
            var root = GetRootCanvas(targetParent);
            if (!root) return false;
            for (int i = 0; i < items.Count; i++)
            {
                var r = GetRootCanvas(items[i]);
                if (r != root) return false;
            }

            return true;
        }
        
        private static void FitByRelativeBounds(RectTransform target,
            IList<RectTransform> items,
            RectTransform targetParent,
            Vector2 padding)
        {
            Bounds total = default;
            bool init = false;

            for (int i = 0; i < items.Count; i++)
            {
                var b = RectTransformUtility.CalculateRelativeRectTransformBounds(targetParent, items[i]);
                if (!init)
                {
                    total = b;
                    init = true;
                }
                else
                {
                    total.Encapsulate(b.min);
                    total.Encapsulate(b.max);
                }
            }

            target.SetParent(targetParent, false);
            target.anchoredPosition = (Vector2)total.center;
            target.sizeDelta = (Vector2)total.size + padding * 2f;
        }

        private static void FitByScreenSpace(RectTransform target,
            IList<RectTransform> items,
            RectTransform targetParent,
            Vector2 padding)
        {
            Vector2 screenMin = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 screenMax = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

            var corners = new Vector3[4];

            for (int i = 0; i < items.Count; i++)
            {
                var cell = items[i];
                if (!cell) continue;

                cell.GetWorldCorners(corners);
                
                var srcCanvas = GetRootCanvas(cell);
                Camera srcCam = null;
                if (srcCanvas && srcCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                    srcCam = srcCanvas.worldCamera;

                for (int k = 0; k < 4; k++)
                {
                    var sp = RectTransformUtility.WorldToScreenPoint(srcCam, corners[k]);
                    screenMin = Vector2.Min(screenMin, sp);
                    screenMax = Vector2.Max(screenMax, sp);
                }
            }
            
            var dstCanvas = GetRootCanvas(targetParent);
            Camera dstCam = null;
            if (dstCanvas && dstCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                dstCam = dstCanvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetParent, screenMin, dstCam, out var localMin);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(targetParent, screenMax, dstCam, out var localMax);
            
            var center = (localMin + localMax) * 0.5f;
            var size = new Vector2(Mathf.Abs(localMax.x - localMin.x), Mathf.Abs(localMax.y - localMin.y));

            target.SetParent(targetParent, false);
            target.anchoredPosition = center;
            target.sizeDelta = size + padding * 2f;
        }
    }
}