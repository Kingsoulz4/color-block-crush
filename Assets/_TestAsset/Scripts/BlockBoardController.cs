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
        [SerializeField] private Vector3 _spawnOrigin = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 _maxBlockScale = Vector3.one;
        [SerializeField] private float _maxGridWidth = 10f;
        [SerializeField] private float _maxGridHeight = 10f;
        [SerializeField] private float _minBlockScale = 0.2f;
        [SerializeField] private GridAxisTypes _axisType = GridAxisTypes.XY;
        [SerializeField] private GridAnchorTypes _anchorType = GridAnchorTypes.MiddleCenter;

        [Header("Prefabs")]
        [SerializeField] private Block _blockPrefab;

        [Header("References")]
        [SerializeField] private GridController _gridController;
        [SerializeField] private Transform _blockContainer;

        private int _rows;
        private int _columns;
        private List<Block>[,] _blockStacks;
        private Vector3 _calculatedBlockScale;
        private Vector3 _calculatedBlockOffset;

        public int EndRow { get; private set; } = 0;

        public Action OnBoardCleared;
        public void Init(LevelConfig levelConfig)
        {
            EndRow = 0;
            _rows = levelConfig.mapConfig.mapSize.y;
            _columns = levelConfig.mapConfig.mapSize.x;

            CalculateDynamicScaleAndOffset(_rows, _columns);

            _gridController.PrepareGrid(
                _rows,
                _columns,
                _spawnOrigin,
                _axisType,
                _anchorType,
                _calculatedBlockScale,  
                _calculatedBlockOffset  
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

            block.transform.localScale = _calculatedBlockScale;

            block.name = $"Block_{row}_{col}";
            block.Initialize(type, color, hp, isStatic, canDestroy);
            block.GridNode = node;

            PlaceBlock(row, col, block);
            block.OnBlockDestroyed += OnBlockDestroyed;

            return block;
        }

        private void OnBlockDestroyed(Block block)
        {
            throw new NotImplementedException();
        }

        private void CalculateDynamicScaleAndOffset(int rows, int columns)
        {
            float scaleFactorX = _maxGridWidth / columns;
            float scaleFactorY = _maxGridHeight / rows;

            float scaleFactor = Mathf.Min(scaleFactorX, scaleFactorY);

            scaleFactor = Mathf.Max(scaleFactor, _minBlockScale);

            if (_axisType == GridAxisTypes.XY)
            {
                _calculatedBlockScale = new Vector3(
                    scaleFactor,
                    scaleFactor,
                    _maxBlockScale.z
                );
            }
            else
            {
                _calculatedBlockScale = new Vector3(
                    scaleFactor,
                    _maxBlockScale.y,
                    scaleFactor
                );
            }

            float offsetFactor = 0;

            if (_axisType == GridAxisTypes.XY)
            {
                _calculatedBlockOffset = new Vector3(offsetFactor, offsetFactor, 0);
            }
            else
            {
                _calculatedBlockOffset = new Vector3(offsetFactor, 0, offsetFactor);
            }

            Debug.Log($"Map Size: {columns}x{rows} | Block Scale: {scaleFactor:F2} | Offset: {offsetFactor:F2}");
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