using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class PopupLoadingProcess : PopupUI
    {
        [SerializeField] Button cancelButton;
        [SerializeField] float timedelayShowButtonCancel;

        private void Awake()
        {
            cancelButton.onClick.AddListener(Hide);
        }

        public void ShowPopup()
        {
            cancelButton.gameObject.SetActive(false);
            StartCoroutine(DelayShowButtonCancel());
        }

        IEnumerator DelayShowButtonCancel()
        {
            yield return new WaitForSeconds(timedelayShowButtonCancel);
            cancelButton.gameObject.SetActive(true);
        }
    }
}
