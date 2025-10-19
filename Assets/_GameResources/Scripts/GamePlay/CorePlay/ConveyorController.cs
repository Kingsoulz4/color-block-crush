using ColorBlockCrush;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class ConveyorController : MonoBehaviour
{
    [Header("Conveyor Settings")]
    [SerializeField] private int maxSlots = 5;
    [SerializeField] private Transform startPos;
    [SerializeField] private float startMovingGunSpacing = 0.1f;
    [SerializeField] private float trayMoveDuration = 5f;
    [SerializeField] private float trayMoveDurationFast = 3f;
    [SerializeField] private EndPointConveyor endPointConveyor;

    [Header("Tray Spawn Settings")]
    [SerializeField] private TrayItem trayPrefab;
    [SerializeField] private TextMeshPro trayText;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Vector3 startPosition; // Vị trí tray đầu tiên (bên trái)
    [SerializeField] private float spaceOffsetX = 0.3f; // Khoảng cách giữa các tray

    [Header("Animation")]
    [SerializeField] private float shiftDuration = 0.2f;
    [SerializeField] private Ease shiftEase = Ease.OutQuad;

    public SplineContainer splineContainer;

    private List<TrayItem> prepairTrayItems = new List<TrayItem>();
    private List<TrayItem> movingTrayItems;
    private List<TrayItem> trayItemsFree = new List<TrayItem>();

    public Action<Gun> OnStartAddGunToConveyor;
    public Action<Gun> OnGunRemovedConveyor;

    public void Init()
    {
        endPointConveyor.gameObject.SetActive(true);
        movingTrayItems = new List<TrayItem>();
        movingTrayItems.Clear();
        UpdateTrayText();
        InitTray();
        LevelEvent.OnFastMode += OnFastMode;
        LevelEvent.OnRevive += OnRevive;
    }

    private void OnDisable()
    {
        LevelEvent.OnFastMode -= OnFastMode;
        LevelEvent.OnRevive -= OnRevive;
    }

    private void OnFastMode()
    {
        endPointConveyor.gameObject.SetActive(false);
    }

    public void MoveGunIn(List<Gun> guns)
    {
        PrepairTrayItems(guns.Count);

        for (int i = 0; i < guns.Count; i++)
        {
            SetGunStartPosition(guns[i], prepairTrayItems[i], i);
            OnStartAddGunToConveyor?.Invoke(guns[i]);
        }
    }

    public void SetGunStartPosition(Gun gun, TrayItem trayItem, int slotIndex)
    {
        float normalizedTime = slotIndex * startMovingGunSpacing;
        Vector3 position = splineContainer.EvaluatePosition(normalizedTime);

        gun.TrayItem = trayItem;
        AddTrayItem(trayItem);
        UpdateTrayText();
        gun.MoveToConeyor(position, () =>
        {

            gun.OnGunEmpty += OnGunEmpty;
            trayItem.SetChild(gun);
            trayItem.Move();
        });
    }

    public bool CanPlaceGuns(int count)
    {
        return trayItemsFree.Count >= count;
    }

    private void OnGunEmpty(Gun gun)
    {

        if (gun.CheckCanDisappear())
        {
            if (gun.TrayItem != null)
            {
                MoveTrayIn(gun.TrayItem);
                RemoveTrayItem(gun.TrayItem);
            }

            foreach (var g in gun.ConnectedGuns)
            {
                if (g.TrayItem != null)
                {
                    MoveTrayIn(g.TrayItem);
                    RemoveTrayItem(g.TrayItem);
                }
            }
        }
        UpdateTrayText();
    }


    [Button("OnRevive")]
    public void OnRevive(int level)
    {
        for (int i = 0; i < movingTrayItems.Count; i++)
        {
            movingTrayItems[i].MyGun.Scale(Vector3.one * 0.8f, 0.2f);
            LevelController.Instance.BonusSlotController.MoveGunIn(movingTrayItems[i].MyGun);
            MoveTrayIn(movingTrayItems[i]);
        }
    }

    #region Tray Management
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

    public void WarnTrayText()
    {
        trayText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
    }

    public void SetTrayStartPosition(TrayItem tray, int slotIndex)
    {
        float normalizedTime = slotIndex * startMovingGunSpacing;
        Vector3 position = splineContainer.EvaluatePosition(normalizedTime);
        tray.SplineAnimate.Container = splineContainer;
        tray.SplineAnimate.Pause();

        tray.MoveToConeyor(position, () =>
        {
            tray.SplineAnimate.NormalizedTime = normalizedTime;
        });
    }

    public void AddTrayItem(TrayItem trayItem)
    {
        if (!movingTrayItems.Contains(trayItem))
        {
            movingTrayItems.Add(trayItem);
        }
    }

    public void RemoveTrayItem(TrayItem trayItem)
    {
        if (trayItem == null) return;

        if (movingTrayItems != null && movingTrayItems.Contains(trayItem))
        {
            trayItem.SplineAnimate.Pause();
            trayItem.MyGun.OnGunEmpty -= OnGunEmpty;
            movingTrayItems.Remove(trayItem);
            OnGunRemovedConveyor?.Invoke(trayItem.MyGun);
        }
    }

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

    public bool MoveTrayIn(TrayItem tray)
    {
        if (trayItemsFree.Count >= maxSlots)
        {
            Debug.LogWarning("List đã đầy!");
            return false;
        }

        if (!tray)
        {
            Debug.Log("tray null");
            return false;
        }

        tray.transform.SetParent(spawnParent);
        RemoveTrayItem(tray);
        tray.ResetTray(GetTrayPosition(0), () =>
        {
            trayItemsFree.Insert(0, tray);
            ShiftTraysToRight();
        });
        return true;
    }

    private void UpdateTrayText()
    {
        trayText.text = $@"{movingTrayItems.Count}/{maxSlots}";
    }

    public TrayItem SpawnTrayAtLeft()
    {
        if (trayItemsFree.Count >= maxSlots)
        {
            Debug.LogWarning("List đã đầy!");
            return null;
        }

        TrayItem newTray = Instantiate(trayPrefab, spawnParent);
        newTray.Init(trayMoveDuration, trayMoveDurationFast);
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

    public int GetMovingTrayCount()
    {
        return movingTrayItems.Count;
    }
    #endregion
}