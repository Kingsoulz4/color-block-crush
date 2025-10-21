using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class PopupTutorialNewFeature : PopupUI
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
        
        public void SetData(NewFeatureItemData newFeatureData)
        {
            m_textFeatureTitle.text = LocalizationManager.GetTranslation(newFeatureData.title);
            m_textFeatureDes.text = LocalizationManager.GetTranslation(newFeatureData.desInTutorial);
            m_imageFeature.sprite = newFeatureData.icon;
        }
    }
}
