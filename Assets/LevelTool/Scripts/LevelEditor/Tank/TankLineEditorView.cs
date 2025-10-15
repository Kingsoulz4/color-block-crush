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

        public void SetId(int newId)
        {
            id = newId;
        }
        
        public void AddElementToLine(ItemTankLineElementView newElement, Action<ItemTankLineElementView> onSelectElement,
            Action<int> onDelete, Action<int> onChangeIndex, GunLineElementConfig elementConfig = null)
        {
            newElement.transform.SetParent(elementParent);
            newElement.transform.localScale = Vector3.one;
            int elementId = (int)CantorPairing.MakeId((ulong)id, (ulong)elementViews.Count);
           
            Action onDeleteElement = () =>
            {
                RemoteBusFromLine(newElement);
                onDelete?.Invoke(id);
            };

            Action<int> onChangeElement = (itemIndex) =>
            {
                ChangeIndexBusFromLine(itemIndex, newElement);
                onChangeIndex?.Invoke(id);
            };

            newElement.Init(elementId, onSelectElement,
                elementConfig, onDeleteElement, 
                () =>
            {
               
            }, onChangeElement);
            elementViews.Add(newElement);
            newElement.rectTransform.SetParent(elementParent, false);
        }

        public void ClearAllElementSelected()
        {
            foreach (var elementView in elementViews)
            {
                elementView.selectedObject.SetActive(false);
            }
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

        public List<ItemTankLineElementView> GetElementView() => elementViews;
    }
}
