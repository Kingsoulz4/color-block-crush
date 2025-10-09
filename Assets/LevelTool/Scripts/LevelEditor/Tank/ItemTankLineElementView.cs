using UnityEngine.UI;
using UnityEngine;
using System;

namespace ColorBlockCrush.Tools
{
    public class ItemTankLineElementView : MonoBehaviour
    {
        public RectTransform rectTransform;
        public Button buttonDelete;
        
        public TankLineElementConfig elementConfig = new TankLineElementConfig();
        
        private Action onUpdateSeatQuantity;
        private Action<int> onChangeIndex;
        
        public void Init(TankLineElementConfig elementConfig = null, Action onDetete = null, Action onUpdateSeatQuantity = null, Action<int> onChangeIndex = null)
        {
            if (elementConfig != null) this.elementConfig = elementConfig;
            this.onUpdateSeatQuantity = onUpdateSeatQuantity;
            buttonDelete.onClick.RemoveAllListeners();
            buttonDelete.onClick.AddListener(() => {
                onDetete?.Invoke();
            });
            UpdateUI();
            this.onChangeIndex = onChangeIndex;
        }
        
        public void ChangeIndex(int newIndex)
        {
            onChangeIndex?.Invoke(newIndex);
        }

        public void UpdateUI()
        {
            
        }
    }
}
