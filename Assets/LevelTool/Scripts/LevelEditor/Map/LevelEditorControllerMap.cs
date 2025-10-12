using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;

namespace ColorBlockCrush.Tools
{
    public partial class LevelEditorController : MonoBehaviour
    {
        [SerializeField]
        private int mapWidth = 30;
        [SerializeField]
        private int mapHeight = 30;
        
        [SerializeField] 
        private ColorType currentColorChoose = ColorType.Red;

        [SerializeField] private DragType currentDragType = DragType.Normal;
        
        private Action<List<GridCellMapView>> onUpdateSelection;
        private Action<List<GridCellMapView>> onDeleteSelection;
        
        [SerializeField]
        private List<GridCellMapView> _gridCellMapViews = new List<GridCellMapView>();
        [SerializeField]
        private List<BlockInforEditorView> _blockInforEditorViews = new List<BlockInforEditorView>();
        [SerializeField]
        private List<KeyInforEditorView> _keyInforEditorViews = new List<KeyInforEditorView>();

        private void ValidateMapWidth(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Invalid map Width");
                return;
            }

            int newMapWidth = int.Parse(value);

            if (newMapWidth <= 0 || newMapWidth > model.maxMapSize ||  newMapWidth < model.minMapSize)
            {
                Debug.LogError("Invalid map Width");
            }
                
            OnChangeMapWidth(newMapWidth);
        }
        
        private void OnChangeMapWidth(int newMapWidth)
        {
            mapWidth = newMapWidth;
        }

        private void ValidateMapWHeight(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Invalid map Height");
                return;
            }

            int newMapHeight = int.Parse(value);

            if (newMapHeight <= 0  || newMapHeight > model.maxMapSize ||  newMapHeight < model.minMapSize)
            {
                Debug.LogError("Invalid map Height");
            }
                
            OnChangeMapHeight(newMapHeight);
        }
        
        private void OnChangeMapHeight(int newMapHeight)
        {
            mapHeight = newMapHeight;
        }
        
        private void CreateMapEmpty()
        {
            ClearAllMap();
            _gridCellMapViews = new List<GridCellMapView>();

            float gridCellSize = (float)(model.gridContainer.sizeDelta.x / (float)GetMapSize());
            model.gridLayoutGroup.constraintCount = mapWidth;
            model.gridLayoutGroup.cellSize = new Vector2(gridCellSize, gridCellSize);
            currentLevelConfig.mapConfig = new MapConfig();
            currentLevelConfig.mapConfig.mapSize = new Vector2Int(mapWidth, mapHeight);
            
            for (int i = 0; i < mapHeight; i++)
            {
                for (int j = 0; j < mapWidth; j++)
                {
                    CellConfig cellConfig = new CellConfig();
                    cellConfig.id = (int)CantorPairing.MakeId((ulong)i, (ulong)j);
                    cellConfig.coordinate = new Vector2Int(i, j);
                    currentLevelConfig.mapConfig.cells.Add(cellConfig);
                    
                    GridCellMapView gridCellMapView = Instantiate(model.gridCellMapViewPrefab, model.gridContainer)
                        .GetComponent<GridCellMapView>();
                    gridCellMapView.cellConfig = cellConfig;
                    _gridCellMapViews.Add(gridCellMapView);
                }
            }
            
            view.dragCellMapSelection.enabled = true;
            DOVirtual.DelayedCall(.55f, () => view.dragCellMapSelection.Init(mapWidth, mapHeight, model.gridContainer, _gridCellMapViews,
                onUpdateSelection, onDeleteSelection));
            
        }

        private void CreateMapByPicture()
        {
            InputImageConfig inputImageConfig = currentLevelConfig.imageConfig;
            mapWidth = (int)inputImageConfig.ImageSize.x;
            mapHeight = (int)inputImageConfig.ImageSize.y;

            view.WidthMapSize.text = mapWidth.ToString();
            view.HeightMapSize.text = mapHeight.ToString();
            
            _gridCellMapViews = new List<GridCellMapView>();

            float gridCellSize = (float)(model.gridContainer.sizeDelta.x / (float)GetMapSize());
            model.gridLayoutGroup.constraintCount = mapWidth;
            model.gridLayoutGroup.cellSize = new Vector2(gridCellSize, gridCellSize);
            currentLevelConfig.mapConfig = new MapConfig();
            currentLevelConfig.mapConfig.mapSize = new Vector2Int(mapWidth, mapHeight);
            
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    CellConfig cellConfig = new CellConfig();
                    cellConfig.id = (int)CantorPairing.MakeId((ulong)i, (ulong)j);
                    cellConfig.coordinate = new Vector2Int(i, j);
                    cellConfig.colorType = currentColorArray[i, j];
                    currentLevelConfig.mapConfig.cells.Add(cellConfig);
                    
                    GridCellMapView gridCellMapView = Instantiate(model.gridCellMapViewPrefab, model.gridContainer)
                        .GetComponent<GridCellMapView>();
                    gridCellMapView.cellConfig = cellConfig;
                    gridCellMapView.UpdateColor(currentColorArray[i, j]);
                    _gridCellMapViews.Add(gridCellMapView);
                }
            }

            view.dragCellMapSelection.enabled = true;
            DOVirtual.DelayedCall(2.5f, () => view.dragCellMapSelection.Init(mapWidth, mapHeight, model.gridContainer, _gridCellMapViews,
                onUpdateSelection, onDeleteSelection));
        }

        private void UpdateMapData()
        {
            MapConfig mapConfig = currentLevelConfig.mapConfig;

            mapWidth = mapConfig.mapSize.x;
            mapHeight = mapConfig.mapSize.y;
            
            view.WidthMapSize.text = mapWidth.ToString();
            view.HeightMapSize.text = mapHeight.ToString();
            float gridCellSize = (float)(model.gridContainer.sizeDelta.x / (float)GetMapSize());
            model.gridLayoutGroup.constraintCount = mapWidth;
            model.gridLayoutGroup.cellSize = new Vector2(gridCellSize, gridCellSize);
            
            _gridCellMapViews = new List<GridCellMapView>();
            
            for (int i = 0; i < mapConfig.mapSize.x; i++)
            {
                for (int j = 0; j < mapConfig.mapSize.y; j++)
                {
                    GridCellMapView gridCellMapView = Instantiate(model.gridCellMapViewPrefab, model.gridContainer)
                        .GetComponent<GridCellMapView>();
                    int cellIndex = i * mapConfig.mapSize.y + j;
                    gridCellMapView.cellConfig = mapConfig.cells[cellIndex];
                    gridCellMapView.UpdateColor(mapConfig.cells[cellIndex].colorType);
                    _gridCellMapViews.Add(gridCellMapView);
                }
            }
            
            view.dragCellMapSelection.enabled = true;
            DOVirtual.DelayedCall(.55f, () =>
            {
                view.dragCellMapSelection.Init(mapWidth, mapHeight, model.gridContainer, _gridCellMapViews,
                    onUpdateSelection, onDeleteSelection);
                
                List<GridCellMapView> cellGridBlockList = new List<GridCellMapView>();
                for (int i = 0; i < mapConfig.blocks.Count; i++)
                {
                    cellGridBlockList.Clear();
                    cellGridBlockList = _gridCellMapViews.Where(cell => mapConfig.blocks[i].cellsId.Contains(cell.cellConfig.id)).ToList();
                    BlockInforEditorView blockInforView =
                        Instantiate(model.blockInforPrefab).GetComponent<BlockInforEditorView>();
                    blockInforView.UpdateInfor(i, view.mapFeatureParent, 
                        cellGridBlockList.ConvertAll(cell => cell.transform as RectTransform), 
                        canvas, mapConfig.blocks[i].blockHealth, mapConfig.blocks[i].colorType);
                    _blockInforEditorViews.Add(blockInforView);
                }

                for (int i = 0; i < mapConfig.keys.Count; i++)
                {
                    cellGridBlockList.Clear();
                    cellGridBlockList = _gridCellMapViews.Where(cell => mapConfig.keys[i].cellsId.Contains(cell.cellConfig.id)).ToList();
                    KeyInforEditorView keyInforView =
                        Instantiate(model.keyInforPrefab).GetComponent<KeyInforEditorView>();
                    keyInforView.UpdateInfor(i, view.mapFeatureParent, 
                        cellGridBlockList.ConvertAll(cell => cell.transform as RectTransform), 
                        canvas);
                    _keyInforEditorViews.Add(keyInforView);
                }
            });
        }
        
        public void SetStateGridCell(List<GridCellMapView> cellSelection)
        {
            if (currentDragType == DragType.Normal)
            {
                foreach (var cellMapView in cellSelection)
                {
                    cellMapView.UpdateColor(currentColorChoose);
                
                    currentLevelConfig.mapConfig.cells[cellMapView.Col * currentLevelConfig.mapConfig.mapSize.y
                                                       + cellMapView.Row].colorType = currentColorChoose;
                }
            }
            else if(currentDragType == DragType.Block)
            {
                string blockHealth = view.blockHealthInputField.text;
                if (string.IsNullOrEmpty(blockHealth))
                {
                    Debug.LogError("Invalid Block Health");
                    return;
                }
                
                BlockConfig blockConfig = new BlockConfig();
                blockConfig.blockGroupId = currentLevelConfig.mapConfig.blocks.Count;
                blockConfig.colorType = currentColorChoose;
                blockConfig.blockHealth = int.Parse(blockHealth);
                
                for (int i = 0; i < cellSelection.Count; i++)
                {
                    cellSelection[i].DeleteColor();

                    int cellIndex = cellSelection[i].Col * currentLevelConfig.mapConfig.mapSize.y
                                    + cellSelection[i].Row;
                    currentLevelConfig.mapConfig.cells[cellIndex].colorType = ColorType.None;
                    currentLevelConfig.mapConfig.cells[cellIndex].blockGroupId = blockConfig.blockGroupId;
                    blockConfig.cellsId.Add(currentLevelConfig.mapConfig.cells[cellIndex].id);
                }
                
                currentLevelConfig.mapConfig.blocks.Add(blockConfig);
                
                BlockInforEditorView blockInforView =
                    Instantiate(model.blockInforPrefab).GetComponent<BlockInforEditorView>();
                blockInforView.UpdateInfor(blockConfig.blockGroupId, view.mapFeatureParent, 
                    cellSelection.ConvertAll(cell => cell.transform as RectTransform), 
                    canvas, int.Parse(blockHealth), currentColorChoose);
                _blockInforEditorViews.Add(blockInforView);
            }
            else if(currentDragType == DragType.Key)
            {
                KeyConfig keyConfig = new KeyConfig();
                keyConfig.keyId = currentLevelConfig.mapConfig.keys.Count;
                
                for (int i = 0; i < cellSelection.Count; i++)
                {
                    int cellIndex = cellSelection[i].Col * currentLevelConfig.mapConfig.mapSize.y
                                    + cellSelection[i].Row;
                    currentLevelConfig.mapConfig.cells[cellIndex].keyId = keyConfig.keyId;
                    keyConfig.cellsId.Add(currentLevelConfig.mapConfig.cells[cellIndex].id);
                }
                
                currentLevelConfig.mapConfig.keys.Add(keyConfig);
                
                KeyInforEditorView keyInforView =
                    Instantiate(model.keyInforPrefab).GetComponent<KeyInforEditorView>();
                keyInforView.UpdateInfor(keyConfig.keyId, view.mapFeatureParent, 
                    cellSelection.ConvertAll(cell => cell.transform as RectTransform), 
                    canvas);
                _keyInforEditorViews.Add(keyInforView);
            }
        }
        
        public void DeleteStateGridCell(List<GridCellMapView> cellSelection)
        {
            if (currentDragType == DragType.Normal)
            {
                foreach (var cellMapView in cellSelection)
                {
                    cellMapView.DeleteColor();
                    currentLevelConfig.mapConfig.cells[cellMapView.Col * currentLevelConfig.mapConfig.mapSize.y
                                                       + cellMapView.Row].colorType = ColorType.None;
                }
            }
            else if (currentDragType == DragType.Block)
            {
                HashSet<int> blocksRemoved = new HashSet<int>();
                List<BlockConfig> blockConfigsRemove = new List<BlockConfig>();
                
                foreach (var cellMapView in cellSelection)
                {
                    cellMapView.DeleteColor();
                    currentLevelConfig.mapConfig.cells[cellMapView.Col * currentLevelConfig.mapConfig.mapSize.y
                                                       + cellMapView.Row].colorType = ColorType.None;
                    if (cellMapView.cellConfig.blockGroupId != -1 
                        && !blocksRemoved.Contains(cellMapView.cellConfig.blockGroupId))
                    {
                        blocksRemoved.Add(cellMapView.cellConfig.blockGroupId);
                        BlockInforEditorView blockInforViewRemove =
                            _blockInforEditorViews.FirstOrDefault(blocksRemove =>
                                blocksRemove.GetKeyId() == cellMapView.cellConfig.blockGroupId);
                        if (blockInforViewRemove != null)
                        {
                            _blockInforEditorViews.Remove(blockInforViewRemove);
                            Destroy(blockInforViewRemove.gameObject);
                            Debug.Log($"Remove Block {cellMapView.cellConfig.blockGroupId}");
                        }
                        BlockConfig blockConfigRenove = currentLevelConfig.mapConfig.blocks[cellMapView.cellConfig.blockGroupId];
                        currentLevelConfig.mapConfig.blocks.Remove(blockConfigRenove);
                        blockConfigsRemove.Add(blockConfigRenove);
                    }
                }
                
                ReUpdateBlockId();

                Debug.Log("Block Config Count " + blocksRemoved.Count);
                if (blockConfigsRemove.Count > 0)
                {
                    foreach (var blockConfig in blockConfigsRemove)
                    {
                        foreach (var cellId in blockConfig.cellsId)
                        {
                            (ulong row, ulong col) = CantorPairing.Unpair((ulong)cellId);
                            int cellIndex = (int)col * currentLevelConfig.mapConfig.mapSize.y
                                            + (int)row;
                            Debug.Log("Cell Index " + cellIndex);
                            _gridCellMapViews[cellIndex].cellConfig.blockGroupId = -1;
                        }
                    }
                }
            }
            else if (currentDragType == DragType.Key)
            {
                HashSet<int> keysIdRemoved = new HashSet<int>();
                List<KeyConfig> keyConfigsRemove = new List<KeyConfig>();
                
                foreach (var cellMapView in cellSelection)
                {
                    if (cellMapView.cellConfig.keyId != -1 
                        && !keysIdRemoved.Contains(cellMapView.cellConfig.keyId))
                    {
                        keysIdRemoved.Add(cellMapView.cellConfig.keyId);
                        KeyInforEditorView keyInforViewRemove =
                            _keyInforEditorViews.FirstOrDefault(keyRemove =>
                                keyRemove.GetKeyId() == cellMapView.cellConfig.keyId);
                        if (keyInforViewRemove != null)
                        {
                            _keyInforEditorViews.Remove(keyInforViewRemove);
                            Destroy(keyInforViewRemove.gameObject);
                            Debug.Log($"Remove Key {cellMapView.cellConfig.keyId}");
                        }
                        KeyConfig keyConfigRemove = currentLevelConfig.mapConfig.keys[cellMapView.cellConfig.keyId];
                        currentLevelConfig.mapConfig.keys.Remove(keyConfigRemove);
                        keyConfigsRemove.Add(keyConfigRemove);
                    }
                }
                
                ReUpdateKeyIdInLevel();
                
                Debug.Log("Key Config Count " + keyConfigsRemove.Count);
                if (keyConfigsRemove.Count > 0)
                {
                    foreach (var keyConfig in keyConfigsRemove)
                    {
                        foreach (var cellId in keyConfig.cellsId)
                        {
                            (ulong row, ulong col) = CantorPairing.Unpair((ulong)cellId);
                            int cellIndex = (int)col * currentLevelConfig.mapConfig.mapSize.y
                                            + (int)row;
                            Debug.Log("Cell Index " + cellIndex);
                            _gridCellMapViews[cellIndex].cellConfig.keyId = -1;
                        }
                    }
                }
            }
        }

        private void SetColorSelected()
        {
            view.dragCellMapSelection.OnClickSetColor();
        }

        private void DeleteColorSelected()
        {
            view.dragCellMapSelection.OnClickDeleteColor();   
        }

        private void ClearAllMap()
        {
            view.dragCellMapSelection.ClearAllGrid();
        }

        private void ClearAllColor()
        {
            view.dragCellMapSelection.CLearAllColor();
            foreach (var cell in currentLevelConfig.mapConfig.cells)
            {
                cell.colorType = ColorType.None;
            }
        }

        private void UpdateCurrentColorChoose(ColorType colorChoose)
        {
            foreach (var buttonChoose in view.buttonCellGridColorChooses)
            {
                buttonChoose.UpdateChoosing(buttonChoose.GetColorType() == colorChoose);
            }
            currentColorChoose = colorChoose;
        }

        private void UpdateCurrentDragType(DragType dragType)
        {
            foreach (var buttonChoose in view.buttonChooseDragTypes)
            {
                buttonChoose.UpdateButtonState(dragType);
            }
            currentDragType = dragType;
            view.dragCellMapSelection.ChangeDragType(currentDragType);
        }

        private void ReUpdateBlockId()
        {
            for (int i = 0; i < currentLevelConfig.mapConfig.blocks.Count; i++)
            {
                BlockConfig blockConfig = currentLevelConfig.mapConfig.blocks[i];
                currentLevelConfig.mapConfig.blocks[i].blockGroupId = i;

                foreach (var cellId in blockConfig.cellsId)
                {
                    (ulong row, ulong col) = CantorPairing.Unpair((ulong)cellId);
                    int cellIndex = (int)col * currentLevelConfig.mapConfig.mapSize.y
                                    + (int)row;
                    _gridCellMapViews[cellIndex].cellConfig.blockGroupId = i;
                    currentLevelConfig.mapConfig.cells[cellIndex].blockGroupId = i;
                }
            }
        }

        private void ReUpdateKeyIdInLevel()
        {
            for (int i = 0; i < currentLevelConfig.mapConfig.keys.Count; i++)
            {
                KeyConfig keyConfig = currentLevelConfig.mapConfig.keys[i];
                currentLevelConfig.mapConfig.keys[i].keyId = i;
                
                foreach (var cellId in keyConfig.cellsId)
                {
                    (ulong row, ulong col) = CantorPairing.Unpair((ulong)cellId);
                    int cellIndex = (int)col * currentLevelConfig.mapConfig.mapSize.y
                                    + (int)row;
                    _gridCellMapViews[cellIndex].cellConfig.keyId = i;
                    currentLevelConfig.mapConfig.cells[cellIndex].keyId = i;
                }
            }
        }
        
        private int GetMapSize()
        {
            return mapWidth >= mapHeight ? mapWidth : mapHeight;
        }
    }
}
