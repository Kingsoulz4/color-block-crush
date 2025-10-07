using System.Collections.Generic;
using UnityEngine;

public class GridPath
{
    public List<GridPoint> PathPoints { get; private set; }

    public List<Vector3> PathPositions { get; private set; }

    public List<Vector3> TurnPositions { get; private set; }

    public GridPath()
    {
        PathPoints = new List<GridPoint>();
        PathPositions = new List<Vector3>();
        TurnPositions = new List<Vector3>();
    }

    public GridPath(List<GridPoint> pathPoints, List<Vector3> pathPositions, List<Vector3> turnPositions)
    {
        PathPoints = pathPoints ?? new List<GridPoint>();
        PathPositions = pathPositions ?? new List<Vector3>();
        TurnPositions = turnPositions ?? new List<Vector3>();
    }
}