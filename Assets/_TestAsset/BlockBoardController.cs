using Geckout.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class BlockBoardController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _rows = 30;
        [SerializeField] private int _columns = 10;
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

        private List<Block>[,] _blockStacks;

        public bool IsBoardInAnimationState { get; private set; }
        public int EndRow { get; private set; } = 0;

        public Action OnBoardAnimationsStarted;
        public Action OnBoardAnimationsCompleted;
        public Action OnBoardCleared;

        private int _activeAnimations = 0;

        public void Initialize()
        {
            EndRow = 0;

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
        }

        public Block SpawnBlock(int row, int col, BlockType type, ColorType color, int hp)
        {
            if (!IsValidPosition(row, col)) return null;

            GridNode node = _gridController.GridNodes[row, col];
            Block block = Instantiate(_blockPrefab, node.transform.position, Quaternion.identity, _blockContainer);
            block.name = $"Block_{row}_{col}";

            block.Initialize(type, color, hp);
            block.CurrentNode = node;

            PlaceBlock(row, col, block);

            block.OnBlockDestroyed += OnBlockDestroyed;
            block.OnMovementStarted += OnBlockMovementStarted;
            block.OnMovementCompleted += OnBlockMovementCompleted;

            return block;
        }

        private void PlaceBlock(int row, int col, Block block)
        {
            if (!IsValidPosition(row, col)) return;

            _blockStacks[row, col].Add(block);

            if (block.Type == BlockType.Stone)
            {
                _gridController.GridNodes[row, col].IsBlocked = true;
            }
        }

        private void RemoveBlock(Block block)
        {
            if (block.CurrentNode == null) return;

            GridPoint gridPoint = block.CurrentNode.GridPoint;
            int row = gridPoint.Y;
            int col = gridPoint.X;

            if (IsValidPosition(row, col))
            {
                _blockStacks[row, col].Remove(block);

                if (block.Type == BlockType.Stone)
                {
                    _gridController.GridNodes[row, col].IsBlocked = false;
                }
            }

            block.OnBlockDestroyed -= OnBlockDestroyed;
            block.OnMovementStarted -= OnBlockMovementStarted;
            block.OnMovementCompleted -= OnBlockMovementCompleted;
        }

        public List<Block> GetBottomRowBlocks()
        {
            List<Block> blocks = new List<Block>();

            for (int c = 0; c < _columns; c++)
            {
                if (_blockStacks[EndRow, c].Count > 0)
                {
                    Block baseBlock = _blockStacks[EndRow, c][0];
                    Block topBlock = baseBlock.GetTopMostBlock();
                    blocks.Add(topBlock);
                }
            }

            return blocks;
        }

        public Dictionary<ColorType, List<Block>> GetAttackableBlocksByColor()
        {
            Dictionary<ColorType, List<Block>> dict = new Dictionary<ColorType, List<Block>>();

            List<Block> bottomBlocks = GetBottomRowBlocks();

            foreach (Block block in bottomBlocks)
            {
                if (block.Type == BlockType.Stone) continue;

                if (!dict.ContainsKey(block.Color))
                {
                    dict[block.Color] = new List<Block>();
                }

                dict[block.Color].Add(block);
            }

            return dict;
        }

        private void OnBlockDestroyed(Block block)
        {
            GridPoint pos = block.CurrentNode.GridPoint;
            RemoveBlock(block);

            ShiftDownAbovePieceToEmptyGridPoint(pos.Y, pos.X);
        }

        private void ShiftDownAbovePieceToEmptyGridPoint(int emptyRow, int emptyCol)
        {
            FindBlockToShiftDownResult result = FindBlockToShiftDownRecursive(
                emptyCol,
                emptyRow,
                new List<GridPoint>(),
                new HashSet<(int, int)>()
            );

            if (result != null && result.Block != null && result.Path.Count > 0)
            {
                List<GridNode> pathNodes = new List<GridNode>();
                foreach (GridPoint gp in result.Path)
                {
                    pathNodes.Add(_gridController.GridNodes[gp.Y, gp.X]);
                }

                Block movingBlock = result.Block;
                GridPoint originalPos = movingBlock.CurrentNode.GridPoint;

                _blockStacks[originalPos.Y, originalPos.X].Remove(movingBlock);

                GridPoint finalPos = result.Path[result.Path.Count - 1];
                _blockStacks[finalPos.Y, finalPos.X].Add(movingBlock);

                movingBlock.StartMoveToNodeSequence(pathNodes, () =>
                {
                    if (_blockStacks[originalPos.Y, originalPos.X].Count == 0)
                    {
                        ShiftDownAbovePieceToEmptyGridPoint(originalPos.Y, originalPos.X);
                    }
                });
            }
            else
            {
                CheckIfAllAnimationsComplete();
            }
        }

        private FindBlockToShiftDownResult FindBlockToShiftDownRecursive(
            int col,
            int row,
            List<GridPoint> pathSoFar,
            HashSet<(int, int)> visited)
        {
            if (!IsValidPosition(row, col)) return null;
            if (visited.Contains((row, col))) return null;

            visited.Add((row, col));

            if (_blockStacks[row, col].Count > 0)
            {
                Block block = _blockStacks[row, col][0];
                pathSoFar.Add(new GridPoint(col, row));
                return new FindBlockToShiftDownResult
                {
                    Block = block,
                    Path = new List<GridPoint>(pathSoFar)
                };
            }

            GridNode currentNode = _gridController.GridNodes[row, col];
            List<GridNode> sourceNodes = new List<GridNode>();

            // Get nodes that can fall into this position
            // Look for nodes above (row+1) that are not blocked
            if (row + 1 < _rows)
            {
                GridNode up = _gridController.GridNodes[row + 1, col];
                if (!up.IsBlocked) sourceNodes.Add(up);

                if (col > 0)
                {
                    GridNode upLeft = _gridController.GridNodes[row + 1, col - 1];
                    if (!upLeft.IsBlocked) sourceNodes.Add(upLeft);
                }

                if (col < _columns - 1)
                {
                    GridNode upRight = _gridController.GridNodes[row + 1, col + 1];
                    if (!upRight.IsBlocked) sourceNodes.Add(upRight);
                }
            }

            foreach (GridNode source in sourceNodes)
            {
                List<GridPoint> newPath = new List<GridPoint>(pathSoFar);
                newPath.Add(new GridPoint(col, row));

                FindBlockToShiftDownResult result = FindBlockToShiftDownRecursive(
                    source.GridPoint.X,
                    source.GridPoint.Y,
                    newPath,
                    visited
                );

                if (result != null) return result;
            }

            return null;
        }

        private void OnBlockMovementStarted(Block block)
        {
            _activeAnimations++;

            if (_activeAnimations == 1)
            {
                IsBoardInAnimationState = true;
                OnBoardAnimationsStarted?.Invoke();
            }
        }

        private void OnBlockMovementCompleted(Block block)
        {
            _activeAnimations--;
            CheckIfAllAnimationsComplete();
        }

        private void CheckIfAllAnimationsComplete()
        {
            if (_activeAnimations <= 0)
            {
                _activeAnimations = 0;
                IsBoardInAnimationState = false;
                OnBoardAnimationsCompleted?.Invoke();

                if (GetTotalBlockCount() == 0)
                {
                    OnBoardCleared?.Invoke();
                }
            }
        }

        public int GetTotalBlockCount()
        {
            int count = 0;
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _columns; c++)
                {
                    count += _blockStacks[r, c].Count;
                }
            }
            return count;
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

    public class FindBlockToShiftDownResult
    {
        public Block Block;
        public List<GridPoint> Path;
    }
}