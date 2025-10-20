using Spine.Unity;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class HoleToFade : MonoBehaviour,IPointerDownHandler,IPointerUpHandler   
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private List<SkeletonGraphic> skeletonGraphics;

    public void OnPointerDown(PointerEventData eventData)
    {
        canvasGroup.DOKill();
        if (skeletonGraphics != null) 
        {
            foreach (var item in skeletonGraphics)
            {
                FadeOutSkeleton(item, fadeDuration);
            }
        }
        canvasGroup.DOFade(0, fadeDuration).OnComplete(() =>
        {
            
            //FirebaseManager.Instance.LogHoleToFade();
        });
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (skeletonGraphics != null && skeletonGraphics.Count > 0) 
        {
            foreach (var item in skeletonGraphics)
            {
                FadeInSkeleton(item, fadeDuration);
            }           
        }
        canvasGroup.DOKill();
        canvasGroup.DOFade(1, fadeDuration);
    }

    public void FadeOutSkeleton(SkeletonGraphic skeletonGraphic, float duration)
    {        
        skeletonGraphic.DOKill();
        skeletonGraphic
           .DOColor(new Color(1f, 1f, 1f, 0f), duration)
           .OnComplete(() => { skeletonGraphic.enabled = false; });
    }

    public void FadeInSkeleton(SkeletonGraphic skeletonGraphic, float duration)
    {
        skeletonGraphic.enabled = true;
        skeletonGraphic.DOKill();
        skeletonGraphic
           .DOColor(new Color(1f, 1f, 1f, 1f), duration);
    }
}
