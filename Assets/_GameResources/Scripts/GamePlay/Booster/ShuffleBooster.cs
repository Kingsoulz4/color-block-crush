using DG.Tweening;
using ColorBlockCrush;
using System.Collections;
using UnityEngine;

public class ShuffleBooster : BoosterBase
{
    protected override int CurrentCount { get => UserDataManager.ShuffleBooster; set => UserDataManager.ShuffleBooster = value; }

    public override void Init()
    {
        base.Init();
    }

    public override void CancelBooster()
    {
        base.CancelBooster();
        IsShowConfirm = false;
    }

    public override void ActiveBooster()
    {
        base.ActiveBooster();
        UpdateVisualBooster();
        IsShowConfirm = false;
        LevelController.Instance.GunBoardController.ShuffleBoard();
        Done();
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
