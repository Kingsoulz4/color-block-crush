using ColorBlockCrush;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoosterType
{
    NONE = -1,
    ADD_TRAY = 0,
    SHUFFLE = 1,
    HAND_MOVE = 2,
    SUPER_SHOOT = 3,
    REVIVAL = 4,
    LIVES = 5
}
public abstract class BoosterBase : MonoBehaviour
{
    [SerializeField] BoosterType boosterType;
    private int currentCount;
    private bool canActive => !InProgress;
    protected bool inProgress;
    private bool isShowConfirm = false;

    public Action<BoosterBase, int> OnStartUseBooster;
    public Action<BoosterBase, int> OnChangeBoosterCount;
    public Action<BoosterBase, int> OnUseBoosterDone;
    public Action<BoosterBase, int> OnCancelBooster;

    protected virtual int CurrentCount
    {
        get
        {

            return currentCount;
        }
        set
        {

            currentCount = value;
        }
    }

    public bool InProgress { get => inProgress; set => inProgress = value; }
    public BoosterType BoosterType { get => boosterType; }
    protected bool IsShowConfirm { get => isShowConfirm; set => isShowConfirm = value; }

    protected virtual bool CanShowBooster() => CurrentCount > 0 && canActive;

    public virtual void Init()
    {
        InProgress = false;
    }

    public virtual void DoShowBooster(Action<bool> callback = null)
    {
        if (CanShowBooster())
        {
            ShowBooster();
            callback?.Invoke(true);

        }
        else
        {
            if (inProgress)
            {
                UIManager.Instance.NotifyContent("You can't use it now!");
                return;
            }
            // Show popup buy
            PopupBuyBooster poup = UIManager.Instance.GetPopupActive<PopupBuyBooster>();
            if (poup == null)
            {
                GameManager.Instance.SetGameState(GameState.Paused);
                poup = UIManager.Instance.ShowPopup<PopupBuyBooster>(() =>
                {
                    GameManager.Instance.SetGameState(GameState.Playing);
                    OnUseBoosterDone?.Invoke(this, CurrentCount);
                });

                poup.Show(boosterType);
                poup.OnBought = UpdateVisualBooster;
            }
            callback?.Invoke(false);
        }
    }

    //public void UpdateCountBooster()
    //{
    //    CurrentCount += 1;
    //    UpdateVisualBooster();
    //}

    public void UpdateVisualBooster()
    {
        OnChangeBoosterCount?.Invoke(this, CurrentCount);
    }

    public virtual void ActiveBooster()
    {
        CurrentCount--;
    }

    public virtual void CancelBooster()
    {
        IsShowConfirm = false;
        InProgress = false;
        OnCancelBooster?.Invoke(this, 0);
    }

    protected virtual void ShowBooster()
    {
        InProgress = true;
    }

    protected virtual void Done()
    {
        InProgress = false;
        OnUseBoosterDone?.Invoke(this, CurrentCount);
    }
}
