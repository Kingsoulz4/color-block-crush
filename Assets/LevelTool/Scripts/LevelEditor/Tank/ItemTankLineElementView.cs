using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
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
        [SerializeField] private List<UiLine> linesConnected = new List<UiLine>();
        [SerializeField] private List<ItemTankLineElementView> tankConnects = new List<ItemTankLineElementView>();
        
        [Header("Tunnel Infor")]
        [SerializeField] private GameObject tunnelInfor;
        [SerializeField] private TextMeshProUGUI tankNumberValue;
        
        [Header("Other")]
        public Button buttonDelete;
        public Button buttonSelected;
        public GameObject selectedObject;
        public TextMeshProUGUI elementIdText;
        
        public GunLineElementConfig elementConfig = new GunLineElementConfig();
        
        private Action onUpdateSeatQuantity;
        private Action<int> onChangeIndex;


        public void SetElementId(int elementId)
        {
            elementConfig.elementId = elementId;
            elementIdText.text = elementId.ToString();
        }
        
        public void Init(int elementId, Action<ItemTankLineElementView> onSelectElement, GunLineElementConfig elementConfig = null, 
            Action onDetete = null, Action onUpdateSeatQuantity = null, Action<int> onChangeIndex = null)
        {
            if (elementConfig != null) this.elementConfig = elementConfig;
            else
            {
                SetElementConfigDefault();
            }

            SetElementId(elementId);
            
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
            elementConfig = new GunLineElementConfig();
            elementConfig.elementType = GunLineElementType.Tank;
            elementConfig.gunConfig = new GunConfig();
            elementConfig.gunConfig.colorType = ColorType.PowderPink;
            elementConfig.gunConfig.bulletNumber = 10;
                
            elementConfig.gunConfig.isHidden = false;
            
            elementConfig.tunnelConfig = new TunnelConfig();
        }

        public void UpdateUI()
        {
            if (elementConfig.elementType == GunLineElementType.Tank)
            {
                tankInfor.SetActive(true);
                tunnelInfor.SetActive(false);
                lockIcon.SetActive(false);  
                
                GunConfig tankConfig = elementConfig.gunConfig;
                tankColorBg.color = ColorReference.Instance.GetColor(tankConfig.colorType);
                tankBulletTxtValue.text = tankConfig.bulletNumber.ToString();
                
                hidden.SetActive(tankConfig.isHidden);
            }
            else if(elementConfig.elementType == GunLineElementType.Tunnel)
            {
                tankInfor.SetActive(false);
                tunnelInfor.SetActive(true);
                lockIcon.SetActive(false);       
                
                TunnelConfig tunnelConfig = elementConfig.tunnelConfig;
                tankNumberValue.text = tunnelConfig.tankNumber.ToString();
            }
            else if(elementConfig.elementType == GunLineElementType.Lock)
            {
                tankInfor.SetActive(false);
                tunnelInfor.SetActive(false);
                lockIcon.SetActive(true);                
            }
            
        }

        public void AddConnectLine(UiLine uiLine, ItemTankLineElementView tankConnect)
        {
            linesConnected.Add(uiLine);
            tankConnects.Add(tankConnect);
        }

        public void UpdateUiLinePos()
        {
            if(linesConnected == null || linesConnected.Count == 0) return;
            
            foreach (var uiLine in linesConnected)
            {
                uiLine.UpdatePosFollowElementEditorView();
            }
        }

        public void RemoveConnectLine(UiLine uiLine, ItemTankLineElementView tankConnect)
        {
            linesConnected.Remove(uiLine);
            tankConnects.Remove(tankConnect);
        }

        public void ClearConnectLine()
        {
            linesConnected.Clear();
            tankConnects.Clear();
        }

        public List<ItemTankLineElementView> GetTanksConnect() => tankConnects;
    }
}
