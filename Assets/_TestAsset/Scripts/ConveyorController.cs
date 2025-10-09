using ColorBlockCrush;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ConveyorController : MonoBehaviour
{
    [Header("Conveyor Settings")]
    [SerializeField] private int maxSlots = 5;
    [SerializeField] private Transform startPos;
    [SerializeField] private float startMovingGunSpacing = 0.1f;

    [Header("Tray Spawn Settings")]
    [SerializeField] private TrayItem trayPrefab;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Vector3 startPosition; // Vị trí tray đầu tiên (bên trái)
    [SerializeField] private float spaceOffsetX = 0.3f; // Khoảng cách giữa các tray

    [Header("Animation")]
    [SerializeField] private float shiftDuration = 0.3f;
    [SerializeField] private Ease shiftEase = Ease.OutQuad;

    public SplineContainer splineContainer;

    private List<TrayItem> prepairTrayItems = new List<TrayItem>();
    private List<TrayItem> movingTrayItems;
    private List<TrayItem> trayItemsFree = new List<TrayItem>();

    public Action<Gun> OnStartAddGunToConveyor;
    public Action<Gun> OnGunRemovedConveyor;

    public void Init()
    {
        movingTrayItems = new List<TrayItem>();
        movingTrayItems.Clear();
        InitTray();
    }

    public void PushGuns(List<Gun> guns)
    {
        PrepairTrayItems(guns.Count);

        for (int i = 0; i < guns.Count; i++)
        {
            SetGunStartPosition(guns[i], prepairTrayItems[i], i);
            OnStartAddGunToConveyor?.Invoke(guns[i]);
        }
    }

    private void PrepairTrayItems(int trayCount)
    {
        prepairTrayItems.Clear();

        for (int i = 0; i < trayCount; i++)
        {
            var tray = TakeFromRight();

            if (tray == null)
            {
                Debug.LogError("Không đủ tray!");
                break;
            }

            prepairTrayItems.Add(tray);
            SetTrayStartPosition(tray, i);
        }
    }

    public void SetTrayStartPosition(TrayItem tray, int slotIndex)
    {
        float normalizedTime = slotIndex * startMovingGunSpacing;
        Vector3 position = splineContainer.EvaluatePosition(normalizedTime);

        tray.MoveToConeyor(position, () =>
        {
            tray.SplineAnimate.Container = splineContainer;
            tray.SplineAnimate.NormalizedTime = normalizedTime;
        });
    }

    public void SetGunStartPosition(Gun gun, TrayItem trayItem, int slotIndex)
    {
        float normalizedTime = slotIndex * startMovingGunSpacing;
        Vector3 position = splineContainer.EvaluatePosition(normalizedTime);

        gun.TrayItem = trayItem;
        trayItem.SetChild(gun);
        AddTrayItem(trayItem);

        gun.MoveToConeyor(position, () =>
        {
            trayItem.Move();
        });
    }

    public bool CanPlaceGuns(int count)
    {
        return movingTrayItems.Count + count <= maxSlots;
    }

    public void AddTrayItem(TrayItem trayItem)
    {
        if (!movingTrayItems.Contains(trayItem))
        {
            trayItem.MyGun.OnGunEmpty += OnGunEmpty;
            movingTrayItems.Add(trayItem);
        }
    }

    private void OnGunEmpty(Gun gun)
    {
        // Add tray về bên TRÁI (đầu list)
        if (gun.TrayItem != null)
        {
            trayItemsFree.Insert(0, gun.TrayItem);
            gun.TrayItem.transform.DOLocalMove(GetTrayPosition(0), shiftDuration).SetEase(shiftEase);
            ShiftTraysToRight();
        }

        RemoveTrayItem(gun.TrayItem);
        Destroy(gun.gameObject);
    }

    public void RemoveTrayItem(TrayItem trayItem)
    {
        if (movingTrayItems.Contains(trayItem))
        {
            trayItem.MyGun.OnGunEmpty -= OnGunEmpty;
            movingTrayItems.Remove(trayItem);
            OnGunRemovedConveyor?.Invoke(trayItem.MyGun);
        }
    }

    #region Tray Management

    private void InitTray()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            SpawnTrayAtLeft();
        }
    }
    private Vector3 GetTrayPosition(int index)
    {
        return startPosition + new Vector3(index * spaceOffsetX, 0, 0);
    }

    public TrayItem SpawnTrayAtLeft()
    {
        if (trayItemsFree.Count >= maxSlots)
        {
            Debug.LogWarning("List đã đầy!");
            return null;
        }

        TrayItem newTray = Instantiate(trayPrefab, spawnParent);

        trayItemsFree.Insert(0, newTray);

        ShiftTraysToRight();

        return newTray;
    }

    public TrayItem TakeFromRight()
    {
        if (trayItemsFree.Count == 0)
        {
            Debug.LogWarning("List trống!");
            return null;
        }

        int lastIndex = trayItemsFree.Count - 1;
        TrayItem takenTray = trayItemsFree[lastIndex];

        trayItemsFree.RemoveAt(lastIndex);

        ShiftTraysToRight();

        return takenTray;
    }

    private void ShiftTraysToRight()
    {
        int emptySlots = maxSlots - trayItemsFree.Count;

        for (int i = 0; i < trayItemsFree.Count; i++)
        {
            int newIndex = emptySlots + i;
            Vector3 targetPos = GetTrayPosition(newIndex);
            trayItemsFree[i].transform.DOLocalMove(targetPos, shiftDuration).SetEase(shiftEase);
        }
    }
    #endregion
}