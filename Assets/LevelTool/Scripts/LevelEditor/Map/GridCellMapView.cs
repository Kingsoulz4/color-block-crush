using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush.Tools
{
    public class GridCellMapView : MonoBehaviour
    {
        [Header("Cell Infor")]
        public RectTransform rectTransform;
        public Image backGroundImage;
        public Color originColor;
        public GameObject x;
        public TextMeshProUGUI txtDrawIndex;
        public int Row, Col;
        public int drawIndex = -1;

        [Header("Others")]
        public bool IsSelected = false;
        public bool IsSelecting = false;

        public void UpdateSelectingColor()
        {
            Color newTxtColor = txtDrawIndex.color;
            Color newBgColor = backGroundImage.color;

            newTxtColor.a = newBgColor.a = IsSelecting ? .55f : GetNormalFade();

            if (IsSelecting)
            {
                x.SetActive(true);
            }
            else
            {
                x.SetActive(backGroundImage.color.r == originColor.r
                            && backGroundImage.color.g == originColor.g
                            && backGroundImage.color.b == originColor.b);
            }
            
            txtDrawIndex.color = newTxtColor;
            backGroundImage.color = newBgColor;
        }

        public void UpdateSelectedColor()
        {
            Color newTxtColor = txtDrawIndex.color;
            Color newBgColor = backGroundImage.color;

            newTxtColor.a = newBgColor.a = IsSelected ? .35f : GetNormalFade();

            if (IsSelected)
            {
                x.SetActive(true);
            }
            else
            {
                x.SetActive(backGroundImage.color.r == originColor.r
                && backGroundImage.color.g == originColor.g
                && backGroundImage.color.b == originColor.b);
            }

            txtDrawIndex.color = newTxtColor;
            backGroundImage.color = newBgColor;
        }

        public void UpdateColor(ColorType colorType)
        {
            x.gameObject.SetActive(false);
            Color newTxtColor = txtDrawIndex.color;
            Color newBgColor = ColorReference.Instance.GetColor(colorType);

            newBgColor.a = 1f;

            txtDrawIndex.color = newTxtColor;
            backGroundImage.color = newBgColor;        
        }

        public void DeleteColor()
        {
            Color newBgColor = originColor;
            x.gameObject.SetActive(true);
            newBgColor.a = 1f;
            
            backGroundImage.color = newBgColor;        
        }

        private float GetNormalFade()
        {
            return 1f;
        }

        public void SetRowAndCol(int col, int row)
        {
            Col = col;
            Row = row;
        }

        public void ResetSelectStatus()
        {
            IsSelecting = false;
            IsSelected = false;
            UpdateSelectingColor();
            UpdateSelectedColor();
        }
    }
}
