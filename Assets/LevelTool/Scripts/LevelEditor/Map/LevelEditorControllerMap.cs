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
            List<GridCellMapView> gridCellList = new List<GridCellMapView>();

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
                    gridCellMapView.id = cellConfig.id;
                    gridCellList.Add(gridCellMapView);
                }
            }
            
            view.dragCellMapSelection.enabled = true;
            DOVirtual.DelayedCall(.55f, () => view.dragCellMapSelection.Init(mapWidth, mapHeight, model.gridContainer, gridCellList,
                onUpdateSelection, onDeleteSelection));
            
        }

        private void CreateMapByPicture()
        {
            InputImageConfig inputImageConfig = currentLevelConfig.imageConfig;
            mapWidth = (int)inputImageConfig.ImageSize.x;
            mapHeight = (int)inputImageConfig.ImageSize.y;

            view.WidthMapSize.text = mapWidth.ToString();
            view.HeightMapSize.text = mapHeight.ToString();
            
            List<GridCellMapView> gridCellList = new List<GridCellMapView>();

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
                    gridCellMapView.id = cellConfig.id;
                    gridCellMapView.UpdateColor(currentColorArray[i, j]);
                    gridCellList.Add(gridCellMapView);
                }
            }

            view.dragCellMapSelection.enabled = true;
            DOVirtual.DelayedCall(2.5f, () => view.dragCellMapSelection.Init(mapWidth, mapHeight, model.gridContainer, gridCellList,
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
            
            List<GridCellMapView> gridCellList = new List<GridCellMapView>();
            
            for (int i = 0; i < mapConfig.mapSize.x; i++)
            {
                for (int j = 0; j < mapConfig.mapSize.y; j++)
                {
                    GridCellMapView gridCellMapView = Instantiate(model.gridCellMapViewPrefab, model.gridContainer)
                        .GetComponent<GridCellMapView>();
                    gridCellMapView.id = mapConfig.cells[i * mapConfig.mapSize.y + j].id;
                    gridCellMapView.UpdateColor(mapConfig.cells[i * mapConfig.mapSize.y + j].colorType);
                    gridCellList.Add(gridCellMapView);
                }
            }
            
            view.dragCellMapSelection.enabled = true;
            DOVirtual.DelayedCall(.55f, () =>
            {
                view.dragCellMapSelection.Init(mapWidth, mapHeight, model.gridContainer, gridCellList,
                    onUpdateSelection, onDeleteSelection);
                
                List<GridCellMapView> cellGridBlockList = new List<GridCellMapView>();
                for (int i = 0; i < mapConfig.blocks.Count; i++)
                {
                    cellGridBlockList.Clear();
                    cellGridBlockList = gridCellList.Where(cell => mapConfig.blocks[i].cellsId.Contains(cell.id)).ToList();
                    BlockInforEditorView blockInforView =
                        Instantiate(model.blockInforPrefab).GetComponent<BlockInforEditorView>();
                    blockInforView.UpdateInfor(i, view.mapFeatureParent, 
                        cellGridBlockList.ConvertAll(cell => cell.transform as RectTransform), 
                        canvas, mapConfig.blocks[i].blockHealth, mapConfig.blocks[i].colorType);
                }

                for (int i = 0; i < mapConfig.keys.Count; i++)
                {
                    cellGridBlockList.Clear();
                    cellGridBlockList = gridCellList.Where(cell => mapConfig.keys[i].cellsId.Contains(cell.id)).ToList();
                    KeyInforEditorView keyInforView =
                        Instantiate(model.keyInforPrefab).GetComponent<KeyInforEditorView>();
                    keyInforView.UpdateInfor(i, view.mapFeatureParent, 
                        cellGridBlockList.ConvertAll(cell => cell.transform as RectTransform), 
                        canvas);
                }
            });
        }
        
        public void UpdateColorGridCell(List<GridCellMapView> cellSelection)
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
                    cellSelection[i].UpdateColor(currentColorChoose);

                    int cellIndex = cellSelection[i].Col * currentLevelConfig.mapConfig.mapSize.y
                                    + cellSelection[i].Row;
                    currentLevelConfig.mapConfig.cells[cellIndex].colorType = currentColorChoose;
                    currentLevelConfig.mapConfig.cells[cellIndex].blockGroupId = blockConfig.blockGroupId;
                    blockConfig.cellsId.Add(currentLevelConfig.mapConfig.cells[cellIndex].id);
                }
                
                currentLevelConfig.mapConfig.blocks.Add(blockConfig);
                
                BlockInforEditorView blockInforView =
                    Instantiate(model.blockInforPrefab).GetComponent<BlockInforEditorView>();
                blockInforView.UpdateInfor(blockConfig.blockGroupId, view.mapFeatureParent, 
                    cellSelection.ConvertAll(cell => cell.transform as RectTransform), 
                    canvas, int.Parse(blockHealth), currentColorChoose);
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
            }
        }
        
        public void DeleteColorGridCell(List<GridCellMapView> cellSelection)
        {
            foreach (var cellMapView in cellSelection)
            {
                cellMapView.DeleteColor();
                currentLevelConfig.mapConfig.cells[cellMapView.Col * currentLevelConfig.mapConfig.mapSize.y
                                                   + cellMapView.Row].colorType = ColorType.None;
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

        private int GetMapSize()
        {
            return mapWidth >= mapHeight ? mapWidth : mapHeight;
        }
    }
}
