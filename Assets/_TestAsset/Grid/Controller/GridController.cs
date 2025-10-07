using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform _container;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject _gridNodePrefab;

    private Vector3 _gridNodeOffset;

    private GridNode _topRightGridNode;

    private GridNode _bottomLeftGridNode;

    private List<GridNode> _instantiatedGridNodesList;

    public int GridRowCount { get; private set; }

    public int GridColumnCount { get; private set; }

    public GridNode[,] GridNodes { get; private set; }

    private void InitializeVariables()
    {
        _instantiatedGridNodesList = new List<GridNode>();
        _gridNodeOffset = Vector3.zero;
        _topRightGridNode = null;
        _bottomLeftGridNode = null;
    }

    public void PrepareGrid(int gridRowCount, int gridColumnCount, Vector3 spawnPosition, GridAxisTypes axisType, GridAnchorTypes anchorType, Vector3 gridNodeScale, Vector3 gridNodeOffset)
    {
        InitializeVariables();

        GridRowCount = gridRowCount;
        GridColumnCount = gridColumnCount;
        _gridNodeOffset = gridNodeOffset;

        GridNodes = new GridNode[gridRowCount, gridColumnCount];

        Vector3 startPosition = CalculateGridStartPosition(gridRowCount, gridColumnCount, spawnPosition, axisType, anchorType, gridNodeScale, gridNodeOffset);

        CreateAllGridNodes(gridRowCount, gridColumnCount, startPosition, axisType, gridNodeScale, gridNodeOffset);

        FindAllGridNodeAdjacents(gridRowCount, gridColumnCount);

        // Cache boundary nodes
        _bottomLeftGridNode = GridNodes[0, 0];
        _topRightGridNode = GridNodes[gridRowCount - 1, gridColumnCount - 1];
    }

    private Vector3 CalculateGridStartPosition(int gridRowCount, int gridColumnCount, Vector3 spawnPosition, GridAxisTypes axisType, GridAnchorTypes anchorType, Vector3 gridNodeScale, Vector3 gridNodeOffset)
    {
        Vector3 totalGridSize = Vector3.zero;

        // Tính tổng kích thước grid dựa trên axis type
        if (axisType == GridAxisTypes.XY)
        {
            totalGridSize.x = (gridColumnCount - 1) * (gridNodeScale.x + gridNodeOffset.x);
            totalGridSize.y = (gridRowCount - 1) * (gridNodeScale.y + gridNodeOffset.y);
        }
        else // XZ
        {
            totalGridSize.x = (gridColumnCount - 1) * (gridNodeScale.x + gridNodeOffset.x);
            totalGridSize.z = (gridRowCount - 1) * (gridNodeScale.z + gridNodeOffset.z);
        }

        Vector3 offset = Vector3.zero;

        // Tính offset dựa trên anchor type
        switch (anchorType)
        {
            case GridAnchorTypes.BottomLeft:
                offset = Vector3.zero;
                break;
            case GridAnchorTypes.BottomCenter:
                offset = axisType == GridAxisTypes.XY
                    ? new Vector3(-totalGridSize.x / 2f, 0, 0)
                    : new Vector3(-totalGridSize.x / 2f, 0, 0);
                break;
            case GridAnchorTypes.BottomRight:
                offset = axisType == GridAxisTypes.XY
                    ? new Vector3(-totalGridSize.x, 0, 0)
                    : new Vector3(-totalGridSize.x, 0, 0);
                break;
            case GridAnchorTypes.MiddleLeft:
                offset = axisType == GridAxisTypes.XY
                    ? new Vector3(0, -totalGridSize.y / 2f, 0)
                    : new Vector3(0, 0, -totalGridSize.z / 2f);
                break;
            case GridAnchorTypes.MiddleCenter:
                offset = axisType == GridAxisTypes.XY
                    ? new Vector3(-totalGridSize.x / 2f, -totalGridSize.y / 2f, 0)
                    : new Vector3(-totalGridSize.x / 2f, 0, -totalGridSize.z / 2f);
                break;
            case GridAnchorTypes.MiddleRight:
                offset = axisType == GridAxisTypes.XY
                    ? new Vector3(-totalGridSize.x, -totalGridSize.y / 2f, 0)
                    : new Vector3(-totalGridSize.x, 0, -totalGridSize.z / 2f);
                break;
            case GridAnchorTypes.TopLeft:
                offset = axisType == GridAxisTypes.XY
                    ? new Vector3(0, -totalGridSize.y, 0)
                    : new Vector3(0, 0, -totalGridSize.z);
                break;
            case GridAnchorTypes.TopCenter:
                offset = axisType == GridAxisTypes.XY
                    ? new Vector3(-totalGridSize.x / 2f, -totalGridSize.y, 0)
                    : new Vector3(-totalGridSize.x / 2f, 0, -totalGridSize.z);
                break;
            case GridAnchorTypes.TopRight:
                offset = axisType == GridAxisTypes.XY
                    ? new Vector3(-totalGridSize.x, -totalGridSize.y, 0)
                    : new Vector3(-totalGridSize.x, 0, -totalGridSize.z);
                break;
        }

        return spawnPosition + offset;
    }

    public void DestroyGrid()
    {
        DespawnGridNodes();

        GridNodes = null;
        GridRowCount = 0;
        GridColumnCount = 0;
        _topRightGridNode = null;
        _bottomLeftGridNode = null;
    }

    private void CreateAllGridNodes(int gridRowCount, int gridColumnCount, Vector3 startPosition, GridAxisTypes axisType, Vector3 gridNodeScale, Vector3 gridNodeOffset)
    {
        for (int row = 0; row < gridRowCount; row++)
        {
            for (int col = 0; col < gridColumnCount; col++)
            {
                Vector3 nodePosition = startPosition;

                // Tính position dựa trên axis type
                if (axisType == GridAxisTypes.XY)
                {
                    nodePosition.x += col * (gridNodeScale.x + gridNodeOffset.x);
                    nodePosition.y += row * (gridNodeScale.y + gridNodeOffset.y);
                }
                else // XZ
                {
                    nodePosition.x += col * (gridNodeScale.x + gridNodeOffset.x);
                    nodePosition.z += row * (gridNodeScale.z + gridNodeOffset.z);
                }

                // Instantiate node
                GameObject nodeObj = Instantiate(_gridNodePrefab, nodePosition, Quaternion.identity, _container);
                nodeObj.transform.localScale = gridNodeScale;

                GridNode gridNode = nodeObj.GetComponent<GridNode>();
                GridPoint gridPoint = new GridPoint(col, row);

                gridNode.PrepareGridNode(gridPoint, this);

                GridNodes[row, col] = gridNode;
                _instantiatedGridNodesList.Add(gridNode);
            }
        }
    }

    private void FindAllGridNodeAdjacents(int gridRowCount, int gridColumnCount)
    {
        for (int row = 0; row < gridRowCount; row++)
        {
            for (int col = 0; col < gridColumnCount; col++)
            {
                GridNode currentNode = GridNodes[row, col];

                // 4-side adjacents (orthogonal)
                // Top
                if (row < gridRowCount - 1)
                    currentNode.AddFourSideAdjacent(GridNodes[row + 1, col]);

                // Bottom
                if (row > 0)
                    currentNode.AddFourSideAdjacent(GridNodes[row - 1, col]);

                // Right
                if (col < gridColumnCount - 1)
                    currentNode.AddFourSideAdjacent(GridNodes[row, col + 1]);

                // Left
                if (col > 0)
                    currentNode.AddFourSideAdjacent(GridNodes[row, col - 1]);

                // 8-side adjacents (include diagonals)
                // Copy 4-side first
                foreach (var adjacent in currentNode.GetFourSideAdjacents())
                {
                    currentNode.AddEightSideAdjacent(adjacent);
                }

                // Add diagonals
                // Top-Right
                if (row < gridRowCount - 1 && col < gridColumnCount - 1)
                    currentNode.AddEightSideAdjacent(GridNodes[row + 1, col + 1]);

                // Top-Left
                if (row < gridRowCount - 1 && col > 0)
                    currentNode.AddEightSideAdjacent(GridNodes[row + 1, col - 1]);

                // Bottom-Right
                if (row > 0 && col < gridColumnCount - 1)
                    currentNode.AddEightSideAdjacent(GridNodes[row - 1, col + 1]);

                // Bottom-Left
                if (row > 0 && col > 0)
                    currentNode.AddEightSideAdjacent(GridNodes[row - 1, col - 1]);
            }
        }
    }

    private void DespawnGridNodes()
    {
        if (_instantiatedGridNodesList != null)
        {
            foreach (GridNode node in _instantiatedGridNodesList)
            {
                if (node != null)
                {
                    node.DestroyGridNode();
                }
            }
            _instantiatedGridNodesList.Clear();
        }
    }

    public Vector3 GetGridNodeOffset()
    {
        return _gridNodeOffset;
    }

    public GridNode GetGridNodeFromGridPoint(GridPoint gridPoint)
    {
        if (gridPoint.X >= 0 && gridPoint.X < GridColumnCount &&
            gridPoint.Y >= 0 && gridPoint.Y < GridRowCount)
        {
            return GridNodes[gridPoint.Y, gridPoint.X];
        }
        return null;
    }

    public GridNode GetTopRightGridNode()
    {
        return _topRightGridNode;
    }

    public GridNode GetBottomLeftGridNode()
    {
        return _bottomLeftGridNode;
    }
}