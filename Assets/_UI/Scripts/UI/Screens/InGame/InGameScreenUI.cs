using AYellowpaper.SerializedCollections;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using I2.Loc;
using UnityEngine.UI;
using ColorBlockCrush.Tools;

namespace ColorBlockCrush
{
    public class InGameScreenUI : ScreenUI
    {
        [Space, Header("UI")] [SerializeField] Text txt_Time;
        [SerializeField] LocalizationParamsManager txt_Level_Localize_Param;
        private const string txt_Level_Localize_Param_String = "NUMBER";

        [SerializeField] Button btn_pause;
        [SerializeField] Button btn_Replay;

        [SerializeField] SerializedDictionary<LevelDifficult, GameObject> m_listTimeBarBackground;

        [Space, Header("Booster")] BoosterDataSO boosterDataSO;

        [SerializeField] private GameObject boostersObj;
        [SerializeField] BoosterConfirmUI boosterConfirmUI;

        [Header("Booster Add Tray")] [SerializeField]
        VisualCountBooster addTrayBoosterCount;

        [SerializeField] Button addTrayBoosterBtn;
        [SerializeField] Image img_BG_Time;
        [SerializeField] TimeBoosterTopUI timeBoosterTopUI;

        [Header("Booster hand")] [SerializeField]
        Button handMoveBoosterBtn;

        [SerializeField] VisualCountBooster handMoveBoosterCount;

        [Header("Booster shuffle")] [SerializeField]
        Button shuffleBtn;

        [SerializeField] VisualCountBooster shuffleBoosterCount;

        [Header("Booster Magnet")] [SerializeField]
        Button magnetBoosterBtn;

        [SerializeField] VisualCountBooster magnetBoosterCount;

        [Header("Booster Time")] [SerializeField]
        private GameObject m_iconClock;

        [Space, Header("Top")] [SerializeField]
        RectTransform rect_Top;

        private float currentTopY;

        public TimeBoosterTopUI TimeBoosterTopUI
        {
            get => timeBoosterTopUI;
        }

        private LevelController LevelController => LevelManager.Instance.LevelGame;


        public override void Initialize(UIManager uiManager)
        {
            base.Initialize(uiManager);

            addTrayBoosterBtn.onClick.AddListener(OnAddTrayBoosterClick);
            handMoveBoosterBtn.onClick.AddListener(HandMoveboosterClick);
            shuffleBtn.onClick.AddListener(ShuffleBoosterClick);
            magnetBoosterBtn.onClick.AddListener(MagnetBoosterClick);
            btn_pause.onClick.AddListener(OnPauseClick);
            btn_Replay.onClick.AddListener(OnReplayClick);

            foreach (var booster in BoosterManager.Instance.Boosters)
            {
                booster.OnStartUseBooster += OnStartUseBooster;
                booster.OnChangeBoosterCount += OnChangeBoosterCount;
                booster.OnUseBoosterDone += OnUseBoosterDone;
            }

            boosterDataSO = BoosterManager.Instance.BoosterData;

            addTrayBoosterCount.Init(UserDataManager.AddTrayBooster, BoosterType.ADD_TRAY);
            shuffleBoosterCount.Init(UserDataManager.ShuffleBooster, BoosterType.SHUFFLE);
            handMoveBoosterCount.Init(UserDataManager.HandBooster, BoosterType.HAND_MOVE);
            magnetBoosterCount.Init(UserDataManager.MagnetBooster, BoosterType.MAGNET);
        }

        private void OnDisable()
        {
            HideAllTuts();
        }

        private void OnDestroy()
        {
            HideAllTuts();
            if (BoosterManager.Instance == null) return;
            foreach (var booster in BoosterManager.Instance.Boosters)
            {
                if (!booster) return;
                booster.OnStartUseBooster -= OnStartUseBooster;
                booster.OnUseBoosterDone -= OnUseBoosterDone;
                booster.OnChangeBoosterCount -= OnChangeBoosterCount;
            }
        }

        private void OnChangeBoosterCount(BoosterBase booster, int currentCount)
        {
            switch (booster.BoosterType)
            {
                case BoosterType.ADD_TRAY:
                    addTrayBoosterCount.UpdateTextCountBooster(currentCount);
                    break;
                case BoosterType.HAND_MOVE:
                    handMoveBoosterCount.UpdateTextCountBooster(currentCount);
                    break;
                case BoosterType.SHUFFLE:
                    shuffleBoosterCount.UpdateTextCountBooster(currentCount);
                    break;
                case BoosterType.MAGNET:
                    magnetBoosterCount.UpdateTextCountBooster(currentCount);
                    break;
                default:
                    break;
            }
        }

        private void OnUseBoosterDone(BoosterBase booster, int currentCount)
        {
            switch (booster.BoosterType)
            {
                case BoosterType.ADD_TRAY:
                    break;
                case BoosterType.HAND_MOVE:
                    boostersObj.gameObject.SetActive(true);
                    break;
                case BoosterType.SHUFFLE:
                    break;
                case BoosterType.MAGNET:
                    boostersObj.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }


        private void OnStartUseBooster(BoosterBase booster, int currentCount)
        {
            switch (booster.BoosterType)
            {
                case BoosterType.ADD_TRAY:
                    addTrayBoosterCount.UpdateTextCountBooster(currentCount);
                    break;
                case BoosterType.HAND_MOVE:
                    handMoveBoosterCount.UpdateTextCountBooster(currentCount);
                    boosterConfirmUI.gameObject.SetActive(false);
                    break;
                case BoosterType.SHUFFLE:
                    shuffleBoosterCount.UpdateTextCountBooster(currentCount);
                    break;
                case BoosterType.MAGNET:
                    magnetBoosterCount.UpdateTextCountBooster(currentCount);
                    boosterConfirmUI.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        private void OnAddTrayBoosterClick()
        {
            BoosterManager.Instance.AddTrayBooster.DoShowBooster((sucess) =>
            {
                if (sucess)
                {
                    BoosterManager.Instance.AddTrayBooster.ActiveBooster();
                }
            });
        }

        private void HandMoveboosterClick()
        {
            BoosterManager.Instance.HandMoveBooster.DoShowBooster((sucess) =>
            {
                if (sucess)
                {
                    boosterConfirmUI.gameObject.SetActive(true);
                    var boosterData = boosterDataSO.GetBoosterItemData(BoosterType.HAND_MOVE);
                    Image handMoveImg = handMoveBoosterBtn.GetComponent<VisualCountBooster>().Icon;
                    boosterConfirmUI.SetUIData(boosterData, handMoveImg);
                    boostersObj.gameObject.SetActive(false);
                }
            });
        }

        private void ShuffleBoosterClick()
        {
            BoosterManager.Instance.ShuffleBooster.DoShowBooster((sucess) =>
            {
                if (sucess)
                {
                    BoosterManager.Instance.ShuffleBooster.ActiveBooster();
                }
            });
        }

        private void MagnetBoosterClick()
        {
            BoosterManager.Instance.MagnetBooster.DoShowBooster((sucess) =>
            {
                if (sucess)
                {
                    boosterConfirmUI.gameObject.SetActive(true);
                    var boosterData = boosterDataSO.GetBoosterItemData(BoosterType.MAGNET);
                    Image magnetImg = magnetBoosterBtn.GetComponent<VisualCountBooster>().Icon;
                    boosterConfirmUI.SetUIData(boosterData, magnetImg);
                    boostersObj.gameObject.SetActive(false);
                }
            });
        }

        private void OnPauseClick()
        {
            var popupSetting = UIManager.Instance.ShowPopup<PopupSetting>(() =>
            {
                GameManager.Instance.SetGameState(GameState.Playing);
            });
            popupSetting.SetType(PopupSettingType.IN_GAME);
            GameManager.Instance.SetGameState(GameState.Paused);
        }

        public void OnReplayClick()
        {
            ShowConfirmLeave();
            GameManager.Instance.SetGameState(GameState.Playing);
        }

        public override void Active()
        {
            base.Active();
            currentTopY = rect_Top.anchoredPosition.y;
            UpdateUI();
            PoupNewFeature();
            AudioManager.Instance.StopMusic("BG_Home");
            AudioManager.Instance.PlayMusic("BG_Gameplay", 1, true);
        }

        public void UpdateUI()
        {
            txt_Level_Localize_Param.SetParameterValue(txt_Level_Localize_Param_String,
                LevelManager.Instance.CurrentLevel.ToString());
            UpdateTimeBar();
        }

        private void UpdateTimeBar()
        {
            foreach (var item in m_listTimeBarBackground)
            {
                item.Value.SetActive(false);
            }

            m_listTimeBarBackground[LevelController.GameLevelData.levelDifficult].SetActive(true);
        }

        private void PoupNewFeature()
        {
            //return;
            Debug.Log("New Feature");
            var feature = NewFeatureManager.Instance.GetNewFeatureInProgress();
            Debug.Log($"feature: {feature != null} {LevelManager.Instance.CurrentLevel} {feature.level}");
            if (feature != null &&
                LevelManager.Instance.CurrentLevel ==
                feature.level /* && UserDataManager.LastFeatureCount < feature.level*/)
            {
                var pop = UIManager.Instance.ShowPopup<PopupTutorialNewFeature>(null);
                pop.SetData(feature);
            }
        }

        private void HideAllTuts()
        {
            var popupTut = UIManager.Instance.GetPopupActive<PopupTutorialNewFeature>();
            if (popupTut != null)
            {
                popupTut.Hide();
            }
        }


        public void UpdateTimeText(float timeLevel)
        {
            timeLevel = (int)timeLevel;
            txt_Time.text = GetTimeValueToString(timeLevel);
        }


        public static string GetTimeValueToString(float timeInSeconds)
        {
            int minutes = Mathf.FloorToInt(timeInSeconds / 60);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60);
            if (seconds <= 0 && minutes <= 0)
            {
                return "Finish";
            }

            return string.Format("{0:D2}:{1:D2}", minutes, seconds);
        }

        public void MoveUp()
        {
            rect_Top.DOAnchorPosY(currentTopY + 1000, 0.5f).SetId(this);
        }

        public void MoveDown()
        {
            rect_Top.DOAnchorPosY(currentTopY, 0.5f).SetId(this);
        }

        private void ShowConfirmLeave()
        {
            var popupConfirmLeave = UIManager.Instance.ShowPopup<PopupConfirmLeave>(null);
            popupConfirmLeave.SetTextButtonConfirm("Retry");
            popupConfirmLeave.OnConfirm = () =>
            {
                HeartManager.UseHeart(1);
                LevelManager.Instance.OnRetryGame();
            };
            popupConfirmLeave.OnClose = () => { };
        }

        #region Booster Add Time

        public void ShowAddTimeAnim(int valAdd)
        {
            StartCoroutine(IEAnimateAddTime(valAdd));
        }

        private IEnumerator IEAnimateAddTime(int valAdd)
        {
            yield return new WaitForEndOfFrame();
        }

        public Vector3 GetClockIconPosition()
        {
            return m_iconClock.transform.position;
        }

        #endregion
    }
}