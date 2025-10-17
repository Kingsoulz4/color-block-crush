using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class PopupTutorialNewBooster : PopupUI, IFlowCallback
    {
        [SerializeField] private Button m_buttonGotIt;
        [SerializeField] private Text m_textFeatureName;
        [SerializeField] private Text m_textFeatureDes;
        [SerializeField] private Image m_imageFeatureIcon;

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
            Hide();
        }

        public void Execute(Action callback)
        {
            var boosterUnlock = BoosterManager.Instance.BoosterData.boosterItemDatas.Find(x => x.levelUnlock == LevelManager.Instance.CurrentLevel);
            if (boosterUnlock != null)
            {
                base.Show(callback);
                SetData(boosterUnlock.title, boosterUnlock.description, boosterUnlock.icon);
            }
            else
            {
                callback?.Invoke();
            }    
        }
    }
}
