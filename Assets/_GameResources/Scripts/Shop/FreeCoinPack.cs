using System;
using System.Collections;
using Analytics;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class FreeCoinPack : MonoBehaviour
    {
        [SerializeField] private Button m_buttonGet;
        [SerializeField] private Button m_buttonDisable;
        [SerializeField] private Button m_buttonOutOfTurns;
        [SerializeField] private Text m_textCoinQuantity;
        [SerializeField] private Text m_textCoolDown;

        public Action<int> OnGotCoin { get; set; }

        private int coinQuantity = 200;

        private int intervalReceiveFreeCoin = 21600;

        private int maxTimeReceiveFreeCoin = 2;

        private double timeCoolDown;

        private DateTime LastTimeReceiveFreeCoin
        {
            get
            {
                return MyUlti.TimeStamp2DateTime(long.Parse(PlayerPrefs.GetString("LastTimeReceiveFreeCoin", "0")));
            }
            set
            {
                PlayerPrefs.SetString("LastTimeReceiveFreeCoin", MyUlti.ToTimestamp(value).ToString());
            }
        }

        private int CurrentTimeReceiveFreeCoin
        {
            get => PlayerPrefs.GetInt("CurrentTimeReceiveFreeCoin", 0);
            set => PlayerPrefs.SetInt("CurrentTimeReceiveFreeCoin", value);
        }    
        
        [Button]
        public int GetCurrentLastTimeReceiveFreeCoin() => PlayerPrefs.GetInt("LastTimeReceiveFreeCoin", 0);

        [Button]
        public void SetCurrentTimeReceiveFreeCoin(int value)
        {
            PlayerPrefs.SetInt("CurrentTimeReceiveFreeCoin", value);            
        }
        
        [Button]
        public int GetCurrentTimeReceiveFreeCoin() => PlayerPrefs.GetInt("CurrentTimeReceiveFreeCoin", 0);

        private void Awake()
        {
            m_buttonGet.onClick.AddListener(OnClickGet);
            m_buttonDisable.onClick.AddListener(OnClickDisable);
            m_buttonOutOfTurns.onClick.AddListener(OnClickOutOfTurn);

            m_textCoinQuantity.text = coinQuantity.ToString();
        }

        private void OnClickOutOfTurn()
        {
            UIManager.Instance.ShowPopup<PopupMiniNoti>(null).Show("Out of turns");
        }

        private void OnClickDisable()
        {
            UIManager.Instance.ShowPopup<PopupMiniNoti>(null).Show($"Next free in: {MyUlti.Int2TimeString((int)timeCoolDown)}");
        }

        private void OnEnable()
        {
            UpdateUI();
        }

        private void OnClickGet()
        {
            MaxAdsManager.Instance.ShowRewardedAd(MaxKeys.rewardedID, () =>
            {
                UserDataManager.AddGold(coinQuantity, "free_coin", reason: ReasonType.watch_ads.ToString());
                OnGotCoin?.Invoke(coinQuantity);
                LastTimeReceiveFreeCoin = DateTime.Now;
                CurrentTimeReceiveFreeCoin++;
                UpdateUI();
            });
        }

        public void UpdateUI()
        {
            if (LastTimeReceiveFreeCoin.Date != DateTime.Now.Date && CurrentTimeReceiveFreeCoin >= maxTimeReceiveFreeCoin)
            {
                CurrentTimeReceiveFreeCoin = 0;
            }

            if (CurrentTimeReceiveFreeCoin >= maxTimeReceiveFreeCoin)
            {
                timeCoolDown = (LastTimeReceiveFreeCoin.AddSeconds(intervalReceiveFreeCoin) - DateTime.Now).TotalSeconds;
            }
            // else
            // {
            //     timeCoolDown = (DateTime.Now.Date.AddDays(1) - LastTimeReceiveFreeCoin).TotalSeconds;
            // }

            m_buttonGet.gameObject.SetActive(timeCoolDown <= 0 && CurrentTimeReceiveFreeCoin < maxTimeReceiveFreeCoin);
            m_buttonDisable.gameObject.SetActive(CurrentTimeReceiveFreeCoin >= maxTimeReceiveFreeCoin);
            //m_buttonDisable.gameObject.SetActive(!m_buttonGet.gameObject.activeInHierarchy && !m_buttonOutOfTurns.gameObject.activeInHierarchy);

            StartCoolDownTime();
        }

        private void StartCoolDownTime()
        {
            if (timeCoolDown > 0)
            {
                StartCoroutine(IEStartCoolDown());
            }
        }

        private IEnumerator IEStartCoolDown()
        {

            while (timeCoolDown > 0)
            {
                // if (CurrentTimeReceiveFreeCoin < maxTimeReceiveFreeCoin)
                // {
                //     timeCoolDown = (LastTimeReceiveFreeCoin.AddSeconds(intervalReceiveFreeCoin) - DateTime.Now).TotalSeconds;
                // }
                // else
                // {
                //     timeCoolDown = (DateTime.Now.Date.AddDays(1) - LastTimeReceiveFreeCoin).TotalSeconds;
                // }
                Debug.Log($"Time CoolDown: {timeCoolDown}");
                timeCoolDown = (LastTimeReceiveFreeCoin.AddSeconds(intervalReceiveFreeCoin) - DateTime.Now).TotalSeconds;
                m_textCoolDown.text = $"{MyUlti.Int2TimeString((int)timeCoolDown)}";

                // if(LastTimeReceiveFreeCoin.Date != DateTime.Now.Date)
                // {
                //     UpdateUI();
                // }    

                yield return new WaitForSeconds(1f);
            }
            
            UpdateUI();

        }
    }
}
