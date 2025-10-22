using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using ColorBlockCrush.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    [DefaultExecutionOrder(-10)]
    public class TestManager : SingletonMono<TestManager>
    {
        [SerializeField] Button btn_Load;
        [SerializeField] Button btn_nextLevel;
        [SerializeField] Button btn_backLevel;
        [SerializeField] Button btn_setCoins;
        [SerializeField] InputField inputField;
        [SerializeField] InputField inputLevelSet;
        [SerializeField] InputField inputCoins;
        [SerializeField] private Button btnTestLose;
        [SerializeField] private Button btnTestWin;
        [SerializeField] private Button btnExit;
        [SerializeField] private GameObject cheatingPanel;
        [SerializeField] private Toggle adsOffToogle;
        [SerializeField] private Toggle testServerToogle;
        
        
        public static bool IsCheating
        {
            get => PlayerPrefs.GetInt("IsCheating", 0) > 0;
            set => PlayerPrefs.SetInt("IsCheating", value? 1: 0);
        }
        
        public static bool IsAdsOff
        {
            get
            {
                return PlayerPrefs.GetInt("debugAdsoff", 0) == 1;
            }

            set
            {
                PlayerPrefs.SetInt("debugAdsoff", value ? 1 : 0);
            }
        }

        public void UpdateState()
        {
            cheatingPanel.SetActive(IsCheating);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                btn_backLevel.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                btn_nextLevel.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                var ingameUI = UIManager.Instance.GetScreenActive<InGameScreenUI>();
                ingameUI.OnReplayClick();
            }
        }

        private void Start()
        {
            //AddRes();

            UpdateState();
            BoosterManager.Instance.UpdateVisualBooster();
            btn_Load.onClick.AddListener(LoadLevel);
            btn_backLevel.onClick.AddListener(() =>
            {
                UserDataManager.AddHeart(1, "test", false);
                LevelManager.Instance.StartLevel(LevelManager.Instance.CurrentLevel -= 1);
                UIManager.Instance.GetScreenActive<InGameScreenUI>().UpdateUI();
            });
            btn_nextLevel.onClick.AddListener(() =>
            {
                UserDataManager.AddHeart(1, "test", false);
                LevelManager.Instance.StartLevel(LevelManager.Instance.CurrentLevel += 1);
                UIManager.Instance.GetScreenActive<InGameScreenUI>().UpdateUI();
            });

            if (inputLevelSet != null)
            {
                inputLevelSet.onSubmit.AddListener((s) =>
                {
                    LevelManager.Instance.CurrentLevelSetID = int.Parse(inputLevelSet.text);
                });
            }    
            
            btn_setCoins.onClick.AddListener(SetCoinButton);
            
            btnTestLose.onClick.AddListener(LoseLevel);
            
            btnTestWin.onClick.AddListener(() =>
            {
                LevelEvent.OnWin?.Invoke(LevelManager.Instance.CurrentLevel);
            });
            
            btnExit.onClick.AddListener(ReturnLevelEditor);
            
            adsOffToogle.onValueChanged.AddListener(OnAdsOffTogle);
            adsOffToogle.isOn = IsAdsOff;
        }

        private void LoadLevel()
        {
            if (string.IsNullOrEmpty(inputField.text))
            {
                return;
            }

            if (int.Parse(inputField.text) == -1)
            {
                //AddRes();
                return;
            }

            LevelManager.Instance.CurrentLevel = int.Parse(inputField.text);
            LevelManager.Instance.StartCurrentLevel();
        }

        private void LoseLevel()
        {
            Action onClose = () =>
            {
                LevelEvent.OnLose?.Invoke(LevelManager.Instance.CurrentLevel);
            };

            Action onrevival = () =>
            {

            };
            
            var popupOutOfSpace = UIManager.Instance.ShowPopup<PopupOutOfSpace>(() => { });
            popupOutOfSpace.Show(null);
            popupOutOfSpace.OnClose = () =>
            {
                var popupLose = UIManager.Instance.ShowPopup<PopupLose>(null);

                HeartManager.UseHeart(1);

                popupLose.OnClose = () =>
                {
                    var loading = UIManager.Instance.ShowScreen<LoadingScreen>();
                    loading.Show(() =>
                    {
                        UIManager.Instance.ShowScreen<MainScreenUI>();
                    });
                };
                popupLose.OnRetry = LevelManager.Instance.OnRetryGame;
            };
            //popupOutOfSpace.OnKeepPlaying = LevelManager.Instance.OnReviveGame;
        }

        private void ReturnLevelEditor()
        {
            if (LevelTestManager.Instance.currentLevelPlay != null)
            {
                LevelTestManager.Instance.currentLevelPlay = null;
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
                SceneManager.LoadScene(0);
            }
        }
        
        private void SetCoinButton()
        {
            if (int.TryParse(inputCoins.text, out int result))
            {
                UserDataManager.AddGold(1000, "set coin", false);;
                PlayerPrefs.Save();
            }
        }
        
        private void OnAdsOffTogle(bool arg0)
        {
            IsAdsOff = arg0;
        }

        private void AddRes()
        {
            UserDataManager.AddGold(1000, "test", false);
            UserDataManager.AddHeart(5, "test", false);
            UserDataManager.ShuffleBooster += 3;
            UserDataManager.HandBooster += 3;
            UserDataManager.AddTrayBooster += 3;
        }
    }
}
