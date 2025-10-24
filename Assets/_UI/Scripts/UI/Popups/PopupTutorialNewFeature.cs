using I2.Loc;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ColorBlockCrush
{
    public class PopupTutorialNewFeature : PopupUI, IFlowCallback
    {
        [SerializeField] private Button m_continue;
        [SerializeField] private Text m_textFeatureTitle;
        [SerializeField] private Text m_textFeatureDes;
        [SerializeField] private Image m_imageFeature;

        public override void Initialize(UIManager uiManager)
        {
            m_continue.onClick.AddListener(() =>
            {
                GameManager.Instance.SetGameState(GameState.Playing);
                Hide();
            });
        }
        

        private void OnEnable()
        {
            if (!LevelManager.Instance.inGameplay)
            {
                Hide();
                return;
            };
        }

        public void SetData(NewFeatureItemData newFeatureData)
        {
            m_textFeatureTitle.text = LocalizationManager.GetTranslation(newFeatureData.title);
            m_textFeatureDes.text = LocalizationManager.GetTranslation(newFeatureData.desInTutorial);
            m_imageFeature.sprite = newFeatureData.icon;
        }

        public void Execute(Action callback)
        {
            Debug.Log("Check Feature");
            var feature = NewFeatureManager.Instance.NewFeatureSO.newFeatureItemDatas
                .Find(x => x.level == LevelManager.Instance.CurrentLevel);
            if (feature != null && LevelManager.Instance.CurrentLevel == feature.level)
            {
                Debug.Log("Show Feature");
                GameManager.Instance.SetGameState(GameState.Paused);
                base.Show(callback);
                SetData(feature);
            }
            else
            {
                callback?.Invoke();
                Hide();
            }
        }
    }
}
