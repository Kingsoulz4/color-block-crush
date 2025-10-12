using System;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush.Tools
{
    public class ButtonChooseDragType : MonoBehaviour
    {
        public Button button;
        [SerializeField] private DragType dragType;
        [SerializeField] private GameObject activeStateObj;
        
        private Action<DragType> onClick;

        public void Init(Action<DragType> onClick)
        {
            this.onClick = onClick;
        }
        
        public void OnClick()
        {
            onClick.Invoke(dragType);
        }

        public void UpdateButtonState(DragType dragType)
        {
            activeStateObj.SetActive(this.dragType == dragType);
        }
    }
}
