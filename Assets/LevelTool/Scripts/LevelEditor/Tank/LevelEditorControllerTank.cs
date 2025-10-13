using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace ColorBlockCrush.Tools
{
    public partial class LevelEditorController : MonoBehaviour
    {
        [SerializeField]
        private List<ItemTankLineElementView> tankLinesELementSelected = new List<ItemTankLineElementView>();

        public ItemTankLineElementView currentTankLineElementSelected =>
            tankLinesELementSelected != null && tankLinesELementSelected.Count > 0
                ? tankLinesELementSelected[0]
                : null;

        #region Tank

        [SerializeField] private ColorType currentTankColor = ColorType.Red;

        public void AddElementToLine(int index, TankLineElementConfig elementConfig = null)
        {
            if (index < 0 && index >= view.tankLineViews.Count) return;

            Action onDelete = () =>
            {
                UpdateTankLinesInfor();
                UpdateLevelState();
            };

            ItemTankLineElementView newElement =
                Instantiate(model.tankLineElementPrefab).GetComponent<ItemTankLineElementView>();
            newElement.GetComponent<DraggableTankLineElementItem>().dragCanvas = canvas;
            view.tankLineViews[index].AddElementToLine(newElement, SelectTankLineElement, 
                onDelete, elementConfig);
            
            ClearAllTankLineELementSelected();
            SelectTankLineElement(newElement);
            UpdateTankLinesInfor();
            
            UpdateLevelState();
        }

        private void UpdateTankLineData()
        {
            if(currentLevelConfig.tankLines == null || currentLevelConfig.tankLines.Count == 0)
                return;
            
            for (int i = 0; i < currentLevelConfig.tankLines.Count; i++)
            {
                if(currentLevelConfig.tankLines[i].tankLineElementConfigs.Count == 0)
                    return;

                for (int j = 0; j < currentLevelConfig.tankLines[i].tankLineElementConfigs.Count; j++)
                {
                    AddElementToLine(i, currentLevelConfig.tankLines[i].tankLineElementConfigs[j]);
                }
            }
        }

        private void SelectTankLineElement(ItemTankLineElementView elementSelect)
        {
            tankLinesELementSelected.Add(elementSelect);
            elementSelect.selectedObject.SetActive(true);
            if (tankLinesELementSelected.Count == 1)
                UpdateTankLineElementPropertiesInfor(elementSelect.elementConfig);
        }

        private void ClearAllTankLineELementSelected()
        {
            tankLinesELementSelected.Clear();

            foreach (var tankLineView in view.tankLineViews)
            {
                tankLineView.ClearAllElementSelected();
            }
            
            ClearTunnelView();
            
        }

        public void UpdateTankLineElementPropertiesInfor(TankLineElementConfig elementConfig)
        {
            if (elementConfig.elementType == TankLineElementType.Tank)
            {
                TankConfig tankConfig = elementConfig.tankConfig;

                view.bulletInputField.text = tankConfig.bulletNumber.ToString();
                view.toggleTankLock.isOn = tankConfig.hasLock;
                view.toggleTankHidden.isOn = tankConfig.isHidden;

                UpdateCurrentColorChooseTank(tankConfig.colorType);
            }
            else
            {
                UpdateTunnelHumanQueue();
            }
        }

        private void UpdateCurrentColorChooseTank(ColorType colorChoose)
        {
            foreach (var buttonChoose in view.buttonTankColorChooses)
            {
                buttonChoose.UpdateChoosing(buttonChoose.GetColorType() == colorChoose);
            }

            currentTankColor = colorChoose;
        }

        private void SetTankInfor()
        {
            if (currentTankLineElementSelected == null) return;

            currentTankLineElementSelected.elementConfig.elementType = TankLineElementType.Tank;

            if (currentTankLineElementSelected.elementConfig.tankConfig == null)
                currentTankLineElementSelected.elementConfig.tankConfig = new TankConfig();

            currentTankLineElementSelected.elementConfig.tankConfig.colorType = currentTankColor;
            currentTankLineElementSelected.elementConfig.tankConfig.bulletNumber =
                int.Parse(view.bulletInputField.text);
            currentTankLineElementSelected.elementConfig.tankConfig.hasLock = view.toggleTankLock.isOn;
            currentTankLineElementSelected.elementConfig.tankConfig.isHidden = view.toggleTankHidden.isOn;

            currentTankLineElementSelected.UpdateUI();
            UpdateTankLinesInfor();
            
            UpdateLevelState();
            ClearAllTankLineELementSelected();
        }

        private void DellTankInfor()
        {
            if (currentTankLineElementSelected == null) return;

            currentTankLineElementSelected.SetElementConfigDefault();
            currentTankLineElementSelected.UpdateUI();
            UpdateTankLineElementPropertiesInfor(currentTankLineElementSelected.elementConfig);
            
            UpdateTankLinesInfor();
            
            UpdateLevelState();
            ClearAllTankLineELementSelected();
        }

        private void SetConnectLine()
        {
            if (currentTankLineElementSelected == null) return;
            if (currentTankLineElementSelected.elementConfig.elementType != TankLineElementType.Tank) return;

            if (tankLinesELementSelected.Count > 1)
            {
                currentTankLineElementSelected.elementConfig.tankConfig.tankConnect = new List<int>();
                for (int i = 1; i < tankLinesELementSelected.Count; i++)
                {
                    if (tankLinesELementSelected[i].elementConfig.elementType != TankLineElementType.Tank)
                    {
                        Debug.LogError($"Must Choose All Tank");
                        return;   
                    }
                    
                    currentTankLineElementSelected.elementConfig.tankConfig.tankConnect
                        .Add(tankLinesELementSelected[i].elementConfig.elementId);

                    ItemTankLineElementView tankConnect =
                        GetTankLineElementViewById(tankLinesELementSelected[i].elementConfig.elementId);
                    SpawnConnectUi(currentTankLineElementSelected, tankConnect);
                }
            }

            ClearAllTankLineELementSelected();
            UpdateTankLinesInfor();
            UpdateLevelState();
        }

        private void DelConnectLine()
        {
            if (currentTankLineElementSelected == null) return;
            if (currentTankLineElementSelected.elementConfig.elementType != TankLineElementType.Tank) return;

            if (tankLinesELementSelected.Count > 1)
            {
                for (int i = 0; i < currentTankLineElementSelected.elementConfig.tankConfig.tankConnect.Count; i++)
                {
                    currentTankLineElementSelected.elementConfig.tankConfig.ClearConnect();
                }
            }

            UpdateTankLinesInfor();
            UpdateLevelState();
        }

        private void SpawnConnectUi(ItemTankLineElementView a, ItemTankLineElementView b)
        {
            UiLine uiLine = Instantiate(model.uiLinePrefab, view.uiLineParent).GetComponent<UiLine>();
            uiLine.canvas = canvas;
            
            uiLine.SetPoints(a.GetComponent<RectTransform>(), b.GetComponent<RectTransform>());
        }

        private ItemTankLineElementView GetTankLineElementViewById(int id)
        {
            (ulong i, ulong j) = CantorPairing.Unpair((ulong)id);

            return view.tankLineViews[(int)i].GetElementView()[(int)j];
        }

        #endregion

        #region Tunnel

        public List<ItemTunnelQueueView> listIconTunnelQueue = new List<ItemTunnelQueueView>();
        [SerializeField] private ColorType currentTankTunnelQueueColor = ColorType.Red;

        private void UpdateCurrentColorChooseTankTunnelQueue(ColorType colorChoose)
        {
            foreach (var buttonChoose in view.buttonTankTunnelQueueColorChooses)
            {
                buttonChoose.UpdateChoosing(buttonChoose.GetColorType() == colorChoose);
            }

            currentTankTunnelQueueColor = colorChoose;
        }
        
        private void OnAddTankQueueToTunnel(TankConfig tankConfig)
        {
            if (currentTankLineElementSelected == null
                && currentTankLineElementSelected.elementConfig.elementType != TankLineElementType.Tunnel)
            {
                Debug.LogError("OnSetTunnelArea - SELECT A TUNNEL FIRST");
                return;
            }

            currentTankLineElementSelected.elementConfig.tunnelConfig.tanks.Add(tankConfig);

            ItemTunnelQueueView newItemTunnelQueue =
                Instantiate(model.itemTunnelQueuePrefab).GetComponent<ItemTunnelQueueView>();
            newItemTunnelQueue.GetComponent<DraggableTunnelItem>().dragCanvas = canvas;
            Action<int, int> onChangeIndex = (oldItemIndex, newItemIndex) =>
            {
                ChangeIndexItemTunnel(oldItemIndex, newItemIndex, newItemTunnelQueue);
            };
            Action<int> onDelete = (itemIndex) => { RemoveItemFromTunnel(itemIndex, newItemTunnelQueue); };
            newItemTunnelQueue.rectTransform.SetParent(view.tunnelElementQueueParent, false);
            newItemTunnelQueue.IconColor.color = ColorReference.Instance.GetColor(tankConfig.colorType);
            newItemTunnelQueue.tankConfig = tankConfig;
            newItemTunnelQueue.bulletNumber.text = tankConfig.bulletNumber.ToString();
            newItemTunnelQueue.Init(onDelete, onChangeIndex);
            listIconTunnelQueue.Add(newItemTunnelQueue);
            view.tankTunnelQueueNumber.text = listIconTunnelQueue.Count.ToString();
            UpdateTankLinesInfor();
            UpdateLevelState();
        }

        private void ChangeIndexItemTunnel(int oldItemIndex, int newItemIndex, ItemTunnelQueueView itemTunnel)
        {
            listIconTunnelQueue.Remove(itemTunnel);
            currentTankLineElementSelected.elementConfig.tunnelConfig.tanks.RemoveAt(oldItemIndex);
            listIconTunnelQueue.Insert(newItemIndex, itemTunnel);
            currentTankLineElementSelected.elementConfig.tunnelConfig.tanks.Insert(newItemIndex, itemTunnel.tankConfig);
            Debug.Log($"Change Item Tunnel {itemTunnel} from {oldItemIndex} to {newItemIndex}");
        }

        private void RemoveItemFromTunnel(int itemIndex, ItemTunnelQueueView itemTunnel)
        {
            listIconTunnelQueue.Remove(itemTunnel);
            currentTankLineElementSelected.elementConfig.tunnelConfig.tanks.RemoveAt(itemIndex);
            Destroy(itemTunnel.gameObject);
            view.tankTunnelQueueNumber.text = listIconTunnelQueue.Count.ToString();
            UpdateTankLinesInfor();
            UpdateLevelState();
        }

        private void OnClearTunnelHumanQueue()
        {
            if (currentTankLineElementSelected == null
                && currentTankLineElementSelected.elementConfig.elementType != TankLineElementType.Tunnel)
            {
                Debug.Log("OnSetTunnelArea - SELECT A TUNNEL FIRST");
                return;
            }

            ClearTunnelView();
            currentTankLineElementSelected.elementConfig.tunnelConfig.tanks.Clear();
            view.tankTunnelQueueNumber.text = listIconTunnelQueue.Count.ToString();
            currentTankLineElementSelected.UpdateUI();
            
            currentTankLineElementSelected.SetElementConfigDefault();
            currentTankLineElementSelected.UpdateUI();
            UpdateTankLineElementPropertiesInfor(currentTankLineElementSelected.elementConfig);
            
            UpdateTankLinesInfor();
            UpdateLevelState();
        }

        private void ClearTunnelView()
        {
            for (int i = listIconTunnelQueue.Count - 1; i >= 0; --i)
            {
                Destroy(listIconTunnelQueue[i].gameObject);
            }

            listIconTunnelQueue.Clear();
            view.tankTunnelQueueNumber.text = "0";
        }

        private void UpdateTunnelHumanQueue()
        {
            for (int i = listIconTunnelQueue.Count - 1; i >= 0; --i)
            {
                Destroy(listIconTunnelQueue[i].gameObject);
            }

            listIconTunnelQueue.Clear();

            foreach (var tankConfig in currentTankLineElementSelected.elementConfig.tunnelConfig.tanks)
            {
                ItemTunnelQueueView newItemTunnelQueue = Instantiate(model.itemTunnelQueuePrefab).GetComponent<ItemTunnelQueueView>();
                newItemTunnelQueue.GetComponent<DraggableTunnelItem>().dragCanvas = canvas;
                Action<int, int> onChangeIndex = (oldItemIndex, newItemIndex) =>
                {
                    ChangeIndexItemTunnel(oldItemIndex, newItemIndex, newItemTunnelQueue);
                };
                Action<int> onDelete = (itemIndex) => { RemoveItemFromTunnel(itemIndex, newItemTunnelQueue); };
                newItemTunnelQueue.rectTransform.SetParent(view.tunnelElementQueueParent, false);
                newItemTunnelQueue.IconColor.color = ColorReference.Instance.GetColor(tankConfig.colorType);
                newItemTunnelQueue.tankConfig = tankConfig;
                newItemTunnelQueue.bulletNumber.text = tankConfig.bulletNumber.ToString();
                newItemTunnelQueue.Init(onDelete, onChangeIndex);
                listIconTunnelQueue.Add(newItemTunnelQueue);
            }

            view.tankTunnelQueueNumber.text =
                currentTankLineElementSelected.elementConfig.tunnelConfig.tankNumber.ToString();
        }

        private void SetTunnelInfor()
        {
            if (currentTankLineElementSelected == null) return;

            currentTankLineElementSelected.elementConfig.elementType = TankLineElementType.Tunnel;

            if (currentTankLineElementSelected.elementConfig.tunnelConfig == null)
                currentTankLineElementSelected.elementConfig.tunnelConfig = new TunnelConfig();

            currentTankLineElementSelected.elementConfig.tunnelConfig.tankNumber = listIconTunnelQueue.Count;
            currentTankLineElementSelected.elementConfig.tunnelConfig.tanks = new List<TankConfig>();
            
            for (int i = 0; i < listIconTunnelQueue.Count; i++)
            {
                currentTankLineElementSelected.elementConfig.tunnelConfig.tanks.Add(listIconTunnelQueue[i].tankConfig);   
            }
           
            currentTankLineElementSelected.UpdateUI();
            ClearTunnelView();
            UpdateTankLinesInfor();
            UpdateLevelState();
            
            ClearAllTankLineELementSelected();
        }

        private void DellTunnelInfor()
        {
            OnClearTunnelHumanQueue();
            ClearAllTankLineELementSelected();
        }

        #endregion

        [Button]
        public void ClearAllTankLines()
        {
            foreach (TankLineEditorView tankLineView in view.tankLineViews)
            {
                List<ItemTankLineElementView> itemTankLineElementViews = tankLineView.GetElementView();
                for (int i = itemTankLineElementViews.Count - 1; i >= 0; i--)
                {
                    ItemTankLineElementView itemTankLineElementView = itemTankLineElementViews[i];
                    itemTankLineElementViews.RemoveAt(i);

                    Destroy(itemTankLineElementView.gameObject);
                }

                itemTankLineElementViews.Clear();
            }
        }

        [Button]
        public void UpdateTankLinesInfor()
        {
            int tankNumber = 0, tankHidden = 0, tankLock = 0, tankConnect = 0, tunnel = 0;
            HashSet<ColorType> colors = new HashSet<ColorType>();

            foreach (var tankLineView in view.tankLineViews)
            {
                foreach (var elementView in tankLineView.GetElementView())
                {
                    TankLineElementConfig elementConfig = elementView.elementConfig;
                    if (elementConfig.elementType == TankLineElementType.Tank)
                    {
                        tankNumber++;
                        if (elementConfig.tankConfig.isHidden) tankHidden++;
                        if (elementConfig.tankConfig.hasLock) tankLock++;
                        
                        colors.Add(elementConfig.tankConfig.colorType);
                    }
                    else if (elementConfig.elementType == TankLineElementType.Tunnel)
                    {
                        tankNumber += elementConfig.tunnelConfig.tankNumber;
                        tunnel += elementConfig.tunnelConfig.tankNumber;

                        foreach (var tankConfig in elementConfig.tunnelConfig.tanks)
                        {
                            colors.Add(tankConfig.colorType);
                        }
                    }
                }
            }

            view.tankNumberTxtValue.text = tankNumber.ToString();
            view.tankHiddenNumberTxtValue.text = tankHidden.ToString();
            view.tankLockNumberTxtValue.text = tankLock.ToString();
            view.tankConnectionNumberTxtValue.text = tankConnect.ToString();
            view.tunneNumberTxtValue.text = tunnel.ToString();
            view.tankColorNumberTxtValue.text = colors.Count.ToString();
        }
        
        public void UpdateButtonChooseTankColor()
        {
            foreach (var buttonColorChoose in view.buttonTankColorChooses)
            {
                buttonColorChoose.gameObject.
                    SetActive(currentColorSet.Contains(buttonColorChoose.GetColorType()));
            }
        }

        public void UpdateButtonChooseTunnelQueueColor()
        {
            foreach (var buttonColorChoose in view.buttonTankTunnelQueueColorChooses)
            {
                buttonColorChoose.gameObject.
                    SetActive(currentColorSet.Contains(buttonColorChoose.GetColorType()));
            }
        }

        public int GetBulletNumber(ColorType colorType)
        {
            int number = 0;

            foreach (var tankLineEditorView in view.tankLineViews)
            {
                foreach (var tankLineElementView in tankLineEditorView.GetElementView())
                {
                    if (tankLineElementView.elementConfig.elementType == TankLineElementType.Tank)
                    {
                        if (tankLineElementView.elementConfig.tankConfig.colorType == colorType)
                            number += tankLineElementView.elementConfig.tankConfig.bulletNumber;
                    }
                    else if(tankLineElementView.elementConfig.elementType == TankLineElementType.Tunnel)
                    {
                        foreach (var tankConfig in tankLineElementView.elementConfig.tunnelConfig.tanks)
                        {
                            if (tankConfig.colorType == colorType)
                                number += tankConfig.bulletNumber;
                        }
                    }
                }
            }

            return number;
        }
    }
}