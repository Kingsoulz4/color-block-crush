using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ColorBlockCrush.Tools;

namespace ColorBlockCrush
{
    public class PopupWarningDifficultLevel : PopupUI, IFlowCallback
    {
        [SerializeField] private GameObject m_hardLevelObject;
        [SerializeField] private GameObject m_superHardLevelObject;
        [SerializeField] private SkeletonGraphic m_hardAnim;
        [SerializeField] private SkeletonGraphic m_superHardAnim;
        [SerializeField] private Image m_frameImage;

        private LevelController LevelGame => LevelManager.Instance.LevelGame;

        public void Execute(Action callback)
        {
            if(LevelGame.GameLevelData.levelDifficult != LevelDifficult.Hard && LevelGame.GameLevelData.levelDifficult != LevelDifficult.SuperHard)
            {
                Hide();
                callback?.Invoke();
                return;
            }    
            else
            {
                Show(LevelGame.GameLevelData.levelDifficult, callback);
            }    
        }

        public void Show(LevelDifficult levelType, Action callback)
        {
            base.Show(null);
            StartCoroutine(IEAnimate(levelType, callback));
        }

        private IEnumerator IEAnimate(LevelDifficult levelType, Action callback)
        {
            m_hardLevelObject.SetActive(false);
            m_superHardLevelObject.SetActive(false);
            var objectShow = levelType == LevelDifficult.Hard ? m_hardLevelObject : m_superHardLevelObject;
            objectShow.gameObject.SetActive(true);
            objectShow.transform.localScale = Vector3.zero;
            objectShow.transform.DOScale(1, 0.5f);
            m_frameImage.DOFade(0.5f, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);

            m_hardAnim.AnimationState.SetAnimation(0, "animation hard", false);
            m_superHardAnim.AnimationState.SetAnimation(0, "animation", false);

            yield return new WaitForSeconds(3f);
            Hide();
            callback?.Invoke();
        }
    }
}
