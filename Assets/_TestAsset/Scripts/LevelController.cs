using UnityEngine;

namespace ColorBlockCrush
{
    public class LevelController : SingletonMono<LevelController>
    {
        [Header("Controllers")]
        [SerializeField] private BlockBoardController blockBoardController;
        [SerializeField] private GunBoardController gunBoardController;
        [SerializeField] private SlotController slotController;
        [SerializeField] private ConveyorController conveyorController;

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
            InitializeGame();
        }

        private void InitializeGame()
        {
            CurrentState = GameState.Idle;

            blockBoardController.Init();
            gunBoardController.Init();
            slotController.Init();
            conveyorController.Init();

            RegisterEvents();
            StartGame();
        }

        private void RegisterEvents()
        {
            blockBoardController.OnBoardCleared += OnWin;
        }

        private void StartGame()
        {
            CurrentState = GameState.Playing;

            SpawnTestBlocks();
        }

        private void SpawnTestBlocks()
        {
            for (int c = 0; c < 10; c++)
            {
                for (int r = 0; r < 10; r++)
                {
                    ColorType color = (ColorType)(Random.Range(0, 5));
                    int hp = Random.Range(1, 4);
                    blockBoardController.SpawnBlock(r, c, BlockType.Normal, color, hp, false, true);
                }
            }

            //_blockBoardController.SpawnBlock(1, 2, BlockType.Stone, ColorType.Red, 999);
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