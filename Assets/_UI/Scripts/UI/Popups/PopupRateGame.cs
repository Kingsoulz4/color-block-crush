using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace ColorBlockCrush
{
    public class PopupRateGame : PopupUI
    {
        [SerializeField] private Button closeBtn;
        [SerializeField] private Button rateUsBtn;
        [SerializeField] private List<Image> listStarButton;
        [SerializeField] private GameObject inputField;
        
        [SerializeField] private int currentStarRate;
        
        private void Awake()
        {
            closeBtn.onClick.AddListener(() => {onClose?.Invoke(); Hide();});
            rateUsBtn.onClick.AddListener(Rate);
        }
        
        public void SetRate(int star) {
            currentStarRate = star;
            inputField.SetActive(currentStarRate != listStarButton.Count);

            for (int i = 0; i < listStarButton.Count; i++) {
                listStarButton[i].enabled = (currentStarRate - 1) >= i;
            }
        }

        private void Rate()
        {
            Application.OpenURL("market://details?id=" + Application.identifier);
            onClose?.Invoke();
            Hide();
        }
    }
}
