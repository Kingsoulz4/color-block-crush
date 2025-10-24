using System;
using Analytics;
using ColorBlockCrush.Tools;
using Yoolax.Framework;
using UnityEditor;
using UnityEngine;
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
            MaxAdsManager.Instance.ShowRewardedAd(MaxKeys.rewardedID, () =>
            {
                UserDataManager.PlayType = PlayType.next;
                UserDataManager.AddGold(80, "winX2", ReasonType.reward.ToString());
                OnClaimedReward.Invoke(coinReceiveValue * 2); 
            });
        }

        private void OnClickClaim()
        {
            UserDataManager.PlayType = PlayType.next;
            MaxAdsManager.Instance.ShowInterstitialAd(MaxKeys.interstitialID);
            UserDataManager.AddGold(coinReceiveValue, "win", ReasonType.reward.ToString());
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
            m_goldBar.gameObject.SetActive(false);

            Action onUpdateLevel = () =>
            {
                m_goldBar.SetText(UserDataManager.Gold);
                m_goldBar.Sync = true;
                onClaimComplete?.Invoke();
                Hide();
            };
            var popupReceiveCoin = UIManager.Instance.ShowPopup<PopupReceiveCoin>(null);
            var popupBlock = UIManager.Instance.ShowPopup<PopupBlock>(null);
            popupReceiveCoin.PlayCoinFX(m_goldBar.transform.position, Vector3.zero, quantity, () =>
            {
                m_goldBar.gameObject.SetActive(true);
                onUpdateLevel?.Invoke();
                popupBlock.Hide();
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
            FetchLevelManager.Instance.FetchLevels(UserDataManager.Level + 1);
            AudioManager.Instance.PlayOneShot(winSfx, 1);
            UserDataManager.AddHeart(1, "win", false, reason: ReasonType.reward.ToString());
            m_buttonClaimX2.gameObject.SetActive(UserDataManager.Level >= GameManager.Instance.levelTriggerData.levelShowButtonClaimX2);
            
            LevelAnalyticStruct levelAnalyticStruct = new LevelAnalyticStruct();
            levelAnalyticStruct = levelAnalyticStruct.SetBaseLevel().SetLevelEndStruct(UserDataManager.PlayType,
                LevelController.Instance.GunBoardController.TotalGunCount, LevelResult.win, 
                LoseBy.NULL, (float)(DateTime.Now - LevelManager.Instance.timeStart).TotalSeconds);
            Server.Get<OnLevelEndEventLog>().Dispatch(levelAnalyticStruct);

            UserDataManager.PlayIndex = 0;
            UserDataManager.LoseIndex = 0;
            UserDataManager.ExitIndex = 0;
            UserDataManager.LoseStreak = 0;
            UserDataManager.WinStreak++;
        }
    }
}
