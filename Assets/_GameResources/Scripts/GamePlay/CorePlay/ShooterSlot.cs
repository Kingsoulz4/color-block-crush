using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class ShooterSlot : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private float minAlpha = 0.2f;
        [SerializeField] private float maxAlpha = 1f;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Tween fadeTween;

        public void StartWarning()
        {
            fadeTween?.Kill();

            Color color = spriteRenderer.color;
            color.a = maxAlpha;
            spriteRenderer.color = color;

            fadeTween = spriteRenderer
                .DOFade(minAlpha, fadeDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void StopWarning()
        {
            fadeTween?.Kill();

            Color color = spriteRenderer.color;
            color.a = 0f;
            spriteRenderer.color = color;
        }

        void OnDestroy()
        {
            fadeTween?.Kill();
        }
    }
}
