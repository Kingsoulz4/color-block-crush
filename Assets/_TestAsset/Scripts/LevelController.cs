using ColorBlockCrush.Tools;
using Newtonsoft.Json;
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
        [SerializeField] private Button test;

        [SerializeField] LevelConfig levelData;

        public enum GameState
        {
            Idle,
            Playing,
            Won,
            Lost,
            Home
        }

        public GameState CurrentState { get; private set; }
        public BlockBoardController BlockBoardController { get => blockBoardController;}
        public GunBoardController GunBoardController { get => gunBoardController;}
        public SlotController SlotController { get => slotController;}
        public ConveyorController ConveyorController { get => conveyorController;}

        private void Start()
        {
            ParseLevelData();
            InitializeGame();
            test.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(0);
            });
        }

        private void InitializeGame()
        {
            CurrentState = GameState.Idle;

            blockBoardController.Init(levelData);
            gunBoardController.Init(levelData);
            slotController.Init();
            conveyorController.Init();

            RegisterEvents();
            StartGame();
        }

        private void ParseLevelData()
        {
            var ta = Resources.Load<TextAsset>("Level_1");
            var json = ta.text;
            json = QT.DecryptAndDecompress(json, LevelEditorController.pass);
            levelData =  JsonConvert.DeserializeObject<LevelConfig>(json);
            //levelData = ScriptableObject.CreateInstance<LevelConfig>();
            //JsonUtility.FromJsonOverwrite(json, levelData);

        }

        private void RegisterEvents()
        {
            blockBoardController.OnBoardCleared += OnWin;
        }

        private void StartGame()
        {
            CurrentState = GameState.Playing;
        }

       
        public void OnWin()
        {
            if (CurrentState != GameState.Playing) return;

            CurrentState = GameState.Won;
            Debug.Log("YOU WIN!");
        }

        public void OnLose()
        {
            if (CurrentState != GameState.Playing) return;

            CurrentState = GameState.Lost;
            Debug.Log("YOU LOSE!");
        }

        public void RestartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }
}