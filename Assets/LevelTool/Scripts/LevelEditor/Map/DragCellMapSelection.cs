using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace ColorBlockCrush.Tools
{
    public class DragCellMapSelection : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        [Header("Grid & Camera")] public RectTransform gridParent;
        public Camera uiCamera;

        [Header("Cells")] public List<GridCellMapView> GridCellList;
        public List<GridCellMapView> CurrentlySelectedCells = new List<GridCellMapView>();
        public List<GridCellMapView> FinalSelectedCells = new List<GridCellMapView>();

        [Header("Grid Size")] 
        [SerializeField] private int rows = 32;
        [SerializeField] private int cols = 32;
        
        [Header("Drag Type")]
        [SerializeField] private DragType currentDragType;

        private bool isDragging = false;
        
        private Action<List<GridCellMapView>> onUpdateSelection;
        private Action<List<GridCellMapView>> onDeleteSelection;

        private GridLayoutGroup glg;
        private RectOffset pad;
        private Vector2 cellSize, spacing;
        private float stepX, stepY;
        private Vector2 originLocal;

        [SerializeField] private Vector2 startPos;

        private GridCellMapView[,] grid;
        private Dictionary<GridCellMapView, Vector2Int> cellToRC;

        private bool hasLastPointerRC = false;
        private Vector2Int lastPointerRC;

        [SerializeField] private List<GridCellMapView> selectionOrder = new List<GridCellMapView>();

        private enum SelectionFilter
        {
            None,
            NumberedOnly,
            UnnumberedOnly
        }

        private SelectionFilter filterMode = SelectionFilter.None;

        [SerializeField] private readonly SortedSet<int> freedIndices = new SortedSet<int>();

        private int nextDrawIndexCounter = 0;

        [SerializeField] private GridCellMapView startCell;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClearOnlySelection();
            }
        }

        // ===================== Init =====================
        public void Init(int width, int height, RectTransform gridContainer, List<GridCellMapView> gridCellList,
            Action<List<GridCellMapView>> onUpdateSelection, Action<List<GridCellMapView>> onDeleteSelection)
        {
            rows = height;
            cols = width;
            gridParent = gridContainer;
            GridCellList = gridCellList;
            this.onUpdateSelection = onUpdateSelection;
            this.onDeleteSelection = onDeleteSelection;

            CacheLayoutParams();
            BuildGridIndex();
            RecomputeIndexState();
        }

        public void ChangeDragType(DragType newDragType)
        {
            currentDragType = newDragType;
            
            if (FinalSelectedCells.Count > 0)
            {
                ClearOnlySelection();
            }
        }

        private void CacheLayoutParams()
        {
            glg = gridParent.GetComponent<GridLayoutGroup>();
            if (glg == null) throw new Exception("GridLayoutGroup not found on gridParent.");

            pad = glg.padding;
            cellSize = glg.cellSize;
            spacing = glg.spacing;
            stepX = cellSize.x + spacing.x;
            stepY = cellSize.y + spacing.y;

            // Origin theo corner
            var r = gridParent.rect;
            switch (glg.startCorner)
            {
                case GridLayoutGroup.Corner.LowerLeft:
                    originLocal = new Vector2(r.xMin + pad.left, r.yMin + pad.bottom);
                    break;
                case GridLayoutGroup.Corner.UpperLeft:
                    originLocal = new Vector2(r.xMin + pad.left, r.yMax - pad.top - cellSize.y);
                    break;
                case GridLayoutGroup.Corner.LowerRight:
                    originLocal = new Vector2(r.xMax - pad.right - cellSize.x, r.yMin + pad.bottom);
                    break;
                case GridLayoutGroup.Corner.UpperRight:
                    originLocal = new Vector2(r.xMax - pad.right - cellSize.x, r.yMax - pad.top - cellSize.y);
                    break;
            }
        }

        private void BuildGridIndex()
        {
            grid = new GridCellMapView[rows, cols];
            cellToRC = new Dictionary<GridCellMapView, Vector2Int>(GridCellList.Count);

            for (int i = 0; i < GridCellList.Count; i++)
            {
                int row = i % rows;
                int col = Mathf.CeilToInt(i / rows);
                
                grid[row, col] = GridCellList[i];
                GridCellList[i].SetRowAndCol(col, row);
            }
        }

        public void RecomputeIndexState()
        {
            freedIndices.Clear();
            int maxUsed = -1;
            var used = new HashSet<int>();

            foreach (var cell in GridCellList)
            {
                if (cell.drawIndex >= 0)
                {
                    used.Add(cell.drawIndex);
                    if (cell.drawIndex > maxUsed) maxUsed = cell.drawIndex;
                }
            }

            nextDrawIndexCounter = maxUsed + 1;

            for (int i = 0; i < maxUsed; i++)
                if (!used.Contains(i))
                    freedIndices.Add(i);
        }

        // ===================== Pointer =====================
        public void OnPointerDown(PointerEventData eventData)
        {
            var cam = eventData.pressEventCamera; // Overlay => null
            Debug.Log("Pointer down");

            if (currentDragType == DragType.Normal || currentDragType == DragType.PixelSnake)
            {
                if (!ScreenToRC(eventData.position, cam, out var rcStart))
                {
                    if (FinalSelectedCells.Count > 0)
                    {
                        ClearOnlySelection();
                    }
                    isDragging = false;
                    return;
                }
            
                startCell = GetCell(rcStart.x, rcStart.y);
                if (startCell == null)
                {
                    if (FinalSelectedCells.Count > 0)
                    {
                        ClearOnlySelection();
                    }

                    isDragging = false;
                    return;
                }

                filterMode = (startCell.drawIndex != -1) ? SelectionFilter.NumberedOnly : SelectionFilter.UnnumberedOnly;

                CurrentlySelectedCells.Clear();
                PushIfNew(startCell);

                hasLastPointerRC = true;
                lastPointerRC = rcStart;
                isDragging = true;
            }
            else if (currentDragType == DragType.Block)
            {
                CurrentlySelectedCells.Clear();
                if (FinalSelectedCells.Count > 0)
                {
                    ClearOnlySelection();
                }
                isDragging = true;
                startPos = eventData.position;
            }
            else if(currentDragType == DragType.Key || currentDragType == DragType.TunnelArea)
            {
                CurrentlySelectedCells.Clear();
                if (FinalSelectedCells.Count > 0)
                {
                    ClearOnlySelection();
                }
                isDragging = true;
                startPos = eventData.position;
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (!isDragging) return;

            if (currentDragType == DragType.Normal || currentDragType == DragType.PixelSnake)
            {
                var cam = eventData.pressEventCamera;
                if (!ScreenToRC(eventData.position, cam, out var currRC))
                {
                    hasLastPointerRC = false;
                    return;
                }

                if (!hasLastPointerRC)
                {
                    hasLastPointerRC = true;
                    lastPointerRC = currRC;
                }

                foreach (var step in Supercover(lastPointerRC.x, lastPointerRC.y, currRC.x, currRC.y))
                {
                    var cell = GetCell(step.x, step.y);
                    if (cell == null) continue;
                    if (!AcceptByMode(cell)) continue;

                    int top = CurrentlySelectedCells.Count - 1;
                    if (top >= 0 && cell == CurrentlySelectedCells[top])
                    {
                        continue;
                    }

                    if (top >= 1 && cell == CurrentlySelectedCells[top - 1])
                    {
                        var toRemove = CurrentlySelectedCells[top];
                        toRemove.IsSelecting = false;
                        toRemove.UpdateSelectingColor();
                        CurrentlySelectedCells.RemoveAt(top);
                    }
                    else if (CurrentlySelectedCells.IndexOf(cell) >= 0)
                    {
                        continue;
                    }
                    else
                    {
                        PushIfNew(cell);
                    }
                }

                lastPointerRC = currRC;   
            }
            else if (currentDragType == DragType.Block)
            {
                Vector2 endPos = eventData.position;

                Vector2 localStart, localEnd;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(gridParent, startPos, uiCamera, out localStart);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(gridParent, endPos, uiCamera, out localEnd);
                
                Rect selectionRect = new Rect(
                    Mathf.Min(localStart.x, localEnd.x),
                    Mathf.Min(localStart.y, localEnd.y),
                    Mathf.Abs(localEnd.x - localStart.x),
                    Mathf.Abs(localEnd.y - localStart.y)
                );

                HighlightCellsInRect(selectionRect);
            }else if (currentDragType == DragType.Key
                      || currentDragType == DragType.TunnelArea)
            {
                Vector2 endPos = eventData.position;

                Vector2 localStart, localEnd;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(gridParent, startPos, uiCamera, out localStart);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(gridParent, endPos, uiCamera, out localEnd);
                
                Rect selectionRect = new Rect(
                    Mathf.Min(localStart.x, localEnd.x),
                    Mathf.Min(localStart.y, localEnd.y),
                    Mathf.Abs(localEnd.x - localStart.x),
                    Mathf.Abs(localEnd.y - localStart.y)
                );
                
                HighlightCellsInRect(selectionRect);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging) return;

            isDragging = false;

            if (currentDragType == DragType.Normal || currentDragType == DragType.PixelSnake)
            {
                hasLastPointerRC = false;

                foreach (var cell in CurrentlySelectedCells)
                {
                    bool willBeSelected = !cell.IsSelected;
                    cell.IsSelected = willBeSelected;
                    cell.UpdateSelectedColor();

                    cell.IsSelecting = false;
                    cell.UpdateSelectingColor();
                }
            }
            else if (currentDragType == DragType.Block)
            {
                foreach (var cell in CurrentlySelectedCells)
                {
                    bool willBeSelected = !cell.IsSelected;
                    cell.IsSelected = willBeSelected;
                    cell.UpdateSelectedColor();

                    cell.IsSelecting = false;
                    cell.UpdateSelectingColor();
                }   
            }
            else if(currentDragType == DragType.Key
                    || currentDragType == DragType.TunnelArea)
            {
                foreach (var cell in CurrentlySelectedCells)
                {
                    bool willBeSelected = !cell.IsSelected;
                    cell.IsSelected = willBeSelected;
                    cell.UpdateSelectedColor();

                    cell.IsSelecting = false;
                    cell.UpdateSelectingColor();
                }   
            }

            CurrentlySelectedCells.Clear();
            UpdateSelection();
        }
        
        void HighlightCellsInRect(Rect selectionRect)
        {
            bool hasSelectCell = false;
            foreach (GridCellMapView cell in GridCellList)
            {
                // Get world corners of the cell
                Vector3[] corners = new Vector3[4];
                cell.rectTransform.GetWorldCorners(corners);

                // Convert corners to local space relative to gridParent
                for (int i = 0; i < 4; i++)
                    corners[i] = gridParent.InverseTransformPoint(corners[i]);

                Rect cellRect = new Rect(corners[0], corners[2] - corners[0]);

                bool isOverlapping = selectionRect.Overlaps(cellRect, true);
                bool isCellSelecting = CurrentlySelectedCells.Contains(cell);

                if (isOverlapping)
                {
                    hasSelectCell = true;
                    if (!isCellSelecting)
                    {
                        CurrentlySelectedCells.Add(cell);
                        cell.IsSelecting = true;
                        cell.UpdateSelectingColor();
                    }
                }
                else
                {
                    if (isCellSelecting)
                    {
                        CurrentlySelectedCells.Remove(cell);
                        cell.IsSelecting = false;
                        cell.UpdateSelectingColor();
                    }
                }
            }

            if (!hasSelectCell)
            {
                // ClearSelection();
            }
        }

        // ===================== Buttons =====================

        public void OnClickSetColor()
        {
            onUpdateSelection?.Invoke(FinalSelectedCells);

            foreach (var cellMapView in FinalSelectedCells)
            {
                if (cellMapView.IsSelected)
                {
                    cellMapView.IsSelected = false;
                    cellMapView.UpdateSelectedColor();
                }
            }
            FinalSelectedCells.Clear();
            CurrentlySelectedCells.Clear();
        }
        
        public void OnClickDeleteColor()
        {
            onDeleteSelection?.Invoke(FinalSelectedCells);

            foreach (var cellMapView in FinalSelectedCells)
            {
                if (cellMapView.IsSelected)
                {
                    cellMapView.IsSelected = false;
                    cellMapView.UpdateSelectedColor();
                }
            }
            FinalSelectedCells.Clear();
            CurrentlySelectedCells.Clear();
        }

        [Button]
        public void ClearAllGrid()
        {
            CurrentlySelectedCells.Clear();
            FinalSelectedCells.Clear();

            for (int i = GridCellList.Count - 1; i >= 0; i--)
            {
                GridCellMapView gridCell = GridCellList[i];
                GridCellList.RemoveAt(i);
                
                Destroy(gridCell.gameObject);
            }
            
            GridCellList.Clear();
        }

        public void CLearAllColor()
        {
            CurrentlySelectedCells.Clear();
            FinalSelectedCells.Clear();

            foreach (var gridCellMapView in GridCellList)
            {
                gridCellMapView.DeleteColor();
            }
        }

        // ===================== Utils =====================
        private void UpdateSelection()
        {
            FinalSelectedCells.Clear();
            foreach (var cell in GridCellList)
            {
                if (cell.IsSelected)
                {
                    cell.UpdateSelectedColor();
                    FinalSelectedCells.Add(cell);
                }
            }
            
            FinalSelectedCells.Remove(startCell);
            FinalSelectedCells.Insert(0, startCell);
        }

        private void ClearOnlySelection()
        {
            foreach (var cell in FinalSelectedCells)
            {
                cell.IsSelected = false;
                cell.UpdateSelectedColor();
            }

            FinalSelectedCells.Clear();

            CurrentlySelectedCells.Clear();
            //selectionOrder.Clear();

            hasLastPointerRC = false;
            filterMode = SelectionFilter.None;
        }

        private void PushIfNew(GridCellMapView cell)
        {
            if (!CurrentlySelectedCells.Contains(cell))
            {
                CurrentlySelectedCells.Add(cell);
                cell.IsSelecting = true;
                cell.UpdateSelectingColor();
            }
        }

        private bool AcceptByMode(GridCellMapView cell)
        {
            if (cell == null) return false;

            switch (filterMode)
            {
                case SelectionFilter.NumberedOnly: return cell.drawIndex != -1;
                case SelectionFilter.UnnumberedOnly: return cell.drawIndex == -1;
                default: return false;
            }
        }

        private GridCellMapView GetCell(int row, int col)
        {
            if (row < 0 || col < 0 || row >= rows || col >= cols) return null;
            return grid[row, col];
        }

        private bool ScreenToRC(Vector2 screenPos, Camera cam, out Vector2Int rc)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(gridParent, screenPos, cam, out var local);
            if (LocalToRC(local, out int r, out int c))
            {
                rc = new Vector2Int(r, c);
                return true;
            }

            rc = default;
            return false;
        }

        private bool LocalToRC(Vector2 local, out int row, out int col)
        {
            float dx, dy;
            switch (glg.startCorner)
            {
                case GridLayoutGroup.Corner.LowerLeft:
                    dx = local.x - originLocal.x;
                    dy = local.y - originLocal.y;
                    break;
                case GridLayoutGroup.Corner.UpperLeft:
                    dx = local.x - originLocal.x;
                    dy = (originLocal.y + cellSize.y) - local.y;
                    break;
                case GridLayoutGroup.Corner.LowerRight:
                    dx = (originLocal.x + cellSize.x) - local.x;
                    dy = local.y - originLocal.y;
                    break;
                case GridLayoutGroup.Corner.UpperRight:
                    dx = (originLocal.x + cellSize.x) - local.x;
                    dy = (originLocal.y + cellSize.y) - local.y;
                    break;
                default:
                    dx = dy = 0f; break;
            }

            col = Mathf.FloorToInt(dx / stepX);
            row = Mathf.FloorToInt(dy / stepY);

            float rx = dx - col * stepX;
            float ry = dy - row * stepY;
            if (rx < 0f || ry < 0f || rx > cellSize.x || ry > cellSize.y)
            {
                row = col = -1;
                return false;
            }

            if (row < 0 || col < 0 || row >= rows || col >= cols) return false;
            return true;
        }

        private Rect LocalRectOf(RectTransform rt)
        {
            var corners = new Vector3[4];
            rt.GetWorldCorners(corners); // BL, TL, TR, BR
            for (int i = 0; i < 4; i++)
            {
                corners[i] = gridParent.InverseTransformPoint(corners[i]);
            }
            return new Rect(corners[0], corners[2] - corners[0]);
        }

        private IEnumerable<Vector2Int> Supercover(int r0, int c0, int r1, int c1)
        {
            int x0 = r0, y0 = c0, x1 = r1, y1 = c1;
            int dx = Math.Abs(x1 - x0), dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;
            int x = x0, y = y0;
            yield return new Vector2Int(x, y);

            while (x != x1 || y != y1)
            {
                int e2 = err << 1;
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                    yield return new Vector2Int(x, y);
                }

                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                    yield return new Vector2Int(x, y);
                }
            }
        }
    }

    public enum DragType
    {
        Normal,
        Key,
        Block,
        TunnelArea,
        PixelSnake
    }
}