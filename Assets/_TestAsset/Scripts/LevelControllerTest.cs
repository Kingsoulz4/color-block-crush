using UnityEngine;

namespace ColorBlockCrush
{
    public class LevelControllerTest : MonoBehaviour
    {
        public static LevelControllerTest Instance { get; private set; }

        [Header("Controllers")]
        [SerializeField] private BlockBoardController _blockBoardController;
        [SerializeField] private GunBoardController _gunBoardController;
        [SerializeField] private SlotController _slotController;
        //[SerializeField] private AttackManager _attackManager;

        public enum GameState
        {
            Idle,
            Playing,
            Won,
            Lost
        }

        public GameState CurrentState { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            CurrentState = GameState.Idle;

            _blockBoardController.Initialize();
            _gunBoardController.Initialize();
            _slotController.Initialize();

            RegisterEvents();
            StartGame();
        }

        private void RegisterEvents()
        {
            _blockBoardController.OnBoardCleared += OnWin;
            _slotController.OnLoseConditionMet += OnLose;
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
                    _blockBoardController.SpawnBlock(r, c, BlockType.Normal, color, hp, false, true);
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