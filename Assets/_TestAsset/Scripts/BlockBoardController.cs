using ColorBlockCrush.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace ColorBlockCrush
{
    public class BlockBoardController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector3 _spawnOrigin = new Vector3(-4.5f, 0, 0);
        [SerializeField] private Vector3 _gridNodeScale = Vector3.one;
        [SerializeField] private Vector3 _gridNodeOffset = Vector3.one;
        [SerializeField] private GridAxisTypes _axisType = GridAxisTypes.XY;
        [SerializeField] private GridAnchorTypes _anchorType = GridAnchorTypes.BottomLeft;

        [Header("Prefabs")]
        [SerializeField] private Block _blockPrefab;

        [Header("References")]
        [SerializeField] private GridController _gridController;
        [SerializeField] private Transform _blockContainer;

        private int _rows;
        private int _columns;
        private List<Block>[,] _blockStacks;

        public bool IsBoardInAnimationState { get; private set; }
        public int EndRow { get; private set; } = 0;

        public Action OnBoardAnimationsStarted;
        public Action OnBoardAnimationsCompleted;
        public Action OnBoardCleared;
        public void Init(LevelConfig levelConfig)
        {
            EndRow = 0;
            _rows = levelConfig.mapConfig.mapSize.y;
            _columns = levelConfig.mapConfig.mapSize.x;
            // Initialize grid using GridController
            _gridController.PrepareGrid(
                _rows,
                _columns,
                _spawnOrigin,
                _axisType,
                _anchorType,
                _gridNodeScale,
                _gridNodeOffset
            );

            _blockStacks = new List<Block>[_rows, _columns];

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _columns; c++)
                {
                    _blockStacks[r, c] = new List<Block>();
                }
            }

            SpawnBlockBoard(levelConfig);
        }
        private void SpawnBlockBoard(LevelConfig levelConfig)
        {
            var listBlock = levelConfig.mapConfig.cells;
            for (int c = 0; c < _columns; c++)
            {
                for (int r = 0; r < _rows; r++)
                {
                    var idx = c * _rows + r;
                    ColorType color = listBlock[idx].colorType;
                    int hp = listBlock[idx].blockHealth;
                    
                    SpawnBlock(r, c, BlockType.Normal, color, hp, false, true);
                }
            }
        }


        public Block SpawnBlock(int row, int col, BlockType type, ColorType color, int hp, bool isStatic, bool canDestroy)
        {
            if (!IsValidPosition(row, col)) return null;

            GridNode node = _gridController.GridNodes[row, col];
            Block block = Instantiate(_blockPrefab, node.transform.position, Quaternion.identity, _blockContainer);
            block.name = $"Block_{row}_{col}";

            block.Initialize(type, color, hp, isStatic,canDestroy);
            block.GridNode = node;

            PlaceBlock(row, col, block);

            block.OnBlockDestroyed += OnBlockDestroyed;

            return block;
        }

        private void OnBlockDestroyed(Block block)
        {
            throw new NotImplementedException();
        }

        private void PlaceBlock(int row, int col, Block block)
        {
            if (!IsValidPosition(row, col)) return;

            _blockStacks[row, col].Add(block);

            if (!block.CanDestroy)
            {
                _gridController.GridNodes[row, col].IsBlocked = true;
            }
        }

        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < _rows && col >= 0 && col < _columns;
        }

        public GridNode GetGridNode(int row, int col)
        {
            if (IsValidPosition(row, col))
                return _gridController.GridNodes[row, col];
            return null;
        }
    }
}