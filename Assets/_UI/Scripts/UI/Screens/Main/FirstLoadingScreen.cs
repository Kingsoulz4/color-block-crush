using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System;

namespace ColorBlockCrush
{
    public class FirstLoadingScreen : ScreenUI
    {
        [SerializeField] private float timeLoading;
        [SerializeField] private Slider m_sliderProgressLoading;

        public void Show(Action callback)
        {
            m_sliderProgressLoading.DOValue(1, timeLoading).OnComplete(() =>
            {
                callback?.Invoke();
            });
        }
    }
}
