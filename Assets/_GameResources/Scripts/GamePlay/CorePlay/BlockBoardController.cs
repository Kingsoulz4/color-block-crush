using ColorBlockCrush.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

namespace ColorBlockCrush
{
    public class BlockBoardController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector3 spawnOrigin = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 maxBlockScale = Vector3.one;
        [SerializeField] private float maxGridWidth = 9f;
        [SerializeField] private float maxGridHeight = 9f;
        [SerializeField] private float _minBlockScale = 0.1f;
        [SerializeField] private GridAxisTypes axisType = GridAxisTypes.XZ;
        [SerializeField] private GridAnchorTypes anchorType = GridAnchorTypes.MiddleCenter;

        [Header("Prefabs")]
        [SerializeField] private Block blockPrefab;
        [SerializeField] private BlockKey blockKeyPrefab;

        [Header("References")]
        [SerializeField] private GridController gridController;
        [SerializeField] private Transform blockContainer;
        [SerializeField] private Transform keyContainer;

        private int _rows;
        private int _columns;


        private Block[,] _blockStacks;
        private List<BlockKey> listKey = new();

        private Vector3 calculatedBlockScale;
        private Vector3 calculatedBlockOffset;
        private LevelConfig levelConfig;

        public int EndRow { get; private set; } = 0;

        public Action OnBoardCleared;
        public void Init(LevelConfig levelConfig)
        {
            EndRow = 0;
            this.levelConfig = levelConfig;
            _rows = levelConfig.mapConfig.mapSize.y;
            _columns = levelConfig.mapConfig.mapSize.x;

            CalculateDynamicScaleAndOffset(_rows, _columns);

            gridController.PrepareGrid(
                _rows,
                _columns,
                spawnOrigin,
                axisType,
                anchorType,
                calculatedBlockScale,  
                calculatedBlockOffset  
            );

            _blockStacks = new Block[_rows, _columns];

            SpawnBlockBoard(levelConfig);
            SpawnKeys();
        }
        private void SpawnBlockBoard(LevelConfig levelConfig)
        {
            var listBlock = levelConfig.mapConfig.blocks;
            for (int c = 0; c < _columns; c++)
            {
                for (int r = 0; r < _rows; r++)
                {
                    var idx = c * _rows + r;
                    ColorType color = listBlock[idx].colorType;
                    SpawnBlock(r, c, BlockType.Normal, listBlock[idx]);
                }
            }
        }


        public Block SpawnBlock(int row, int col, BlockType type, BlockConfig blockData)
        {
            if (!IsValidPosition(row, col)) return null;

         
            GridNode node = gridController.GridNodes[row, col];
            Block block = Instantiate(blockPrefab, node.transform.position, Quaternion.identity, blockContainer);
            calculatedBlockScale.y = 0.65f;
            block.transform.localScale = calculatedBlockScale;

            block.name = $"Block_{row}_{col}";
            block.Init(type, blockData);
            block.GridNode = node;

            PlaceBlock(row, col, block);
            block.OnBlockDestroyed += OnBlockDestroyed;

            return block;
        }

        private void SpawnKeys()
        {
            for(int i=0; i<levelConfig.mapConfig.keys.Count; i++)
            {
                if (levelConfig.mapConfig.keys[i].blockId.Count > 0)
                {
                    listKey.Add(SpawnKey(levelConfig.mapConfig.keys[i]));
                }
            }
        }    

        private BlockKey SpawnKey(KeyConfig keyConfig)
        {
            var newKey = Instantiate(blockKeyPrefab, keyContainer);
            newKey.transform.position = CalculateCenter(keyConfig.blockId);
            newKey.Init(keyConfig);
            var directions = new List<Vector2Int>() { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, Vector2Int.one, Vector2Int.one * -1, new Vector2Int(1, -1), new Vector2Int(-1, 1)};

            for (int i = 0; i < keyConfig.blockId.Count; i++)
            {
                (ulong row, ulong col) = CantorPairing.Unpair((ulong)keyConfig.blockId[i]);
                int idReal = (int)col * levelConfig.mapConfig.mapSize.y + (int)row;
                var blockData = levelConfig.mapConfig.blocks[idReal];
                var block = _blockStacks[blockData.coordinate.x, blockData.coordinate.y];
                for(int j=0; j<directions.Count; j++)
                {
                    var dir = directions[j];
                    var coordX = blockData.coordinate.x + dir.x;
                    var coordY = blockData.coordinate.y + dir.y;
                    if (coordX >= 0 && coordX < levelConfig.mapConfig.mapSize.x && coordY >= 0 && coordY < levelConfig.mapConfig.mapSize.y)
                    {
                        var blockNeighbor = _blockStacks[blockData.coordinate.x + dir.x, blockData.coordinate.y + dir.y];
                        newKey.AddBlock(blockNeighbor);
                        blockNeighbor.OnBlockDestroyed += (bl) =>
                        {
                            newKey.CheckCanResolve();
                        };
                    }
                }

            }

            return newKey;
        }

        private Vector3 CalculateCenter(List<int> listBlockId)
        {
            var sumPos = new Vector3();
            for(int i=0; i<listBlockId.Count; i++)
            {
                (ulong row, ulong col) = CantorPairing.Unpair((ulong)listBlockId[i]);
                int idReal = (int)col * levelConfig.mapConfig.mapSize.y + (int)row;
                var blockData = levelConfig.mapConfig.blocks[idReal];
                var block = _blockStacks[blockData.coordinate.x, blockData.coordinate.y];
                sumPos += block.transform.position;
            }

            return sumPos / listBlockId.Count;
        }

        public BlockKey GetPendingKey()
        {
            return listKey.Find(x => !x.IsResolved && x.IsUnBlocked);
        }

        private void OnBlockDestroyed(Block block)
        {
            //throw new NotImplementedException();
        }

        //private void CalculateDynamicScaleAndOffset(int rows, int columns)
        //{
        //    float scaleFactorX = maxGridWidth / columns;
        //    float scaleFactorY = maxGridHeight / rows;

        //    float scaleFactor = Mathf.Min(scaleFactorX, scaleFactorY);

        //    scaleFactor = Mathf.Max(scaleFactor, _minBlockScale);

        //    if (axisType == GridAxisTypes.XY)
        //    {
        //        calculatedBlockScale = new Vector3(
        //            scaleFactor,
        //            scaleFactor,
        //            maxBlockScale.z
        //        );
        //    }
        //    else
        //    {
        //        calculatedBlockScale = new Vector3(
        //            scaleFactor,
        //            maxBlockScale.y,
        //            scaleFactor
        //        );
        //    }

        //    float offsetFactor = 0;

        //    if (axisType == GridAxisTypes.XY)
        //    {
        //        calculatedBlockOffset = new Vector3(offsetFactor, offsetFactor, 0);
        //    }
        //    else
        //    {
        //        calculatedBlockOffset = new Vector3(offsetFactor, 0, offsetFactor);
        //    }

        //    Debug.Log($"Map Size: {columns}x{rows} | Block Scale: {scaleFactor:F2} | Offset: {offsetFactor:F2}");
        //}


        private void PlaceBlock(int row, int col, Block block)
        {
            if (!IsValidPosition(row, col)) return;

            _blockStacks[row, col] = block;

            if (block.BlockData.blockType == BlockType.Stone)
            {
                gridController.GridNodes[row, col].IsBlocked = true;
            }
        }

        private void CalculateDynamicScaleAndOffset(int rows, int columns)
        {
            float spacingRatio = -0.1f;

            // Tính scale c?n thi?t ?? fit trong bound
            float totalUnitsX = columns + (columns - 1) * spacingRatio;
            float totalUnitsY = rows + (rows - 1) * spacingRatio;

            float scaleFactorX = maxGridWidth / totalUnitsX;
            float scaleFactorY = maxGridHeight / totalUnitsY;

            float scaleFactor = Mathf.Min(scaleFactorX, scaleFactorY, 1f);
            scaleFactor = Mathf.Max(scaleFactor, _minBlockScale);

            // Apply scale
            if (axisType == GridAxisTypes.XY)
            {
                calculatedBlockScale = new Vector3(scaleFactor, scaleFactor, maxBlockScale.z);
                calculatedBlockOffset = new Vector3(scaleFactor * spacingRatio, scaleFactor * spacingRatio, 0);
            }
            else // XZ
            {
                calculatedBlockScale = new Vector3(scaleFactor, maxBlockScale.y, scaleFactor);
                calculatedBlockOffset = new Vector3(scaleFactor * spacingRatio, 0, scaleFactor * spacingRatio);
            }

            Debug.Log($"Map: {columns}x{rows} | Scale: {scaleFactor:F3} | BlockSize: {calculatedBlockScale.x:F3}");
        }

        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < _rows && col >= 0 && col < _columns;
        }

        public GridNode GetGridNode(int row, int col)
        {
            if (IsValidPosition(row, col))
                return gridController.GridNodes[row, col];
            return null;
        }
    }
}