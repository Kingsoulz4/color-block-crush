using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush.Tools
{
    public class BlockBulletValidateEditorView: MonoBehaviour
    {
        [SerializeField] private ColorType colorType;
        
        [SerializeField] private Image bulletColorBg;
        [SerializeField] private TextMeshProUGUI bulletNumberTxtValue;
        [SerializeField] private Image blockColorBg;
        [SerializeField] private TextMeshProUGUI blockNumberTxtValue;

        [SerializeField] private TextMeshProUGUI alertText;

        private void Awake()
        {
            bulletColorBg.color = ColorReference.Instance.GetColor(colorType);
            blockColorBg.color = ColorReference.Instance.GetColor(colorType);
        }

        public void ValidateState(int blockNumber, int bulletNumber)
        {
            bulletNumberTxtValue.text = bulletNumber.ToString();
            blockNumberTxtValue.text = blockNumber.ToString();

            if (blockNumber == bulletNumber)
            {
                alertText.gameObject.SetActive(false);
            }
            else
            {
                alertText.gameObject.SetActive(true);
                alertText.text = $"-{Mathf.Abs(blockNumber - bulletNumber)}";
            }
        }

        public ColorType GetColorType() => colorType;
    }
}
