using System;
using ColorBlockCrush.Tools;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Analytics;
using Yoolax.Framework;

namespace ColorBlockCrush
{
    public class LevelManager : SingletonDontDestroyMono<LevelManager>
    {
        [SerializeField] private LevelController m_levelGameOriginal;
        [SerializeField] private int levelTest;

        private SortedDictionary<int, IFlowCallback> queueFlowStartGame = new();

        public bool IsEdittingLevel { get; set; } = false;

        private LevelController levelGame;

        private int priceRevive = 900;

        public DateTime timeStart;
        public bool inGameplay = false;

        public LevelController LevelGame
        {
            get
            {
                if (levelGame == null)
                {
                    levelGame = Instantiate(m_levelGameOriginal);
                }

                return levelGame;
            }
            set
            {
                levelGame = value;
            }
        }

        public int CurrentLevel
        {
            get
            {
                return UserDataManager.Level;
            }
            set
            {
                UserDataManager.Level = value;
            }
        }

        public int CurrentLevelSetID
        {
            get
            {
                return UserDataManager.LevelSetID;
            }
            set
            {
                UserDataManager.LevelSetID = value;
            }
        }

        private void OnEnable()
        {
            LevelEvent.OnWin += OnWinGame;
            LevelEvent.OnLose += OnLoseGame;
            LevelEvent.OnRevive += OnReviveGame;

        }

        private void OnDisable()
        {
            LevelEvent.OnWin -= OnWinGame;
            LevelEvent.OnLose -= OnLoseGame;
            LevelEvent.OnRevive -= OnReviveGame;
        }
        
        public void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                UserDataManager.ExitIndex++;
                LevelAnalyticStruct levelAnalyticStruct = new LevelAnalyticStruct();
                levelAnalyticStruct = levelAnalyticStruct.SetBaseLevel().SetLevelExitStruct(UserDataManager.PlayType,
                    LevelController.Instance.GunBoardController.TotalGunCount, 
                    (float)(DateTime.Now - timeStart).TotalSeconds, UserDataManager.ExitIndex);
                Server.Get<OnLevelExitEventLog>().Dispatch(levelAnalyticStruct);
            }
            else
            {
                LevelAnalyticStruct levelAnalyticStruct = new LevelAnalyticStruct();
                levelAnalyticStruct = levelAnalyticStruct.SetBaseLevel().SetLevelReopenStruct();
                Server.Get<OnLevelReopenEventLog>().Dispatch(levelAnalyticStruct);
            }
        }

        public void StartCurrentLevel()
        {
            LevelAnalyticStruct levelAnalyticStruct = new LevelAnalyticStruct();
            levelAnalyticStruct = levelAnalyticStruct.SetBaseLevel().SetLevelStartStruct(UserDataManager.PlayType,
                0);       
            Server.Get<OnLevelStartEventLog>().Dispatch(levelAnalyticStruct);
            timeStart = DateTime.Now;
            StartLevel(CurrentLevel, CurrentLevelSetID);
        }

        public void StartLevel(int level, int levelSetID = 0)
        {
            var levelData = LoadCurrentLevel(level);
            if (LevelTestManager.Instance)
                if (LevelTestManager.Instance.currentLevelPlay != null)
                    levelData = LevelTestManager.Instance.currentLevelPlay;
            //#if !UNITY_EDITOR
            Destroy(LevelGame.gameObject);
            LevelGame = Instantiate(m_levelGameOriginal);
            //#endif
            LevelGame.SetLevelData(levelData);
            OnStartGame(CurrentLevel);
        }

        public LevelConfig LoadCurrentLevel(int level)
        {
            int levelId = level;
            if (level > FetchLevelManager.Instance.maxLevel)
            {
                levelId = FetchLevelManager.Instance.GetLoopLevelId(level);
            }
            LevelConfig levelConfig = UserDataManager.GetCurrentLevel(levelId);

            if (levelConfig == null)
            {
                levelConfig = LoadLevel(levelId, UserDataManager.LevelSetID);
            }
            else
            {
                Debug.Log($"Have Cache Level {level}, Loop {levelId}");
            }

            return levelConfig;
        }

        public LevelConfig LoadLevel(int level, int levelSetID)
        {
            var textLv = Resources.Load<TextAsset>($"Levels/{levelSetID}/Level_{level}");
            if (textLv == null)
            {
                textLv = Resources.Load<TextAsset>($"Levels/0/Level_{level}");
                if (textLv == null)
                {
                    textLv = Resources.Load<TextAsset>($"Levels/0/Level_{1}");
                }
            }
            return ParseLevelData(textLv);
        }

        private LevelConfig ParseLevelData(TextAsset textLv)
        {
            var json = textLv.text;
            json = QT.DecryptAndDecompress(json, LevelEditorController.pass);
            var levelData = JsonConvert.DeserializeObject<LevelConfig>(json);
            return levelData;
        }

        public LevelDifficult GetCurrentLevelType()
        {
            var levelData = LoadCurrentLevel(CurrentLevel);
            return levelData.levelDifficult;
        }

        #region GameState

        public void OnLoseGame(int level)
        {
            if (GameManager.GameState == GameState.Lose)
            {
                return;
            }
            GameManager.Instance.SetGameState(GameState.Lose);
            if (!LevelController.Instance.CheckCanRevive())
            {
                var popupLose = UIManager.Instance.ShowPopup<PopupLose>(null);

                //HeartManager.UseHeart(1);

                popupLose.OnClose = () =>
                {
                    var loading = UIManager.Instance.ShowScreen<LoadingScreen>();
                    loading.Show(() =>
                    {
                        UIManager.Instance.ShowScreen<MainScreenUI>();
                    });
                };
                popupLose.OnRetry = OnRetryGame;
            }
            else
            {
                var popupRevival = UIManager.Instance.ShowPopup<PopupOutOfSpace>(null);
                popupRevival.OnClose = () =>
                {
                    var popupLose = UIManager.Instance.ShowPopup<PopupLose>(null);

                    //HeartManager.UseHeart(1);

                    popupLose.OnClose = () =>
                    {
                        var loading = UIManager.Instance.ShowScreen<LoadingScreen>();
                        loading.Show(() =>
                        {
                            UIManager.Instance.ShowScreen<MainScreenUI>();
                        });
                    };
                    popupLose.OnRetry = OnRetryGame;
                };
            }
        }

        public void OnRetryGame()
        {
            if (UserDataManager.Heart > 0)
            {
                HeartManager.UseHeart(1);
                UserDataManager.PlayType = PlayType.restart;
                StartCurrentLevel();
                var loading = UIManager.Instance.ShowScreen<LoadingScreen>();
                loading.Show(() =>
                {
                    UIManager.Instance.ShowScreen<InGameScreenUI>();
                });
            }
            else
            {
                var popupGetMoreLives = UIManager.Instance.ShowPopup<PopupGetMoreLives>(null);
                popupGetMoreLives.OnRefilled = () =>
                {
                    UserDataManager.PlayType = PlayType.restart;
                    StartCurrentLevel();
                };
                popupGetMoreLives.OnClose = () =>
                {
                    var loading = UIManager.Instance.ShowScreen<LoadingScreen>();
                    loading.Show(() =>
                    {
                        UIManager.Instance.ShowScreen<MainScreenUI>();
                    });
                };
            }
        }

        public void OnReviveGame(int price)
        {
            GameManager.Instance.SetGameState(GameState.Playing);
            // if (UserDataManager.Gold >= price)
            // {
            //     UserDataManager.AddGold(-price, "Revival");
            //     
            // }
            // else
            // {
            //     GameManager.Instance.SetGameState(GameState.Paused);
            //
            //     UIManager.Instance.ShowPopup<PopupShop>(() =>
            //     {
            //         GameManager.Instance.SetGameState(GameState.Playing);
            //     });
            // }
        }

        public void OnPauseGame(int level)
        {
            throw new System.NotImplementedException();
        }


        public void OnStartGame(int level)
        {
            LevelGame.StartLevel();

            CheckShowTutorials();

            var popupTutNewFeature = UIManager.Instance.ShowPopup<PopupTutorialNewFeature>(null);
            InjectToFlowStartGame(popupTutNewFeature, 1);
            var popupTutNewBooster = UIManager.Instance.GetPopup<PopupTutorialNewBooster>();
            InjectToFlowStartGame(popupTutNewBooster, 2);
            var popupWarningDifficultLevel = UIManager.Instance.GetPopup<PopupWarningDifficultLevel>();
            InjectToFlowStartGame(popupWarningDifficultLevel, 0);

            ExecuteNextFlowStep();

            LevelEvent.OnLevelStart?.Invoke(level);
        }

        private void CheckShowTutorials()
        {
            if (CurrentLevel == 1 || CurrentLevel == 2)
            {
                LevelGame.ActiveTutLevel1();
            }
        }

        public void OnWinGame(int level)
        {
            GameManager.Instance.SetGameState(GameState.Win);
            var popupWin = UIManager.Instance.ShowPopup<PopupWin>(null);
            
            popupWin.OnClaimedReward = (val) =>
            {
                CurrentLevel++;
                if (UserDataManager.Level < GameManager.Instance.levelTriggerData.levelBackToHome
                    && UserDataManager.Session <= 1)
                {
                    popupWin.ShowClaimReward(val, NextLevel);
                    //NextLevel();   
                }
                else
                {
                    if (UserDataManager.Level == GameManager.Instance.levelTriggerData.levelShowPopupRate)
                    {
                        var popupRate = UIManager.Instance.ShowPopup<PopupRateGame>(() =>
                        {
                            popupWin.Hide();
                            var loading = UIManager.Instance.ShowScreen<LoadingScreen>();
                            loading.Show(() =>
                            {
                                var mainScreen = UIManager.Instance.ShowScreen<MainScreenUI>();
                                mainScreen.ShowClaimReward(val);   
                            });
                        });
                        popupRate.SetRate(3);
                    }
                    else
                    {
                        var loading = UIManager.Instance.ShowScreen<LoadingScreen>();
                        popupWin.Hide();
                        loading.Show(() =>
                        {
                            var mainScreen = UIManager.Instance.ShowScreen<MainScreenUI>();
                            mainScreen.ShowClaimReward(val);   
                        });
                    }
                }
            };
        }

        public void NextLevel()
        {
            StartCurrentLevel();
            
            var loading = UIManager.Instance.ShowScreen<LoadingScreen>();
            loading.Show(() =>
            {
                UIManager.Instance.ShowScreen<InGameScreenUI>();
            });
        }
        #endregion

        #region Flow Start Game

        public void InjectToFlowStartGame(IFlowCallback step, int priority = -1)
        {
            if (priority < 0)
            {
                queueFlowStartGame[queueFlowStartGame.Count] = step;
            }
            else
            {
                queueFlowStartGame[priority] = step;
            }
        }

        public void ExecuteNextFlowStep()
        {
            if (queueFlowStartGame.Count <= 0)
            {
                DoneFlowStartGame();
                return;
            }

            var topElement = queueFlowStartGame.First();
            queueFlowStartGame.Remove(topElement.Key);
            topElement.Value.Execute(ExecuteNextFlowStep);

        }

        public void DoneFlowStartGame()
        {
            Debug.Log($"Done");
            GameManager.Instance.SetGameState(GameState.Playing);
        }

        #endregion
    }
}
