using System;
using System.Collections.Generic;
using UnityEngine;

public class GridNode : MonoBehaviour
{
    private bool _isDestroyed;
    private MeshRenderer _meshRenderer;

    public int GCost { get; set; }

    public int HCost { get; set; }

    public int ExtraCost { get; set; }

    public int FCost => GCost + HCost + ExtraCost;

    public bool IsBlocked { get; set; }

    public GridController GridController { get; set; }

    public GridPoint GridPoint { get; set; }

    public GridNode ParentGridNode { get; set; }

    public HashSet<GridNode> FourSideAdjacents { get; private set; }

    public HashSet<GridNode> EightSideAdjacents { get; private set; }

    public event Action<GridNode> OnGridNodeInitialized;

    private void InitializeVariables()
    {
        FourSideAdjacents = new HashSet<GridNode>();
        EightSideAdjacents = new HashSet<GridNode>();
        _isDestroyed = false;

        GCost = 0;
        HCost = 0;
        ExtraCost = 0;
        IsBlocked = false;
        ParentGridNode = null;

        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void PrepareGridNode(GridPoint gridPoint, GridController gridController)
    {
        InitializeVariables();

        GridPoint = gridPoint;
        GridController = gridController;

        OnGridNodeInitialized?.Invoke(this);
    }

    public void DestroyGridNode()
    {
        if (_isDestroyed)
            return;

        _isDestroyed = true;

        FourSideAdjacents?.Clear();
        EightSideAdjacents?.Clear();
        OnGridNodeInitialized = null;

        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    public void AddFourSideAdjacent(GridNode adjacentGridNode)
    {
        if (adjacentGridNode != null && !FourSideAdjacents.Contains(adjacentGridNode))
        {
            FourSideAdjacents.Add(adjacentGridNode);
        }
    }

    public void SetFourSideAdjacents(HashSet<GridNode> adjacents)
    {
        FourSideAdjacents = adjacents ?? new HashSet<GridNode>();
    }

    public HashSet<GridNode> GetFourSideAdjacents()
    {
        return FourSideAdjacents;
    }

    public void AddEightSideAdjacent(GridNode adjacentGridNode)
    {
        if (adjacentGridNode != null && !EightSideAdjacents.Contains(adjacentGridNode))
        {
            EightSideAdjacents.Add(adjacentGridNode);
        }
    }

    public void SetEightSideAdjacents(HashSet<GridNode> adjacents)
    {
        EightSideAdjacents = adjacents ?? new HashSet<GridNode>();
    }

    public HashSet<GridNode> GetEightSideAdjacents()
    {
        return EightSideAdjacents;
    }

    public void SetColor(Color c)
    {
        if (_meshRenderer != null && _meshRenderer.material != null)
        {
            _meshRenderer.material.color = c;
        }
    }

    public void ResetGridNodeParameters()
    {
        GCost = 0;
        HCost = 0;
        ExtraCost = 0;
        ParentGridNode = null;
    }
}