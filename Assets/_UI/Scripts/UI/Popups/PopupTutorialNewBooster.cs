using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;
using Yoolax.Framework;

namespace ColorBlockCrush
{
    public class PopupTutorialNewBooster : PopupUI, IFlowCallback
    {
        [SerializeField] private Button m_buttonGotIt;
        [SerializeField] private Text m_textFeatureName;
        [SerializeField] private Text m_textFeatureDes;
        [SerializeField] private Image m_imageFeatureIcon;
        private BoosterType boosterType;
        private Action doneForceTut;
        
        private void Awake()
        {
            m_buttonGotIt.onClick.AddListener(OnClickGotIt);
        }

        private void OnEnable()
        {
            //var feature = NewFeatureManager.Instance.GetNewFeatureInProgress();
            //if (feature != null )
            //{
            //    m_textFeatureDes.text = feature.des;
            //    m_imageFeatureIcon.sprite = feature.icon;
            //    m_textFeatureName.text = feature.title;
            //}
        }

        public void SetData(string name, string des, Sprite icon)
        {
            m_textFeatureDes.text = LocalizationManager.GetTranslation(des);
            m_imageFeatureIcon.sprite = icon;
            m_textFeatureName.text = LocalizationManager.GetTranslation(name);
        }    

        private void OnClickGotIt()
        {
            Action useBooster = () =>
            {
                UserDataManager.AddBooster(boosterType, 3);
                doneForceTut?.Invoke();
            };
            Server.Get<OnForceTutBooster>().Dispatch(useBooster, boosterType, m_imageFeatureIcon.transform.position);
            Hide();
        }

        public void Execute(Action callback)
        {
            var boosterUnlock = BoosterManager.Instance.BoosterData.boosterItemDatas.Find(x => x.levelUnlock == LevelManager.Instance.CurrentLevel);
            if (CheckShowForceTut())
            {
                base.Show(null);
                doneForceTut = callback;
                GameManager.Instance.SetGameState(GameState.Paused);
                boosterType = boosterUnlock.boosterType;
                SetData(boosterUnlock.title, boosterUnlock.description, boosterUnlock.icon);
                
            }
            else
            {
                callback?.Invoke();
            }    
        }

        public bool CheckShowForceTut()
        {
            var boosterUnlock =
                BoosterManager.Instance.BoosterData.boosterItemDatas.Find(x =>
                    x.levelUnlock == LevelManager.Instance.CurrentLevel);
            if (boosterUnlock != null)
            {
                switch (boosterUnlock.boosterType)
                {
                    case BoosterType.ADD_TRAY:
                        return !UserDataManager.FirstClaimAddTrayBooster;
                    case BoosterType.HAND_MOVE:
                        return !UserDataManager.FirstClaimHandBooster;
                    case BoosterType.SHUFFLE:
                        return !UserDataManager.FirstClaimShuffleBooster;
                    case BoosterType.SUPER_SHOOT:
                        return !UserDataManager.FirstClaimSuperShoot;
                    default:
                        return false;
                }
            }

            return false;
        }
    }
}
