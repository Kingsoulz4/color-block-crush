using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;

namespace ColorBlockCrush.Tools
{
    public partial class LevelEditorController : MonoBehaviour
    {
        [SerializeField]
        private List<ItemTankLineElementView> tankLinesELementSelected = new List<ItemTankLineElementView>();
        [SerializeField]
        private List<UiLine> uiLines = new List<UiLine>();

        public ItemTankLineElementView currentTankLineElementSelected =>
            tankLinesELementSelected != null && tankLinesELementSelected.Count > 0
                ? tankLinesELementSelected[0]
                : null;

        #region Tank

        [SerializeField] private ColorType currentTankColor = ColorType.Red;

        public void AddElementToLine(int index, GunLineElementConfig elementConfig = null)
        {
            if (index < 0 && index >= view.tankLineViews.Count) return;

            Action<ItemTankLineElementView> onDelete = (itemLevelView) =>
            {
                (ulong lineId, ulong itemIndex) = CantorPairing.Unpair((ulong)itemLevelView.elementConfig.elementId);
                DelConnectLine(itemLevelView);
                ColorType colorType = itemLevelView.elementConfig.gunConfig.colorType;
                if (itemLevelView.elementConfig.elementType == GunLineElementType.Tank 
                    && GetBloclColorNumber(colorType) <= 0)
                    if (currentColorSet.Contains(colorType))
                        currentColorSet.Remove(colorType);
                UpdateTankLinesInfor();
                UpdateLevelState();
                ReUpdateElementId((int)lineId);
            };

            Action<int> onChangeIndex = (lineId) =>
            {
                ReUpdateElementId(lineId);
            };

            ItemTankLineElementView newElement =
                Instantiate(model.tankLineElementPrefab).GetComponent<ItemTankLineElementView>();
            newElement.GetComponent<DraggableTankLineElementItem>().dragCanvas = canvas;
            view.tankLineViews[index].AddElementToLine(newElement, SelectTankLineElement, 
                onDelete, onChangeIndex, elementConfig);
            
            ClearAllTankLineELementSelected();
            SelectTankLineElement(newElement);
            UpdateTankLinesInfor();
            
            UpdateLevelState();
        }

        private void UpdateTankLineData()
        {
            if(currentLevelConfig.gunLines == null || currentLevelConfig.gunLines.Count == 0)
                return;

            HashSet<GunLineElementConfig> tankHasConnect = new HashSet<GunLineElementConfig>();
            
            for (int i = 0; i < currentLevelConfig.gunLines.Count; i++)
            {
                if(currentLevelConfig.gunLines[i].gunLineElementConfigs.Count == 0)
                    continue;

                for (int j = 0; j < currentLevelConfig.gunLines[i].gunLineElementConfigs.Count; j++)
                {
                    AddElementToLine(i, currentLevelConfig.gunLines[i].gunLineElementConfigs[j]);
                    if (currentLevelConfig.gunLines[i].gunLineElementConfigs[j].elementType == GunLineElementType.Tank)
                    {
                        if (currentLevelConfig.gunLines[i].gunLineElementConfigs[j].gunConfig.gunConnect.Count > 0)
                        {
                            tankHasConnect.Add(currentLevelConfig.gunLines[i].gunLineElementConfigs[j]);
                        }
                    }
                }
            }

            Debug.Log($"Tank Has Connect Count: {tankHasConnect.Count}");
            DOVirtual.DelayedCall(1.2f, () =>
            {
                if (tankHasConnect.Count > 0)
                {
                    HashSet<Vector2Int> tankConnected = new HashSet<Vector2Int>();
                    foreach (var elementConfig in tankHasConnect)
                    {
                        ItemTankLineElementView tankConnect1 =
                            GetTankLineElementViewById(elementConfig.elementId);
                        for (int i = 0; i < elementConfig.gunConfig.gunConnect.Count; i++)
                        {
                            ItemTankLineElementView tankConnect2 =
                                GetTankLineElementViewById(elementConfig.gunConfig.gunConnect[i]);

                            Vector2Int checkConnect = new Vector2Int(elementConfig.elementId,
                                tankConnect2.elementConfig.elementId);
                            if (!tankConnected.Contains(checkConnect))
                            {
                                SpawnConnectUi(tankConnect1, tankConnect2);

                                tankConnected.Add(checkConnect);
                                tankConnected.Add(new Vector2Int(checkConnect.y, checkConnect.x));
                            }   
                        }
                    }
                }
            });
            
            ClearAllTankLineELementSelected();
        }

        private void ReUpdateElementId(int lineId)
        {
            TankLineEditorView tankLineEditorView = view.tankLineViews[lineId];
            List<ItemTankLineElementView> itemTankLineElementViews = tankLineEditorView.GetElementView();
            List<ItemTankLineElementView> tanksConnect = new List<ItemTankLineElementView>();

            for (int i = 0; i < itemTankLineElementViews.Count; i++)
            {
                int oldElementId = itemTankLineElementViews[i].elementConfig.elementId;
                int newElementId = (int)CantorPairing.MakeId((ulong)lineId, (ulong)i);
                if (itemTankLineElementViews[i].elementConfig.elementType == GunLineElementType.Tank)
                {
                    tanksConnect = itemTankLineElementViews[i].GetTanksConnect();
                    if (tanksConnect != null && tanksConnect.Count > 0)
                    {
                        foreach (var itemTankConnect in tanksConnect.ToList())
                        {
                            itemTankConnect.elementConfig.gunConfig.gunConnect.Remove(oldElementId);
                            itemTankConnect.elementConfig.gunConfig.gunConnect.Add(newElementId);
                        }
                    }   
                }
                itemTankLineElementViews[i].SetElementId(newElementId);
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

        public void UpdateTankLineElementPropertiesInfor(GunLineElementConfig elementConfig)
        {
            if (elementConfig.elementType == GunLineElementType.Tank)
            {
                GunConfig tankConfig = elementConfig.gunConfig;

                view.bulletInputField.text = tankConfig.bulletNumber.ToString();
                view.toggleTankHidden.isOn = tankConfig.isHidden;

                UpdateCurrentColorChooseTank(tankConfig.colorType);
            }
            else if(elementConfig.elementType == GunLineElementType.Tunnel)
            {
                UpdateTunnelHumanQueue();
            }
            
            view.toggleTankLock.isOn = elementConfig.elementType == GunLineElementType.Lock;
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

            foreach (var currentElementSelected in tankLinesELementSelected)
            {
                currentElementSelected.elementConfig.elementType = view.toggleTankLock.isOn ?
                    GunLineElementType.Lock : GunLineElementType.Tank;

                if (currentElementSelected.elementConfig.gunConfig == null)
                    currentElementSelected.elementConfig.gunConfig = new GunConfig();

                currentElementSelected.elementConfig.gunConfig.colorType = currentTankColor;
                currentElementSelected.elementConfig.gunConfig.bulletNumber =
                    int.Parse(view.bulletInputField.text);
                currentElementSelected.elementConfig.gunConfig.isHidden = view.toggleTankHidden.isOn;

                currentElementSelected.UpdateUI();   
            }
            
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
            if (currentTankLineElementSelected.elementConfig.elementType != GunLineElementType.Tank) return;

            if (tankLinesELementSelected.Count > 1)
            {
                for (int i = 0; i < tankLinesELementSelected.Count - 1; i++)
                {
                    if (tankLinesELementSelected[i].elementConfig.elementType != GunLineElementType.Tank
                        || tankLinesELementSelected[i + 1].elementConfig.elementType
                        != GunLineElementType.Tank)
                    {
                        Debug.LogError($"You Have To Choose All Tank");
                        return;
                    }

                    if (tankLinesELementSelected[i].elementConfig.gunConfig.gunConnect == null)
                        tankLinesELementSelected[i].elementConfig.gunConfig.gunConnect = new List<int>();
                    if (tankLinesELementSelected[i + 1].elementConfig.gunConfig.gunConnect == null)
                        tankLinesELementSelected[i + 1].elementConfig.gunConfig.gunConnect = new List<int>();
                    
                    tankLinesELementSelected[i].elementConfig.gunConfig.gunConnect
                        .Add(tankLinesELementSelected[i + 1].elementConfig.elementId);
                    
                    tankLinesELementSelected[i + 1].elementConfig.gunConfig.gunConnect
                        .Add(tankLinesELementSelected[i].elementConfig.elementId);

                    ItemTankLineElementView tankConnect1 =
                        GetTankLineElementViewById(tankLinesELementSelected[i].elementConfig.elementId);
                    ItemTankLineElementView tankConnect2 =
                        GetTankLineElementViewById(tankLinesELementSelected[i + 1].elementConfig.elementId);
                    SpawnConnectUi(tankConnect1, tankConnect2);
                }
            }

            ClearAllTankLineELementSelected();
            UpdateTankLinesInfor();
            UpdateLevelState();
        }

        private void DelConnectLine(ItemTankLineElementView tankLineElementView = null)
        {
            ItemTankLineElementView tankLineDelele = tankLineElementView != null
                ? tankLineElementView
                : currentTankLineElementSelected;
            
            if (tankLineDelele == null) return;
            if (tankLineDelele.elementConfig.elementType != GunLineElementType.Tank) return;

            int currentElementId = tankLineDelele.elementConfig.elementId;
            foreach (var gunIdConnect in tankLineDelele.elementConfig.gunConfig.gunConnect)
            {
                ItemTankLineElementView tankConnect =
                    GetTankLineElementViewById(gunIdConnect);
                tankConnect.elementConfig.gunConfig.gunConnect.Remove(currentElementId);

                UiLine currentLineViewConnect = uiLines.FirstOrDefault(line =>
                    (line.elementConnectA.elementConfig.elementId == currentElementId 
                     && line.elementConnectB.elementConfig.elementId == tankConnect.elementConfig.elementId)
                    || (line.elementConnectA.elementConfig.elementId == tankConnect.elementConfig.elementId &&
                         line.elementConnectB.elementConfig.elementId == currentElementId));

                if (currentLineViewConnect != null)
                {
                    uiLines.Remove(currentLineViewConnect);
                    tankConnect.RemoveConnectLine(currentLineViewConnect, tankLineDelele);
                    Destroy(currentLineViewConnect.gameObject);
                }
            }

            tankLineDelele.elementConfig.gunConfig.gunConnect = new List<int>();
            tankLineDelele.ClearConnectLine();

            UpdateTankLinesInfor();
            UpdateLevelState();
        }

        private void SpawnConnectUi(ItemTankLineElementView a, ItemTankLineElementView b)
        {
            UiLine uiLine = Instantiate(model.uiLinePrefab, view.uiLineParent).GetComponent<UiLine>();
            uiLine.canvas = canvas;
            uiLine.SetPoints(a.GetComponent<RectTransform>(), b.GetComponent<RectTransform>());
            uiLine.SetElementConnect(a, b);
            a.AddConnectLine(uiLine, b);
            b.AddConnectLine(uiLine, a);
            uiLines.Add(uiLine);
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
        
        private void OnAddTankQueueToTunnel(GunConfig tankConfig)
        {
            if (currentTankLineElementSelected == null
                && currentTankLineElementSelected.elementConfig.elementType != GunLineElementType.Tunnel)
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
                && currentTankLineElementSelected.elementConfig.elementType != GunLineElementType.Tunnel)
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

            currentTankLineElementSelected.elementConfig.elementType = GunLineElementType.Tunnel;

            if (currentTankLineElementSelected.elementConfig.tunnelConfig == null)
                currentTankLineElementSelected.elementConfig.tunnelConfig = new TunnelConfig();

            currentTankLineElementSelected.elementConfig.tunnelConfig.tankNumber = listIconTunnelQueue.Count;
            currentTankLineElementSelected.elementConfig.tunnelConfig.tanks = new List<GunConfig>();
            
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
        public void ClearAllTankLinesInfor()
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
            
            for (int i = uiLines.Count - 1; i >= 0; i--)
            {
                UiLine itemTankLineElementView = uiLines[i];
                uiLines.RemoveAt(i);

                Destroy(itemTankLineElementView.gameObject);
            }

            uiLines.Clear();
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
                    GunLineElementConfig elementConfig = elementView.elementConfig;
                    if (elementConfig.elementType == GunLineElementType.Tank)
                    {
                        tankNumber++;
                        if (elementConfig.gunConfig.isHidden) tankHidden++;
                        
                        colors.Add(elementConfig.gunConfig.colorType);
                    }
                    else if (elementConfig.elementType == GunLineElementType.Tunnel)
                    {
                        tankNumber += elementConfig.tunnelConfig.tankNumber;
                        tunnel += elementConfig.tunnelConfig.tankNumber;

                        foreach (var tankConfig in elementConfig.tunnelConfig.tanks)
                        {
                            colors.Add(tankConfig.colorType);
                        }
                    }
                    else if(elementConfig.elementType == GunLineElementType.Lock)
                    {
                        tankLock++;
                    }
                }
            }

            foreach (var colorType in colors)
            {
                currentColorSet.Add(colorType);
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
                    if (tankLineElementView.elementConfig.elementType == GunLineElementType.Tank)
                    {
                        if (tankLineElementView.elementConfig.gunConfig.colorType == colorType)
                            number += tankLineElementView.elementConfig.gunConfig.bulletNumber;
                    }
                    else if(tankLineElementView.elementConfig.elementType == GunLineElementType.Tunnel)
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