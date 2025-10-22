using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;

namespace ColorBlockCrush
{
    public class FirstLoadingScreen : ScreenUI
    {
        [SerializeField] private float timeLoading;
        [SerializeField] private Text loadingText;
        [SerializeField] private Slider m_sliderProgressLoading;

        public void Show(Action callback)
        {
            m_sliderProgressLoading.DOValue(1, timeLoading).OnComplete(() =>
            {
                callback?.Invoke();
            });

            StartCoroutine(ShowLoadingText());
        }

        IEnumerator ShowLoadingText()
        {
            WaitForSeconds wait = new WaitForSeconds(.6f);
            while (true)
            {
                loadingText.text = "Loading";
               yield return wait;
               loadingText.text = "Loading.";
               yield return wait;
               loadingText.text = "Loading..";
               yield return wait;
               loadingText.text = "Loading...";
               yield return wait;
            }
        }
    }
}
