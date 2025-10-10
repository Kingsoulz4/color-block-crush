using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ColorBlockCrush.Tools
{
    public class DoubleClickButton : Button
    {
        public UnityEvent onDoubleClick;

        [Range(0.05f, 0.5f)] public float singleClickDelay = 0.25f;
        bool pendingSingle;

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable()) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;

            if (eventData.clickCount >= 2)
            {
                pendingSingle = false;
                onDoubleClick?.Invoke();
                
                return;
            }

            if (eventData.clickCount == 1)
                StartCoroutine(InvokeSingleLater());
        }

        System.Collections.IEnumerator InvokeSingleLater()
        {
            pendingSingle = true;
            yield return new WaitForSeconds(singleClickDelay);
            if (pendingSingle) onClick?.Invoke();
            pendingSingle = false;
        }
    }
}
