using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

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
                    GridCellMapView gridCellMapView = Instantiate(model.gridCellMapViewPrefab, model.gridContainer)
                        .GetComponent<GridCellMapView>();
                    gridCellList.Add(gridCellMapView);
                    
                    CellConfig cellConfig = new CellConfig();
                    cellConfig.id = (int)CantorPairing.MakeId((ulong)i, (ulong)j);
                    cellConfig.coordinate = new Vector2Int(i, j);
                    currentLevelConfig.mapConfig.cells.Add(cellConfig);
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
                    GridCellMapView gridCellMapView = Instantiate(model.gridCellMapViewPrefab, model.gridContainer)
                        .GetComponent<GridCellMapView>();
                    gridCellMapView.UpdateColor(currentColorArray[i, j]);
                    gridCellList.Add(gridCellMapView);
                    
                    CellConfig cellConfig = new CellConfig();
                    cellConfig.id = (int)CantorPairing.MakeId((ulong)i, (ulong)j);
                    cellConfig.coordinate = new Vector2Int(i, j);
                    cellConfig.colorType = currentColorArray[i, j];
                    currentLevelConfig.mapConfig.cells.Add(cellConfig);
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
                    gridCellMapView.UpdateColor(mapConfig.cells[i * mapConfig.mapSize.y + j].colorType);
                    gridCellList.Add(gridCellMapView);
                }
            }
            
            view.dragCellMapSelection.enabled = true;
            DOVirtual.DelayedCall(.55f, () => view.dragCellMapSelection.Init(mapWidth, mapHeight, model.gridContainer, gridCellList,
                onUpdateSelection, onDeleteSelection));
        }
        
        public void UpdateColorGridCell(List<GridCellMapView> cellSelection)
        {
            foreach (var cellMapView in cellSelection)
            {
                cellMapView.UpdateColor(currentColorChoose);
                
                currentLevelConfig.mapConfig.cells[cellMapView.Col * currentLevelConfig.mapConfig.mapSize.y
                + cellMapView.Row].colorType = currentColorChoose;
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
                buttonChoose.UpdateButtonState(currentDragType);
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
