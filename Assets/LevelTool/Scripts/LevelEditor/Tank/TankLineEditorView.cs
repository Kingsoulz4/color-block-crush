using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush.Tools
{
    public class TankLineEditorView : MonoBehaviour
    {
        [SerializeField] private int id;
        public Button buttonAddElement;
        [SerializeField] private Transform elementParent;
        [SerializeField] private List<ItemTankLineElementView> elementViews = new List<ItemTankLineElementView>();
        
        public void AddElementToLine(ItemTankLineElementView newElement, TankLineElementConfig elementConfig = null)
        {
            TankLineElementConfig newElementConfig = elementConfig != null ? elementConfig : new TankLineElementConfig();
            if (elementConfig == null)
            {
              

            }
           
            Action onDeleteElement = () =>
            {
                RemoteBusFromLine(newElement);
            };

            Action<int> onChangeElement = (itemIndex) =>
            {
                ChangeIndexBusFromLine(itemIndex, newElement);
            };

            newElement.Init(newElementConfig, onDeleteElement, () =>
            {
               
            }, onChangeElement);
            elementViews.Add(newElement);
            newElement.rectTransform.SetParent(elementParent, false);
        }
        
        private void RemoteBusFromLine(ItemTankLineElementView itemElement)
        {
            elementViews.Remove(itemElement);
            Destroy(itemElement.gameObject);
        }

        private void ChangeIndexBusFromLine(int itemIndex, ItemTankLineElementView itemBus)
        {
            elementViews.Remove(itemBus);
            elementViews.Insert(itemIndex, itemBus);
            Debug.Log($"Change Element Index {itemIndex} {itemBus}");
        }
    }
}
