using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace ColorBlockCrush.Tools
{
    public class ItemTankLineElementView : MonoBehaviour
    {
        public RectTransform rectTransform;

        [Header("Tank Infor")] 
        [SerializeField] private GameObject tankInfor;
        [SerializeField] private Image tankColorBg;
        [SerializeField] private TextMeshProUGUI tankBulletTxtValue;
        [SerializeField] private GameObject hidden;
        [SerializeField] private GameObject lockIcon;
        
        [Header("Tunnel Infor")]
        [SerializeField] private GameObject tunnelInfor;
        [SerializeField] private TextMeshProUGUI tankNumberValue;
        
        
        [Header("Other")]
        public Button buttonDelete;
        public Button buttonSelected;
        public GameObject selectedObject;
        
        
        public TankLineElementConfig elementConfig = new TankLineElementConfig();
        
        private Action onUpdateSeatQuantity;
        private Action<int> onChangeIndex;
        
        public void Init(int elementId, Action<ItemTankLineElementView> onSelectElement, TankLineElementConfig elementConfig = null, 
            Action onDetete = null, Action onUpdateSeatQuantity = null, Action<int> onChangeIndex = null)
        {
            if (elementConfig != null) this.elementConfig = elementConfig;
            else
            {
                SetElementConfigDefault();
            }

            this.elementConfig.elementId = elementId;
            
            this.onUpdateSeatQuantity = onUpdateSeatQuantity;
            buttonDelete.onClick.RemoveAllListeners();
            buttonDelete.onClick.AddListener(() => {
                onDetete?.Invoke();
            });
            buttonSelected.onClick.RemoveAllListeners();
            buttonSelected.onClick.AddListener(() =>
            {
                onSelectElement?.Invoke(this);
            });
            UpdateUI();
            this.onChangeIndex = onChangeIndex;
        }
        
        public void ChangeIndex(int newIndex)
        {
            onChangeIndex?.Invoke(newIndex);
        }

        public void SetElementConfigDefault()
        {
            elementConfig = new TankLineElementConfig();
            elementConfig.elementType = TankLineElementType.Tank;
            elementConfig.tankConfig = new TankConfig();
            elementConfig.tankConfig.colorType = ColorType.Pink;
            elementConfig.tankConfig.bulletNumber = 10;
                
            elementConfig.tankConfig.isHidden = false;
            elementConfig.tankConfig.hasLock = false;
            
            elementConfig.tunnelConfig = new TunnelConfig();
        }

        public void UpdateUI()
        {
            if (elementConfig.elementType == TankLineElementType.Tank)
            {
                tankInfor.SetActive(true);
                tunnelInfor.SetActive(false);
                
                TankConfig tankConfig = elementConfig.tankConfig;
                tankColorBg.color = ColorReference.Instance.GetColor(tankConfig.colorType);
                tankBulletTxtValue.text = tankConfig.bulletNumber.ToString();
                
                hidden.SetActive(tankConfig.isHidden);
                lockIcon.SetActive(tankConfig.hasLock);
            }
            else
            {
                tankInfor.SetActive(false);
                tunnelInfor.SetActive(true);
                
                TunnelConfig tunnelConfig = elementConfig.tunnelConfig;
                tankNumberValue.text = tunnelConfig.tankNumber.ToString();
            }
        }
    }
}
