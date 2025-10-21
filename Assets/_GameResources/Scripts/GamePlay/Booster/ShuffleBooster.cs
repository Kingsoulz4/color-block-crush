using DG.Tweening;
using ColorBlockCrush;
using System.Collections;
using UnityEngine;
using System;

public class ShuffleBooster : BoosterBase
{
    protected override int CurrentCount { get => UserDataManager.ShuffleBooster; set => UserDataManager.ShuffleBooster = value; }

    public override void Init()
    {
        base.Init();
    }

    protected override void OnLevelStart(int obj)
    {
        base.OnLevelStart(obj);
    }

    public override void CancelBooster()
    {
        base.CancelBooster();
    }

    public override void ActiveBooster()
    {
        base.ActiveBooster();
        UpdateVisualBooster();
        IsShowConfirm = false;
        LevelController.Instance.GunBoardController.ShuffleBoard();
        this.Wait(0.9f, () =>
        {
            Done();
        });
        OnStartUseBooster?.Invoke(this, CurrentCount);
    }

    protected override void ShowBooster()
    {
        base.ShowBooster();
        IsShowConfirm = true;
    }

    protected override void Done()
    {
        base.Done();
    }


}
