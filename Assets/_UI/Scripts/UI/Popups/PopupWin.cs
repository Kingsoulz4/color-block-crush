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
            Hide();
            OnClaimedReward.Invoke(coinReceiveValue * 2);
        }

        private void OnClickClaim()
        {
            UserDataManager.AddGold(coinReceiveValue, "Win");
            Hide();
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
        }
    }
}
