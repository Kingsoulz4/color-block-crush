using ColorBlockCrush;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ConveyorController : MonoBehaviour
{
    [SerializeField] private int maxSlots = 5;
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform trayContainer;
    [SerializeField] private List<TrayItem> trayItems;
    private List<TrayItem> prepairTrayItems = new List<TrayItem>();
    private List<Gun> movingGuns;
    private Queue<TrayItem> trayItemsFree;

    public Action<Gun> OnStartAddGunToConveyor;
    public Action<Gun> OnGunRemovedConveyor;
    public SplineContainer splineContainer;
    public List<Gun> Guns { get => movingGuns; }

    public void Init()
    {
        movingGuns = new List<Gun>();
        movingGuns.Clear();
        trayItemsFree = new Queue<TrayItem>(trayItems);

    }

    public void PushGuns(List<Gun> guns)
    {
        PrepairTrayItems(guns.Count);

        for (int i = 0; i < guns.Count; i++)
        {
            AddGun(guns[i]);
            SetGunStartPosition(guns[i], prepairTrayItems[i], i);
            OnStartAddGunToConveyor?.Invoke(guns[i]);
        }
    }

    private void PrepairTrayItems(int trayCount)
    {
        prepairTrayItems.Clear();

        for (int i = 0; i < trayCount; i++)
        {
            var tray = trayItemsFree.Dequeue();
            prepairTrayItems.Add(tray);
            SetTrayStartPosition(tray, i);
        }
    }

    public void SetTrayStartPosition(TrayItem tray, int slotIndex, float spacing = 0.01f)
    {
        float normalizedTime = slotIndex * spacing;
        Vector3 position = splineContainer.EvaluatePosition(normalizedTime);

        tray.MoveToConeyor(position, () =>
        {
            tray.SplineAnimate.Container = splineContainer;
            tray.SplineAnimate.NormalizedTime = normalizedTime;
        });
    }

    public void SetGunStartPosition(Gun gun, TrayItem trayItem, int slotIndex, float spacing = 0.01f)
    {
        float normalizedTime = slotIndex * spacing;
        Vector3 position = splineContainer.EvaluatePosition(normalizedTime);

        gun.TrayItem = trayItem;
        trayItem.SetChild(gun);

        gun.MoveToConeyor(position, () =>
        {
            trayItem.Move();
        });
    }


    public bool CanPlaceGuns(int count)
    {
        return movingGuns.Count + count <= maxSlots;
    }

    public void AddGun(Gun gun)
    {
        if (!movingGuns.Contains(gun))
        {
            gun.OnGunEmpty += OnGunEmpty;
            movingGuns.Add(gun);
        }
    }
    private void OnGunEmpty(Gun gun)
    {
        trayItemsFree.Enqueue(gun.TrayItem);
        RemoveGun(gun);
        Destroy(gun.gameObject);
    }

    public void RemoveGun(Gun gun)
    {
        if (movingGuns.Contains(gun))
        {
            gun.OnGunEmpty -= OnGunEmpty;
            movingGuns.Remove(gun);
            OnGunRemovedConveyor?.Invoke(gun);
        }
    }
}
