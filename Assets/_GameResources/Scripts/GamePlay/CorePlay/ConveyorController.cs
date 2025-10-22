using ColorBlockCrush;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class ConveyorController : MonoBehaviour
{
    [Header("Conveyor Settings")]
    [SerializeField] private int initMaxSlot = 5;
    [SerializeField] private Transform startPos;
    [SerializeField] private float startMovingGunSpacing = 0.1f;
    [SerializeField] private float trayMoveDuration = 5f;
    [SerializeField] private float trayMoveDurationFast = 3f;
    [SerializeField] private EndPointConveyor endPointConveyor;

    [Header("Tray Spawn Settings")]
    [SerializeField] private Transform startPosBooster;
    [SerializeField] private TrayItem trayPrefab;
    [SerializeField] private TextMeshPro trayText;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private Vector3 startPositionInit = new Vector3(-0.25f, -0.3f, 0);
    [SerializeField] private float spaceOffsetX = 0.3f; // Khoảng cách giữa các tray

    [Header("Animation")]
    [SerializeField] private float shiftDuration = 0.2f;
    [SerializeField] private Ease shiftEase = Ease.OutQuad;

    public SplineContainer splineContainer;
    private Vector3 currentStartPos;
    private int currentMaxSlots;

    private List<TrayItem> prepairTrayItems = new List<TrayItem>();
    private List<TrayItem> movingTrayItems;
    private List<TrayItem> trayItemsFree = new List<TrayItem>();

    public Action<Gun> OnStartAddGunToConveyor;
    public Action<Gun> OnGunRemovedConveyor;

    private float timeDelayMove = 0;

    public void Init()
    {
        currentMaxSlots = initMaxSlot;
        currentStartPos = startPositionInit;
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

    private void Update()
    {
        if (timeDelayMove > 0)
        {
            timeDelayMove -= Time.deltaTime;
        }
    }

    private void OnFastMode()
    {
        endPointConveyor.gameObject.SetActive(false);
    }

    public void MoveGunIn(List<Gun> guns)
    {
        if (timeDelayMove <= 0) timeDelayMove = 0;
        for (int i = 0; i < guns.Count; i++)
        {
            PrepairTrayItems(1);
            SetGunStartPosition(guns[i], prepairTrayItems[0], i, i * startMovingGunSpacing + timeDelayMove);
            OnStartAddGunToConveyor?.Invoke(guns[i]);

            this.Wait(0.12f * i, () =>
            {
                AudioManager.Instance.PlayOneShot(Constant.SFX.CLICK);
            });

        }

        timeDelayMove = (guns.Count + 1) * startMovingGunSpacing;
    }

    public void SetGunStartPosition(Gun gun, TrayItem trayItem, int slotIndex, float delay = 0)
    {
        slotIndex = 0;
        float normalizedTime = slotIndex * startMovingGunSpacing;
        Vector3 position = splineContainer.EvaluatePosition(normalizedTime);

        gun.TrayItem = trayItem;
        AddTrayItem(trayItem);
        UpdateTrayText();
        gun.OnGunEmpty += OnGunEmpty;
        trayItem.MyGun = gun;

        gun.MoveToConeyor(position, () =>
        {
            trayItem.Move();
            trayItem.SetChild(gun);
        }, delay);
    }

    public bool CanPlaceGuns(int count)
    {
        return trayItemsFree.Count >= count;
    }

    private void OnGunEmpty(Gun gun)
    {
        if (gun.TrayItem != null)
        {
            MoveTrayIn(gun.TrayItem, true);
            RemoveTrayItem(gun.TrayItem);
        }

        UpdateTrayText();
    }


    [Button("OnRevive")]
    public void OnRevive(int level)
    {
        for (int i = movingTrayItems.Count - 1; i >= 0; i--)
        {
            var gun = movingTrayItems[i].MyGun;
            if (gun != null)
            {
                gun.Scale(Vector3.one * 0.8f, 0.2f);
                gun.OnRevive();
                LevelController.Instance.BonusSlotController.MoveGunIn(gun);
            }
            MoveTrayIn(movingTrayItems[i], true);
        }

        Gun lastGunInSlot = LevelController.Instance.SlotController.GetLastGun();
        lastGunInSlot.Scale(Vector3.one * 0.8f, 0.2f);
        LevelController.Instance.BonusSlotController.MoveGunIn(lastGunInSlot);
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

    Tween warnTrayTween;
    public void WarnTrayText()
    {
        if (warnTrayTween != null && warnTrayTween.IsActive())
        {
            warnTrayTween.Kill(true);
        }

        trayText.transform.localScale = Vector3.one * 1.1f;
        warnTrayTween = trayText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
    }

    public void SetTrayStartPosition(TrayItem tray, int slotIndex)
    {
        slotIndex = 0;
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
            if (trayItem.MyGun != null)
            {
                trayItem.MyGun.OnGunEmpty -= OnGunEmpty;
            }
            movingTrayItems.Remove(trayItem);
            OnGunRemovedConveyor?.Invoke(trayItem.MyGun);
        }
        UpdateTrayText();
    }

    public void BoosterAddTrayItem()
    {
        currentMaxSlots += 1;
        currentStartPos.x = -spaceOffsetX * (currentMaxSlots - initMaxSlot + 1);
        SpawnTraBooster();
    }

    private void InitTray()
    {
        for (int i = 0; i < currentMaxSlots; i++)
        {
            SpawnTrayAtLeft();
        }
    }

    private Vector3 GetTrayPosition(int index)
    {
        return currentStartPos + new Vector3(index * spaceOffsetX, 0, 0);
    }

    public void PauseAllTray()
    {
        foreach (var tray in movingTrayItems)
        {
            tray.Pause();
        }
    }

    public bool MoveTrayIn(TrayItem tray, bool forceMove = false)
    {
        if (!LevelController.Instance.SlotController.CanPlaceGuns(1) && !forceMove)
        {
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
        trayText.text = $@"{movingTrayItems.Count}/{currentMaxSlots}";
    }

    public TrayItem SpawnTrayAtLeft()
    {
        if (trayItemsFree.Count >= currentMaxSlots)
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

    public TrayItem SpawnTraBooster()
    {
        if (trayItemsFree.Count >= currentMaxSlots)
        {
            Debug.LogWarning("List đã đầy!");
            return null;
        }

        TrayItem newTray = Instantiate(trayPrefab, spawnParent);
        newTray.transform.position = startPosBooster.position;
        newTray.Init(trayMoveDuration, trayMoveDurationFast);
        trayItemsFree.Insert(0, newTray);
        this.Wait(shiftDuration, () =>
        {
            WarnTrayText();
            UpdateTrayText();
        });
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
        int emptySlots = currentMaxSlots - trayItemsFree.Count;

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