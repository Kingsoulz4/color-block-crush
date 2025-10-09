using System;
using System.Collections;
using System.Collections.Generic;
using ColorBlockCrush.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ColorBlockCrush.Tools
{
    public class ButtonColorChoose : MonoBehaviour
    {
        [SerializeField]
        private ColorType colorType;

        [SerializeField] 
        private GameObject vTick;

        private Action<ColorType> onClick;

        public void Init(Action<ColorType> onClick)
        {
            this.onClick = onClick;
        }
        
        public void OnClick()
        {
            onClick.Invoke(colorType);
        }

        public void UpdateChoosing(bool choosing)
        {
            vTick.SetActive(choosing);
        }

        public ColorType GetColorType() => colorType;
    }
}
