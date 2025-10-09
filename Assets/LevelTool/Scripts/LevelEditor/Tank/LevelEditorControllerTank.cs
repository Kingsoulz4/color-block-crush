using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ColorBlockCrush.Tools
{
    public partial class LevelEditorController : MonoBehaviour
    {
        public Dictionary<int, List<ItemTankLineElementView>> tankLineDict = new Dictionary<int, List<ItemTankLineElementView>>();

        public void AddElementToLine(int index, TankLineElementConfig elementConfig = null)
        {
            if(index < 0 && index >= view.tankLineViews.Count) return;
            Debug.Log("Index: " + index);
            ItemTankLineElementView newElement = Instantiate(model.tankLineElementPrefab).GetComponent<ItemTankLineElementView>();
            newElement.GetComponent<DraggableTankLineElementItem>().dragCanvas = canvas;
            view.tankLineViews[index].AddElementToLine(newElement, elementConfig);
            UpdateTankLinesInfor();
        }
        
        private void RemoteBusFromLine(int index, ItemTankLineElementView itemBus)
        {
            UpdateTankLinesInfor();
        }

        private void ChangeIndexBusFromLine(int busDictIndex, int itemIndex, ItemTankLineElementView itemBus)
        {
            List<ItemTankLineElementView> lstItemView = tankLineDict[busDictIndex];
            lstItemView.Remove(itemBus);
            lstItemView.Insert(itemIndex, itemBus);
            Debug.Log($"Change Bus Index {busDictIndex} {itemIndex} {itemBus}");
        }

        public void UpdateTankLinesInfor()
        {
            
        }
    }
}
