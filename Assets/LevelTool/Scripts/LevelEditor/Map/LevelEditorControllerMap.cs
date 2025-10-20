using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

namespace ColorBlockCrush.Tools
{
    public partial class LevelEditorController : MonoBehaviour
    {
        [SerializeField] private int mapWidth = 30;
        [SerializeField] private int mapHeight = 30;

        [SerializeField] private ColorType currentColorChoose = ColorType.Red;

        [SerializeField] private DragType currentDragType = DragType.Normal;

        private Action<List<GridCellMapView>> onUpdateSelection;
        private Action<List<GridCellMapView>> onDeleteSelection;

        [SerializeField] private List<GridCellMapView> _gridCellMapViews = new List<GridCellMapView>();
        [SerializeField] private List<BlockInforEditorView> _blockInforEditorViews = new List<BlockInforEditorView>();
        [SerializeField] private List<KeyInforEditorView> _keyInforEditorViews = new List<KeyInforEditorView>();
        [SerializeField] private List<PixelSnakeInforEditorView> _pixelSnakeInforEditorViews = new List<PixelSnakeInforEditorView>();

        private void ValidateMapWidth(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Invalid map Width");
                return;
            }

            int newMapWidth = int.Parse(value);

            if (newMapWidth <= 0 || newMapWidth > model.maxMapSize || newMapWidth < model.minMapSize)
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

            if (newMapHeight <= 0 || newMapHeight > model.maxMapSize || newMapHeight < model.minMapSize)
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
            currentColorSet = new HashSet<ColorType>();
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
                    BlockConfig cellConfig = new BlockConfig();
                    cellConfig.id = (int)CantorPairing.MakeId((ulong)i, (ulong)j);
                    cellConfig.coordinate = new Vector2Int(i, j);
                    currentLevelConfig.mapConfig.blocks.Add(cellConfig);

                    GridCellMapView gridCellMapView = Instantiate(model.gridCellMapViewPrefab, model.gridContainer)
                        .GetComponent<GridCellMapView>();
                    gridCellMapView.cellConfig = cellConfig;
                    _gridCellMapViews.Add(gridCellMapView);
                }
            }

            view.dragCellMapSelection.enabled = true;
            DOVirtual.DelayedCall(.55f, () => view.dragCellMapSelection.Init(mapWidth, mapHeight, model.gridContainer,
                _gridCellMapViews,
                onUpdateSelection, onDeleteSelection));

            UpdateLevelState();
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
                    BlockConfig cellConfig = new BlockConfig();
                    cellConfig.id = (int)CantorPairing.MakeId((ulong)i, (ulong)j);
                    cellConfig.coordinate = new Vector2Int(i, j);
                    cellConfig.colorType = currentColorArray[i, j];
                    currentLevelConfig.mapConfig.blocks.Add(cellConfig);

                    GridCellMapView gridCellMapView = Instantiate(model.gridCellMapViewPrefab, model.gridContainer)
                        .GetComponent<GridCellMapView>();
                    gridCellMapView.cellConfig = cellConfig;
                    gridCellMapView.UpdateColor(currentColorArray[i, j]);
                    _gridCellMapViews.Add(gridCellMapView);
                }
            }

            view.dragCellMapSelection.enabled = true;
            DOVirtual.DelayedCall(2.5f, () => view.dragCellMapSelection.Init(mapWidth, mapHeight, model.gridContainer,
                _gridCellMapViews,
                onUpdateSelection, onDeleteSelection));

            UpdateLevelState();
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
            currentColorSet = new HashSet<ColorType>();

            for (int i = 0; i < mapConfig.mapSize.x; i++)
            {
                for (int j = 0; j < mapConfig.mapSize.y; j++)
                {
                    Debug.Log($"Gen Cell {i * mapConfig.mapSize.y + j}");
                    GridCellMapView gridCellMapView = Instantiate(model.gridCellMapViewPrefab, model.gridContainer)
                        .GetComponent<GridCellMapView>();
                    int cellIndex = i * mapConfig.mapSize.y + j;
                    gridCellMapView.cellConfig = mapConfig.blocks[cellIndex];
                    gridCellMapView.UpdateColor(mapConfig.blocks[cellIndex].colorType);
                    currentColorSet.Add(mapConfig.blocks[cellIndex].colorType);
                    _gridCellMapViews.Add(gridCellMapView);
                }
            }

            view.dragCellMapSelection.enabled = true;
            DOVirtual.DelayedCall(1f, () =>
            {
                view.dragCellMapSelection.Init(mapWidth, mapHeight, model.gridContainer, _gridCellMapViews,
                    onUpdateSelection, onDeleteSelection);

                List<GridCellMapView> cellGridBlockList = new List<GridCellMapView>();
                for (int i = 0; i < mapConfig.bigBlocks.Count; i++)
                {
                    cellGridBlockList.Clear();
                    cellGridBlockList = _gridCellMapViews
                        .Where(cell => mapConfig.bigBlocks[i].blocksId.Contains(cell.cellConfig.id)).ToList();
                    BlockInforEditorView blockInforView =
                        Instantiate(model.blockInforPrefab).GetComponent<BlockInforEditorView>();
                    blockInforView.UpdateInfor(i, view.mapFeatureParent,
                        cellGridBlockList.ConvertAll(cell => cell.transform as RectTransform),
                        canvas, mapConfig.bigBlocks[i].blockHealth, mapConfig.bigBlocks[i].isHidden,
                        mapConfig.bigBlocks[i].colorType);
                    _blockInforEditorViews.Add(blockInforView);
                    currentColorSet.Add(mapConfig.bigBlocks[i].colorType);
                }

                for (int i = 0; i < mapConfig.keys.Count; i++)
                {
                    cellGridBlockList.Clear();
                    cellGridBlockList = _gridCellMapViews
                        .Where(cell => mapConfig.keys[i].blockId.Contains(cell.cellConfig.id)).ToList();
                    KeyInforEditorView keyInforView =
                        Instantiate(model.keyInforPrefab).GetComponent<KeyInforEditorView>();
                    keyInforView.UpdateInfor(i, view.mapFeatureParent,
                        cellGridBlockList.ConvertAll(cell => cell.transform as RectTransform),
                        canvas);
                    _keyInforEditorViews.Add(keyInforView);
                }

                for (int i = 0; i < mapConfig.tunnelsArea.Count; i++)
                {
                    cellGridBlockList.Clear();
                    cellGridBlockList = _gridCellMapViews
                        .Where(cell => mapConfig.tunnelsArea[i].blocksId.Contains(cell.cellConfig.id)).ToList();
                    TunnelAreaInforEditorView tunnelAreaInforView =
                        Instantiate(model.tunnelAreaInforPrefab).GetComponent<TunnelAreaInforEditorView>();
                    tunnelAreaInforView.UpdateInfor(i, view.mapFeatureParent,
                        cellGridBlockList.ConvertAll(cell => cell.transform as RectTransform),
                        canvas, mapConfig.tunnelsArea[i], SelectTunnelArea);
                    _tunnelAreaInforEditorViews.Add(tunnelAreaInforView);

                    foreach (var tunnelAreaElementConfig in mapConfig.tunnelsArea[i].elements)
                    {
                        currentColorSet.Add(tunnelAreaElementConfig.elementColor);
                    }
                }
                
                for (int i = 0; i < mapConfig.pixelSnakes.Count; i++)
                {
                    cellGridBlockList.Clear();
                    cellGridBlockList = _gridCellMapViews
                        .Where(cell => mapConfig.pixelSnakes[i].blocksId.Contains(cell.cellConfig.id)).ToList();
                    GridCellMapView headCell = _gridCellMapViews.FirstOrDefault(cell =>
                        mapConfig.pixelSnakes[i].headBlockId == cell.cellConfig.id);
                    PixelSnakeInforEditorView pixelSnakeInforView =
                        Instantiate(model.pixelSnakeInforPrefab).GetComponent<PixelSnakeInforEditorView>();
                    pixelSnakeInforView.UpdateInfor(i, view.mapFeatureParent,
                        cellGridBlockList.ConvertAll(cell => cell.transform as RectTransform),
                        headCell.GetComponent<RectTransform>(),
                        canvas, mapConfig.pixelSnakes[i].health,
                        mapConfig.pixelSnakes[i].colorType);
                    _pixelSnakeInforEditorViews.Add(pixelSnakeInforView);
                    currentColorSet.Add(mapConfig.pixelSnakes[i].colorType);
                }

                UpdateLevelState();
            });
        }

        public void SetStateGridCell(List<GridCellMapView> cellSelection)
        {
            if (currentDragType == DragType.Normal)
            {
                foreach (var cellMapView in cellSelection)
                {
                    cellMapView.UpdateColor(currentColorChoose);

                    currentLevelConfig.mapConfig.blocks[cellMapView.Col * currentLevelConfig.mapConfig.mapSize.y
                                                        + cellMapView.Row].colorType = currentColorChoose;
                }

                currentColorSet.Add(currentColorChoose);
            }
            else if (currentDragType == DragType.Block)
            {
                string blockHealth = view.blockHealthInputField.text;
                if (string.IsNullOrEmpty(blockHealth))
                {
                    Debug.LogError("Invalid Block Health");
                    return;
                }

                BigBlockConfig blockConfig = new BigBlockConfig();
                blockConfig.bigBlockId = currentLevelConfig.mapConfig.bigBlocks.Count;
                blockConfig.colorType = currentColorChoose;
                blockConfig.blockHealth = int.Parse(blockHealth);
                blockConfig.isHidden = view.toggleBlockHidden.isOn;

                for (int i = 0; i < cellSelection.Count; i++)
                {
                    cellSelection[i].DeleteColor();

                    int cellIndex = cellSelection[i].Col * currentLevelConfig.mapConfig.mapSize.y
                                    + cellSelection[i].Row;
                    currentLevelConfig.mapConfig.blocks[cellIndex].colorType = ColorType.None;
                    currentLevelConfig.mapConfig.blocks[cellIndex].bigBlockId = blockConfig.bigBlockId;
                    blockConfig.blocksId.Add(currentLevelConfig.mapConfig.blocks[cellIndex].id);
                }

                currentLevelConfig.mapConfig.bigBlocks.Add(blockConfig);

                BlockInforEditorView blockInforView =
                    Instantiate(model.blockInforPrefab).GetComponent<BlockInforEditorView>();
                blockInforView.UpdateInfor(blockConfig.bigBlockId, view.mapFeatureParent,
                    cellSelection.ConvertAll(cell => cell.transform as RectTransform),
                    canvas, int.Parse(blockHealth), blockConfig.isHidden, currentColorChoose);
                _blockInforEditorViews.Add(blockInforView);

                currentColorSet.Add(currentColorChoose);
            }
            else if (currentDragType == DragType.Key)
            {
                KeyConfig keyConfig = new KeyConfig();
                keyConfig.keyId = currentLevelConfig.mapConfig.keys.Count;

                for (int i = 0; i < cellSelection.Count; i++)
                {
                    cellSelection[i].DeleteColor();

                    int cellIndex = cellSelection[i].Col * currentLevelConfig.mapConfig.mapSize.y
                                    + cellSelection[i].Row;
                    currentLevelConfig.mapConfig.blocks[cellIndex].colorType = ColorType.None;
                    currentLevelConfig.mapConfig.blocks[cellIndex].keyId = keyConfig.keyId;
                    keyConfig.blockId.Add(currentLevelConfig.mapConfig.blocks[cellIndex].id);
                }

                currentLevelConfig.mapConfig.keys.Add(keyConfig);

                KeyInforEditorView keyInforView =
                    Instantiate(model.keyInforPrefab).GetComponent<KeyInforEditorView>();
                keyInforView.UpdateInfor(keyConfig.keyId, view.mapFeatureParent,
                    cellSelection.ConvertAll(cell => cell.transform as RectTransform),
                    canvas);
                _keyInforEditorViews.Add(keyInforView);
            }
            else if(currentDragType == DragType.TunnelArea)
            {
                SetTunnelAreaInfor(cellSelection);
            }
            else if(currentDragType == DragType.PixelSnake)
            {
                string snakeHealth = view.blockHealthInputField.text;
                if (string.IsNullOrEmpty(snakeHealth))
                {
                    Debug.LogError("Invalid Snake Health");
                    return;
                }

                PixelSnakeConfig pixelSnakeConfig = new PixelSnakeConfig();
                pixelSnakeConfig.pixelSnakeId = currentLevelConfig.mapConfig.pixelSnakes.Count;
                pixelSnakeConfig.colorType = currentColorChoose;
                pixelSnakeConfig.health = int.Parse(snakeHealth);
                pixelSnakeConfig.headBlockId = cellSelection[0].cellConfig.id;

                for (int i = 0; i < cellSelection.Count; i++)
                {
                    cellSelection[i].DeleteColor();

                    int cellIndex = cellSelection[i].Col * currentLevelConfig.mapConfig.mapSize.y
                                    + cellSelection[i].Row;
                    currentLevelConfig.mapConfig.blocks[cellIndex].colorType = ColorType.None;
                    currentLevelConfig.mapConfig.blocks[cellIndex].pixelSnakeId = pixelSnakeConfig.pixelSnakeId;
                    pixelSnakeConfig.blocksId.Add(currentLevelConfig.mapConfig.blocks[cellIndex].id);
                }

                currentLevelConfig.mapConfig.pixelSnakes.Add(pixelSnakeConfig);

                PixelSnakeInforEditorView pixelSnakeInforView =
                    Instantiate(model.pixelSnakeInforPrefab).GetComponent<PixelSnakeInforEditorView>();
                pixelSnakeInforView.UpdateInfor(pixelSnakeConfig.pixelSnakeId, view.mapFeatureParent,
                    cellSelection.ConvertAll(cell => cell.transform as RectTransform), 
                    cellSelection[0].GetComponent<RectTransform>(), canvas, 
                    int.Parse(snakeHealth), currentColorChoose);
                _pixelSnakeInforEditorViews.Add(pixelSnakeInforView);

                currentColorSet.Add(currentColorChoose);
            }

            UpdateLevelState();
        }

        public void DeleteStateGridCell(List<GridCellMapView> cellSelection)
        {
            HashSet<ColorType> colorDelete = new HashSet<ColorType>();
            if (currentDragType == DragType.Normal)
            {
                foreach (var cellMapView in cellSelection)
                {
                    colorDelete.Add(cellMapView.cellConfig.colorType);
                    cellMapView.DeleteColor();
                    currentLevelConfig.mapConfig.blocks[cellMapView.Col * currentLevelConfig.mapConfig.mapSize.y
                                                        + cellMapView.Row].colorType = ColorType.None;
                }
            }
            else if (currentDragType == DragType.Block)
            {
                HashSet<int> blocksRemoved = new HashSet<int>();
                List<BigBlockConfig> blockConfigsRemove = new List<BigBlockConfig>();

                foreach (var cellMapView in cellSelection)
                {
                    currentLevelConfig.mapConfig.blocks[cellMapView.Col * currentLevelConfig.mapConfig.mapSize.y
                                                        + cellMapView.Row].colorType = ColorType.None;
                    if (cellMapView.cellConfig.bigBlockId != -1
                        && !blocksRemoved.Contains(cellMapView.cellConfig.bigBlockId))
                    {
                        colorDelete.Add(currentLevelConfig.mapConfig.bigBlocks[cellMapView.cellConfig.bigBlockId]
                            .colorType);
                        blocksRemoved.Add(cellMapView.cellConfig.bigBlockId);
                        BlockInforEditorView blockInforViewRemove =
                            _blockInforEditorViews.FirstOrDefault(blocksRemove =>
                                blocksRemove.GetBlockId() == cellMapView.cellConfig.bigBlockId);
                        if (blockInforViewRemove != null)
                        {
                            _blockInforEditorViews.Remove(blockInforViewRemove);
                            Destroy(blockInforViewRemove.gameObject);
                            Debug.Log($"Remove Block {cellMapView.cellConfig.bigBlockId}");
                        }

                        BigBlockConfig blockConfigRenove =
                            currentLevelConfig.mapConfig.bigBlocks[cellMapView.cellConfig.bigBlockId];
                        currentLevelConfig.mapConfig.bigBlocks.Remove(blockConfigRenove);
                        blockConfigsRemove.Add(blockConfigRenove);
                    }
                }

                ReUpdateBlockId();

                Debug.Log("Block Config Count " + blocksRemoved.Count);
                if (blockConfigsRemove.Count > 0)
                {
                    foreach (var blockConfig in blockConfigsRemove)
                    {
                        foreach (var cellId in blockConfig.blocksId)
                        {
                            (ulong col, ulong row) = CantorPairing.Unpair((ulong)cellId);
                            int cellIndex = (int)col * currentLevelConfig.mapConfig.mapSize.y
                                            + (int)row;
                            Debug.Log("Cell Index " + cellIndex);
                            _gridCellMapViews[cellIndex].cellConfig.bigBlockId = -1;
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
                        foreach (var cellId in keyConfig.blockId)
                        {
                            (ulong col, ulong row) = CantorPairing.Unpair((ulong)cellId);
                            int cellIndex = (int)col * currentLevelConfig.mapConfig.mapSize.y
                                            + (int)row;

                            _gridCellMapViews[cellIndex].cellConfig.keyId = -1;
                        }
                    }
                }
            }
            else if (currentDragType == DragType.PixelSnake)
            {
                HashSet<int> pixelRemoved = new HashSet<int>();
                List<PixelSnakeConfig> pixelSnakeConfigsRemove = new List<PixelSnakeConfig>();

                foreach (var cellMapView in cellSelection)
                {
                    currentLevelConfig.mapConfig.blocks[cellMapView.Col * currentLevelConfig.mapConfig.mapSize.y
                                                        + cellMapView.Row].colorType = ColorType.None;
                    if (cellMapView.cellConfig.pixelSnakeId != -1
                        && !pixelRemoved.Contains(cellMapView.cellConfig.pixelSnakeId))
                    {
                        colorDelete.Add(currentLevelConfig.mapConfig.pixelSnakes[cellMapView.cellConfig.pixelSnakeId]
                            .colorType);
                        pixelRemoved.Add(cellMapView.cellConfig.pixelSnakeId);
                        PixelSnakeInforEditorView pixcelSnakeInforViewRemove =
                            _pixelSnakeInforEditorViews.FirstOrDefault(pixelRemoved =>
                                pixelRemoved.GetPixcelSnakeId() == cellMapView.cellConfig.pixelSnakeId);
                        if (pixcelSnakeInforViewRemove != null)
                        {
                            _pixelSnakeInforEditorViews.Remove(pixcelSnakeInforViewRemove);
                            Destroy(pixcelSnakeInforViewRemove.gameObject);
                            Debug.Log($"Remove Pixel Snake {cellMapView.cellConfig.pixelSnakeId}");
                        }

                        PixelSnakeConfig pixelSnakeConfigRenove =
                            currentLevelConfig.mapConfig.pixelSnakes[cellMapView.cellConfig.pixelSnakeId];
                        currentLevelConfig.mapConfig.pixelSnakes.Remove(pixelSnakeConfigRenove);
                        pixelSnakeConfigsRemove.Add(pixelSnakeConfigRenove);
                    }
                }

                ReUpdatePixcelSnakeIdInLevel();

                Debug.Log("Pixel Snake Config Count " + pixelRemoved.Count);
                if (pixelSnakeConfigsRemove.Count > 0)
                {
                    foreach (var pixcelSnakeConfig in pixelSnakeConfigsRemove)
                    {
                        foreach (var cellId in pixcelSnakeConfig.blocksId)
                        {
                            (ulong col, ulong row) = CantorPairing.Unpair((ulong)cellId);
                            int cellIndex = (int)col * currentLevelConfig.mapConfig.mapSize.y
                                            + (int)row;
                            Debug.Log($"Cell Index {cellIndex} {cellId} {col} {row}");
                            _gridCellMapViews[cellIndex].cellConfig.pixelSnakeId = -1;
                        }
                    }
                }
            }

            if (colorDelete.Count > 0)
            {
                foreach (var colorType in colorDelete)
                {
                    Debug.Log("Color Type Delete " + colorType + " " + GetBloclColorNumber(colorType));
                    if (GetBloclColorNumber(colorType) <= 0)
                        if (currentColorSet.Contains(colorType))
                            currentColorSet.Remove(colorType);
                }
            }

            UpdateLevelState();
        }

        private void SetColorSelected()
        {
            view.dragCellMapSelection.OnClickSetColor();
        }

        private void DeleteColorSelected()
        {
            view.dragCellMapSelection.OnClickDeleteColor();
        }

        #region Tunnel

        public List<ItemTunnelAreaQueueView> listIconTunnelAreaQueue = new List<ItemTunnelAreaQueueView>();
        [SerializeField] private List<TunnelAreaInforEditorView> _tunnelAreaInforEditorViews = new List<TunnelAreaInforEditorView>();
        [SerializeField] private ColorType currentTunnelAreaQueueColor = ColorType.Red;
        [SerializeField] private TunnelAreaInforEditorView currentTunnelAreaSelected;

        private void UpdateCurrentColorChooseTunnelAreaQueue(ColorType colorChoose)
        {
            foreach (var buttonChoose in view.buttonTankTunnelAreaQueueColorChooses)
            {
                buttonChoose.UpdateChoosing(buttonChoose.GetColorType() == colorChoose);
            }

            currentTunnelAreaQueueColor = colorChoose;
        }

        private void OnAddColorQueueToTunnelArea(TunnelAreaElementConfig elementConfig)
        {
            //currentTunnelAreaSelected.tunnelAreaConfig.elements.Add(elementConfig);

            ItemTunnelAreaQueueView newItemTunnelAreaQueue =
                Instantiate(model.itemTunnelAreaQueuePrefab).GetComponent<ItemTunnelAreaQueueView>();
            newItemTunnelAreaQueue.GetComponent<DraggableTunnelItem>().dragCanvas = canvas;
            Action<int, int> onChangeIndex = (oldItemIndex, newItemIndex) =>
            {
                ChangeIndexItemTunnelArea(oldItemIndex, newItemIndex, newItemTunnelAreaQueue);
            };
            Action<int> onDelete = (itemIndex) => { RemoveItemFromTunnelArea(itemIndex, newItemTunnelAreaQueue); };
            newItemTunnelAreaQueue.rectTransform.SetParent(view.tunnelAreaElementQueueParent, false);
            newItemTunnelAreaQueue.IconColor.color = ColorReference.Instance.GetColor(elementConfig.elementColor);
            newItemTunnelAreaQueue.elementConfig = elementConfig;
            newItemTunnelAreaQueue.healthNumber.text = elementConfig.health.ToString();
            newItemTunnelAreaQueue.Init(onDelete, onChangeIndex);
            listIconTunnelAreaQueue.Add(newItemTunnelAreaQueue);
            view.tankTunnelAreaQueueNumber.text = listIconTunnelAreaQueue.Count.ToString();
            UpdateTankLinesInfor();
            UpdateLevelState();
        }

        private void ChangeIndexItemTunnelArea(int oldItemIndex, int newItemIndex, ItemTunnelAreaQueueView itemTunnel)
        {
            listIconTunnelAreaQueue.Remove(itemTunnel);
            listIconTunnelAreaQueue.Insert(newItemIndex, itemTunnel);
            
            if (currentTunnelAreaSelected != null)
            {
                currentTunnelAreaSelected.tunnelAreaConfig.elements.RemoveAt(oldItemIndex);
                currentTunnelAreaSelected.tunnelAreaConfig.elements.Insert(newItemIndex, itemTunnel.elementConfig);
            }
            Debug.Log($"Change Item Tunnel Area {itemTunnel} from {oldItemIndex} to {newItemIndex}");
        }

        private void RemoveItemFromTunnelArea(int itemIndex, ItemTunnelAreaQueueView itemTunnel)
        {
            listIconTunnelAreaQueue.Remove(itemTunnel);
            //currentTunnelAreaSelected.tunnelAreaConfig.elements.RemoveAt(itemIndex);
            Destroy(itemTunnel.gameObject);
            view.tankTunnelAreaQueueNumber.text = listIconTunnelAreaQueue.Count.ToString();
        }

        private void OnClearTunnelAreaQueue()
        {
            if (currentTunnelAreaSelected == null)
            {
                Debug.Log("OnSetTunnelArea - SELECT A TUNNEL AREA FIRST");
                return;
            }

            ClearTunnelAreaView();
            currentTunnelAreaSelected.tunnelAreaConfig.elements.Clear();
            view.tankTunnelAreaQueueNumber.text = listIconTunnelAreaQueue.Count.ToString();
            UpdateLevelState();
        }

        private void ClearTunnelAreaView()
        {
            for (int i = listIconTunnelAreaQueue.Count - 1; i >= 0; --i)
            {
                Destroy(listIconTunnelAreaQueue[i].gameObject);
            }

            listIconTunnelAreaQueue.Clear();
            view.tankTunnelAreaQueueNumber.text = "0";
        }

        private void SelectTunnelArea(TunnelAreaInforEditorView tunnelAreaSelect)
        {
            if(currentDragType != DragType.TunnelArea) return;

            currentTunnelAreaSelected = tunnelAreaSelect;
            
            UpdateTunnelAreaQueue();
        }

        private void UpdateTunnelAreaQueue()
        {
            for (int i = listIconTunnelAreaQueue.Count - 1; i >= 0; --i)
            {
                Destroy(listIconTunnelAreaQueue[i].gameObject);
            }

            listIconTunnelAreaQueue.Clear();

            foreach (var elementConfig in currentTunnelAreaSelected.tunnelAreaConfig.elements)
            {
                ItemTunnelAreaQueueView newItemTunnelAreaQueue =
                    Instantiate(model.itemTunnelAreaQueuePrefab).GetComponent<ItemTunnelAreaQueueView>();
                newItemTunnelAreaQueue.GetComponent<DraggableTunnelItem>().dragCanvas = canvas;
                Action<int, int> onChangeIndex = (oldItemIndex, newItemIndex) =>
                {
                    ChangeIndexItemTunnelArea(oldItemIndex, newItemIndex, newItemTunnelAreaQueue);
                };
                Action<int> onDelete = (itemIndex) => { RemoveItemFromTunnelArea(itemIndex, newItemTunnelAreaQueue); };
                newItemTunnelAreaQueue.rectTransform.SetParent(view.tunnelAreaElementQueueParent, false);
                newItemTunnelAreaQueue.elementConfig = elementConfig;
                newItemTunnelAreaQueue.IconColor.color = ColorReference.Instance.GetColor(elementConfig.elementColor);
                newItemTunnelAreaQueue.healthNumber.text = elementConfig.health.ToString();
                newItemTunnelAreaQueue.Init(onDelete, onChangeIndex);
                listIconTunnelAreaQueue.Add(newItemTunnelAreaQueue);
                currentColorSet.Add(elementConfig.elementColor);
            }

            view.tankTunnelAreaQueueNumber.text =
                currentTunnelAreaSelected.tunnelAreaConfig.colorNumber.ToString();
            
            UpdateLevelState();
        }

        private void SetTunnelAreaInfor(List<GridCellMapView> cellSelection)
        {
            if(listIconTunnelAreaQueue == null || listIconTunnelAreaQueue.Count == 0) return;
            
            if (currentTunnelAreaSelected != null)
            {
                cellSelection = _gridCellMapViews.Where(cell =>
                    cell.cellConfig.tunnelAreaId == currentTunnelAreaSelected.GetTunnelAreaId()).ToList();
                TunnelAreaInforEditorView tunnelAreaDell = currentTunnelAreaSelected;
                DellTunnelAreaInfor(tunnelAreaDell);
            }
            
            TunnelAreaConfig tunnelAreaConfig = new TunnelAreaConfig();
            tunnelAreaConfig.tunnelAreaId = currentLevelConfig.mapConfig.tunnelsArea.Count;
            tunnelAreaConfig.colorNumber = listIconTunnelAreaQueue.Count;

            for (int i = 0; i < listIconTunnelAreaQueue.Count; i++)
            {
                tunnelAreaConfig.elements.Add(listIconTunnelAreaQueue[i].elementConfig);
                currentColorSet.Add(listIconTunnelAreaQueue[i].elementConfig.elementColor);
            }

            for (int i = 0; i < cellSelection.Count; i++)
            {
                cellSelection[i].DeleteColor();

                int cellIndex = cellSelection[i].Col * currentLevelConfig.mapConfig.mapSize.y
                                + cellSelection[i].Row;
                currentLevelConfig.mapConfig.blocks[cellIndex].colorType = ColorType.None;
                currentLevelConfig.mapConfig.blocks[cellIndex].tunnelAreaId = tunnelAreaConfig.tunnelAreaId;
                tunnelAreaConfig.blocksId.Add(currentLevelConfig.mapConfig.blocks[cellIndex].id);
            }

            currentLevelConfig.mapConfig.tunnelsArea.Add(tunnelAreaConfig);

            TunnelAreaInforEditorView tunnelAreaInforView =
                Instantiate(model.tunnelAreaInforPrefab).GetComponent<TunnelAreaInforEditorView>();
            tunnelAreaInforView.UpdateInfor(tunnelAreaConfig.tunnelAreaId, view.mapFeatureParent,
                cellSelection.ConvertAll(cell => cell.transform as RectTransform),
                canvas, tunnelAreaConfig, SelectTunnelArea);
            _tunnelAreaInforEditorViews.Add(tunnelAreaInforView);

            ClearTunnelAreaView();
        }

        private void DellTunnelAreaInfor(TunnelAreaInforEditorView tunnelAreaDel = null)
        {
            if (currentDragType == DragType.TunnelArea)
            {
                if (tunnelAreaDel == null)
                {
                    tunnelAreaDel = currentTunnelAreaSelected;
                    ClearTunnelAreaView();
                }
                if (tunnelAreaDel != null)
                {
                    HashSet<ColorType> colorDelete = new HashSet<ColorType>();
                    List<GridCellMapView> cellHasTunnelArea = _gridCellMapViews.Where(cell =>
                        cell.cellConfig.tunnelAreaId == tunnelAreaDel.GetTunnelAreaId()).ToList();

                    foreach (var cellMapView in cellHasTunnelArea)
                    {
                        currentLevelConfig.mapConfig.blocks[cellMapView.Col * currentLevelConfig.mapConfig.mapSize.y
                                                            + cellMapView.Row].colorType = ColorType.None;
                        currentLevelConfig.mapConfig.blocks[cellMapView.Col * currentLevelConfig.mapConfig.mapSize.y
                                                            + cellMapView.Row].tunnelAreaId = -1;
                        cellMapView.cellConfig.tunnelAreaId = -1;
                    }

                    foreach (var tunnelAreaElementConfig in tunnelAreaDel.tunnelAreaConfig.elements)
                    {
                        colorDelete.Add(tunnelAreaElementConfig.elementColor);
                    }

                    TunnelAreaConfig tunnelAreaConfigRenove =
                        currentLevelConfig.mapConfig.tunnelsArea[tunnelAreaDel.GetTunnelAreaId()];
                    currentLevelConfig.mapConfig.tunnelsArea.Remove(tunnelAreaConfigRenove);

                    _tunnelAreaInforEditorViews.Remove(tunnelAreaDel);
                    Destroy(tunnelAreaDel.gameObject);
                    Debug.Log($"Remove Tunnel Area {tunnelAreaDel.GetTunnelAreaId()}");
                    tunnelAreaDel = null;
                    ReUpdateTunnelAreaId();

                    if (colorDelete.Count > 0)
                    {
                        foreach (var colorType in colorDelete)
                        {
                            Debug.Log("Color Type Delete " + colorType + " " + GetBloclColorNumber(colorType));
                            if (GetBloclColorNumber(colorType) <= 0)
                                if (currentColorSet.Contains(colorType))
                                    currentColorSet.Remove(colorType);
                        }
                    }
                    UpdateLevelState();
                }
            }
        }

        private void SetTunnelArea()
        {
            view.dragCellMapSelection.OnClickSetColor();
        }

        private void DeleteTunnelArea()
        {
            DellTunnelAreaInfor();
        }
        
        private void ClearAllTunnelArea()
        {
            for (int i = _tunnelAreaInforEditorViews.Count - 1; i >= 0; i--)
            {
                TunnelAreaInforEditorView tunnelAreaEditorView = _tunnelAreaInforEditorViews[i];
                _tunnelAreaInforEditorViews.RemoveAt(i);

                Destroy(tunnelAreaEditorView.gameObject);
            }

            _tunnelAreaInforEditorViews.Clear();
        }
        
        private void ReUpdateTunnelAreaId()
        {
            for (int i = 0; i < currentLevelConfig.mapConfig.tunnelsArea.Count; i++)
            {
                TunnelAreaConfig tunnelAreaConfig = currentLevelConfig.mapConfig.tunnelsArea[i];
                currentLevelConfig.mapConfig.tunnelsArea[i].tunnelAreaId = i;

                foreach (var cellId in tunnelAreaConfig.blocksId)
                {
                    (ulong col, ulong row) = CantorPairing.Unpair((ulong)cellId);
                    int cellIndex = (int)col * currentLevelConfig.mapConfig.mapSize.y
                                    + (int)row;
                    _gridCellMapViews[cellIndex].cellConfig.tunnelAreaId = i;
                    currentLevelConfig.mapConfig.blocks[cellIndex].tunnelAreaId = i;
                }
            }
        }

        #endregion

        private void ButtonClearSelect()
        {
            if (currentDragType == DragType.TunnelArea)
            {
                if (currentTunnelAreaSelected != null)
                {
                    currentTunnelAreaSelected = null;
                    ClearTunnelAreaView();
                }
            }
        }

        [Button]
        private void ClearAllMap()
        {
            view.dragCellMapSelection.ClearAllGrid();
            ClearAllBlock();
            ClearAllKey();
            ClearAllTunnelArea();
            ButtonClearSelect();
            ClearTunnelAreaView();
            ClearAllPixelSnake();
        }

        private void ClearAllBlock()
        {
            for (int i = _blockInforEditorViews.Count - 1; i >= 0; i--)
            {
                BlockInforEditorView blockEditorView = _blockInforEditorViews[i];
                _blockInforEditorViews.RemoveAt(i);

                Destroy(blockEditorView.gameObject);
            }

            _blockInforEditorViews.Clear();
        }

        private void ClearAllKey()
        {
            for (int i = _keyInforEditorViews.Count - 1; i >= 0; i--)
            {
                KeyInforEditorView keyInforEditorView = _keyInforEditorViews[i];
                _keyInforEditorViews.RemoveAt(i);

                Destroy(keyInforEditorView.gameObject);
            }

            _keyInforEditorViews.Clear();
        }

        private void ClearAllPixelSnake()
        {
            for (int i = _pixelSnakeInforEditorViews.Count - 1; i >= 0; i--)
            {
                PixelSnakeInforEditorView pixcelSnakeEditorView = _pixelSnakeInforEditorViews[i];
                _pixelSnakeInforEditorViews.RemoveAt(i);

                Destroy(pixcelSnakeEditorView.gameObject);
            }

            _pixelSnakeInforEditorViews.Clear();
        }

        private void ClearAllColor()
        {
            view.dragCellMapSelection.CLearAllColor();
            foreach (var cell in currentLevelConfig.mapConfig.blocks)
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
            
            view.blockBulletValidatePanel.SetActive(dragType != DragType.TunnelArea);
            view.tunnelAreaPanel.SetActive(dragType ==  DragType.TunnelArea);
        }

        private void ReUpdateBlockId()
        {
            for (int i = 0; i < currentLevelConfig.mapConfig.bigBlocks.Count; i++)
            {
                BigBlockConfig blockConfig = currentLevelConfig.mapConfig.bigBlocks[i];
                currentLevelConfig.mapConfig.bigBlocks[i].bigBlockId = i;

                foreach (var cellId in blockConfig.blocksId)
                {
                    (ulong col, ulong row) = CantorPairing.Unpair((ulong)cellId);
                    int cellIndex = (int)col * currentLevelConfig.mapConfig.mapSize.y
                                    + (int)row;
                    _gridCellMapViews[cellIndex].cellConfig.bigBlockId = i;
                    currentLevelConfig.mapConfig.blocks[cellIndex].bigBlockId = i;
                }
            }
        }

        private void ReUpdateKeyIdInLevel()
        {
            for (int i = 0; i < currentLevelConfig.mapConfig.keys.Count; i++)
            {
                KeyConfig keyConfig = currentLevelConfig.mapConfig.keys[i];
                currentLevelConfig.mapConfig.keys[i].keyId = i;

                foreach (var cellId in keyConfig.blockId)
                {
                    (ulong col, ulong row) = CantorPairing.Unpair((ulong)cellId);
                    int cellIndex = (int)col * currentLevelConfig.mapConfig.mapSize.y
                                    + (int)row;
                    _gridCellMapViews[cellIndex].cellConfig.keyId = i;
                    currentLevelConfig.mapConfig.blocks[cellIndex].keyId = i;
                }
            }
        }
        
        private void ReUpdatePixcelSnakeIdInLevel()
        {
            for (int i = 0; i < currentLevelConfig.mapConfig.pixelSnakes.Count; i++)
            {
                PixelSnakeConfig pixeclSnakeConfig = currentLevelConfig.mapConfig.pixelSnakes[i];
                currentLevelConfig.mapConfig.pixelSnakes[i].pixelSnakeId = i;

                foreach (var cellId in pixeclSnakeConfig.blocksId)
                {
                    (ulong col, ulong row) = CantorPairing.Unpair((ulong)cellId);
                    int cellIndex = (int)col * currentLevelConfig.mapConfig.mapSize.y
                                    + (int)row;
                    _gridCellMapViews[cellIndex].cellConfig.pixelSnakeId = i;
                    currentLevelConfig.mapConfig.blocks[cellIndex].pixelSnakeId = i;
                }
            }
        }

        public void UpdateBlockBulletValidate()
        {
            foreach (var blockBulletValidateEditorView in view.blockBulletValidateEditorViews)
            {
                ColorType colorType = blockBulletValidateEditorView.GetColorType();
                if (!currentColorSet.Contains(colorType))
                {
                    blockBulletValidateEditorView.gameObject.SetActive(false);
                }
                else
                {
                    blockBulletValidateEditorView.gameObject.SetActive(true);
                    blockBulletValidateEditorView.ValidateState(GetBloclColorNumber(colorType),
                        GetBulletNumber(colorType));
                }
            }
        }

        public int GetBloclColorNumber(ColorType colorType)
        {
            int number = 0;

            foreach (var cellConfig in currentLevelConfig.mapConfig.blocks)
            {
                if (cellConfig.colorType == colorType)
                    number++;
            }

            foreach (var blockConfig in currentLevelConfig.mapConfig.bigBlocks)
            {
                if (blockConfig.colorType == colorType)
                    number += blockConfig.blockHealth;
            }

            foreach (var tunnelAreaConfig in currentLevelConfig.mapConfig.tunnelsArea)
            {
                foreach (var element in tunnelAreaConfig.elements)
                {
                    if (element.elementColor == colorType)
                    {
                        number += element.health;
                    }
                }
            }

            foreach (var pixelSnakeConfig in currentLevelConfig.mapConfig.pixelSnakes)
            {
                if (pixelSnakeConfig.colorType == colorType)
                    number += pixelSnakeConfig.health;
            }

            return number;
        }

        private int GetMapSize()
        {
            return mapWidth >= mapHeight ? mapWidth : mapHeight;
        }
    }
}