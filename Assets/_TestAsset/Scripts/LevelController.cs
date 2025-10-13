using ColorBlockCrush.Tools;
using ColorBlockCrush;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class LevelController : SingletonMono<LevelController>
    {
        [Header("Controllers")]
        [SerializeField] private BlockBoardController blockBoardController;
        [SerializeField] private GunBoardController gunBoardController;
        [SerializeField] private SlotController slotController;
        [SerializeField] private ConveyorController conveyorController;
        [SerializeField] private TutorialHandGuide m_tutorialHandGuide;


        [SerializeField] LevelConfig levelData;

        public LevelConfig GameLevelData => levelData;
        public BlockBoardController BlockBoardController { get => blockBoardController; }
        public GunBoardController GunBoardController { get => gunBoardController; }
        public SlotController SlotController { get => slotController; }
        public ConveyorController ConveyorController { get => conveyorController; }

        public void SetLevelData(LevelConfig gameLevelData)
        {
            levelData = gameLevelData;
        }

        public void StartLevel()
        {
            InitializeGame();
            GameManager.Instance.SetGameState(GameState.Playing);

        }

        private void InitializeGame()
        {
            blockBoardController.Init(levelData);
            gunBoardController.Init(levelData);
            slotController.Init();
            conveyorController.Init();
        }

        private void WinLevel()
        {
            LevelEvent.OnWin?.Invoke(levelData.levelId);
        }

        private void LoseLevel()
        {
            LevelEvent.OnLose?.Invoke(levelData.levelId);
        }

        private void ReviveLevel()
        {
        }

        public void RestartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }

        #region Boosters

        public void Suffle()
        {
        }

        public void MoveGun(float timeFreeze)
        {
        }

        public void ClearBlock(float timeFreeze)
        {
        }

        #endregion


        #region Tutorial

        public void HideAllTuts()
        {
            m_tutorialHandGuide.gameObject.SetActive(false);
        }

        public void ShowTurialHandGuide(List<Vector3> path)
        {
            m_tutorialHandGuide.ShowGuidePath(path);
        }

        public void ActiveTutLevel1()
        {
        }
        #endregion


    }
}