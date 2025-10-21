using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ColorBlockCrush.Tools;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class PopupWin : PopupUI
    {
        [SerializeField] private WinRewardDataSO winRewardDataSO;
        [SerializeField] private Button m_buttonClaim;
        [SerializeField] private Button m_buttonClaimX2;
        [SerializeField] private GameObject m_winContent;
        [SerializeField] private Text coinReceiveTxt;
        [SerializeField] private Text coinReceiveX2Txt;
        [SerializeField] private AudioClip winSfx;
        [SerializeField] private GoldDisplay m_goldBar;

        private int coinReceiveValue;
        public Action<int> OnClaimedReward { get; set;}

        private void Awake()
        {
            m_buttonClaim.onClick.AddListener(OnClickClaim);
            m_buttonClaimX2.onClick.AddListener(OnClickClaimX2);
        }

        private void OnClickClaimX2()
        {
            UserDataManager.AddGold(80, "WinX2");
            OnClaimedReward.Invoke(coinReceiveValue * 2);
        }

        private void OnClickClaim()
        {
            UserDataManager.AddGold(coinReceiveValue, "Win");
            OnClaimedReward.Invoke(coinReceiveValue);
        }

        public void UpdateInfo()
        {
            LevelDifficult levelDifficult = LevelManager.Instance.GetCurrentLevelType();
            switch (levelDifficult)
            {
                case LevelDifficult.Normal:
                    coinReceiveValue = winRewardDataSO.normalCoinReward;
                    break;
                case LevelDifficult.Hard:
                    coinReceiveValue = winRewardDataSO.hardCoinReward;
                    break;
                case LevelDifficult.SuperHard:
                    coinReceiveValue = winRewardDataSO.superHardCoinReward;
                    break;
            }
            
            coinReceiveTxt.text = $"x{coinReceiveValue.ToString()}";
            coinReceiveX2Txt.text = (coinReceiveValue * 2).ToString();
        }
        
        public void ShowClaimReward(int quantity, Action onClaimComplete)
        {
            m_goldBar.Sync = false;
            m_goldBar.SetText(UserDataManager.Gold - quantity);

            Action onUpdateLevel = () =>
            {
                m_goldBar.SetText(UserDataManager.Gold);
                m_goldBar.Sync = true;
                onClaimComplete?.Invoke();
                Hide();
            };
            var popupReceiveCoin = UIManager.Instance.ShowPopup<PopupReceiveCoin>(null);
            popupReceiveCoin.PlayCoinFX(m_goldBar.transform.position, Vector3.zero, quantity, () =>
            {
                onUpdateLevel?.Invoke();
            });
        }    

        private void OnEnable()
        {
            //onShowDone += UpdateFill;
        }

        private void OnDisable()
        {
            //onShowDone -= UpdateFill;
        }

        public override void Show(Action onClose)
        {
            m_winContent.SetActive(true);
            UpdateInfo();
            base.Show(onClose);
            AudioManager.Instance.PlayOneShot(winSfx, 1);
            m_buttonClaimX2.gameObject.SetActive(UserDataManager.Level >= 10);
        }
    }
}
