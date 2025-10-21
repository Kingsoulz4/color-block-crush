using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class AddTrayBooster : BoosterBase
    {
        protected override int CurrentCount { get => UserDataManager.AddTrayBooster; set => UserDataManager.AddTrayBooster = value; }
        private int maxTrayCount = 5;
        private int currentCount = 0;

        public override void Init()
        {
            base.Init();
        }

        protected override void OnLevelStart(int obj)
        {
            base.OnLevelStart(obj);
            currentCount = 0;
        }

        protected override bool CanShowBooster()
        {
            return base.CanShowBooster() && currentCount < maxTrayCount;
        }

        public override void DoShowBooster(Action<bool> callback = null)
        {
            if (CanShowBooster())
            {
                ShowBooster();
                callback?.Invoke(true);

            }
            else
            {
                if (currentCount >= maxTrayCount)
                {
                    LevelController.Instance.ConveyorController.WarnTrayText();
                    UIManager.Instance.NotifyContent("You can't use it now!");
                    return;
                }

                PopupBuyBooster poup = UIManager.Instance.GetPopupActive<PopupBuyBooster>();
                if (poup == null)
                {
                    GameManager.Instance.SetGameState(GameState.Paused);
                    poup = UIManager.Instance.ShowPopup<PopupBuyBooster>(() =>
                    {
                        GameManager.Instance.SetGameState(GameState.Playing);
                        OnUseBoosterDone?.Invoke(this, CurrentCount);
                    });

                    poup.Show(BoosterType);
                    poup.OnBought = UpdateVisualBooster;
                }
                callback?.Invoke(false);
            }
        }

        public override void ActiveBooster()
        {
            base.ActiveBooster();
            OnStartUseBooster?.Invoke(this, CurrentCount);
            currentCount += 1;
            LevelController.Instance.ConveyorController.BoosterAddTrayItem();
            Done();
        }

        protected override void Done()
        {
            base.Done();
        }
    }
}
