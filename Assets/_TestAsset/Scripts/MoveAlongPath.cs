using DG.Tweening;
using Geckout;
using UnityEngine;

public class MoveAlongPath : MonoBehaviour
{
    public Transform[] waypoints;

    public float duration = 5f;       
    public bool loop = false;         
    public bool lookForward = true;

    void Start()
    {
        waypoints = ConveyorController.Instance.WayPoints;
        Vector3[] path = new Vector3[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
            path[i] = waypoints[i].position;

        var tween = transform.DOPath(path, duration, PathType.Linear, PathMode.Full3D)
                             .SetEase(Ease.Linear).SetId(this);

        if (lookForward)
            tween.SetLookAt(0.01f);

        if (loop)
            tween.SetLoops(-1, LoopType.Restart);
    }
}
