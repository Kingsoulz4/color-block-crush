using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class SuperShootBooster : BoosterBase
    {
        protected override int CurrentCount { get => UserDataManager.MagnetBooster; set => UserDataManager.MagnetBooster = value; }

        private void Update()
        {
            if (IsShowConfirm && Input.GetMouseButton(0))
            {
                StartCoroutine(DoBooster());
            }
        }

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
            OnStartUseBooster?.Invoke(this, CurrentCount);
        }

        private IEnumerator DoBooster()
        {
            ActiveBooster();
            yield return null;
            Done();
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
}
