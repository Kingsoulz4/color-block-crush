using System;
using Analytics;
using UnityEngine;
using UnityEngine.UI;
using Yoolax.Framework;

namespace ColorBlockCrush
{
    public class PopupLose : PopupUI
    {
        [SerializeField] private Button m_buttonRetry;
        [SerializeField] private Button m_buttonClose;
        [SerializeField] private Text m_textLevel;
        
        [SerializeField] private AudioClip failSfx;

        public Action OnRetry { get; set; }

        public Action OnClose { get; set; }

        private void OnEnable()
        {
            m_textLevel.text = $"Level {LevelManager.Instance.CurrentLevel}";
        }

        private void Awake()
        {
            m_buttonRetry.onClick.AddListener(OnClickRetry);
            m_buttonClose.onClick.AddListener(OnClickClose);
        }

        public override void Show(Action onClose)
        {
            base.Show(onClose);
            if(!LevelManager.Instance.inGameplay)  
            {
                Hide();
                return;
            };
            AudioManager.Instance.PlayOneShot(failSfx, 1);
            LevelAnalyticStruct levelAnalyticStruct = new LevelAnalyticStruct();
            levelAnalyticStruct = levelAnalyticStruct.SetBaseLevel().SetLevelEndStruct(UserDataManager.PlayType,
                LevelController.Instance.GunBoardController.TotalGunCount, LevelResult.lose, 
                LoseBy.full_slot, (float)(DateTime.Now - LevelManager.Instance.timeStart).TotalSeconds);
            Server.Get<OnLevelEndEventLog>().Dispatch(levelAnalyticStruct);

            UserDataManager.LoseIndex++;
            UserDataManager.LoseStreak++;
            UserDataManager.WinStreak = 0;
        }

        private void OnClickClose()
        {
            Hide();
            OnClose?.Invoke();
        }

        private void OnClickRetry()
        {
            Hide();
            OnRetry?.Invoke();
        }
    }
}
