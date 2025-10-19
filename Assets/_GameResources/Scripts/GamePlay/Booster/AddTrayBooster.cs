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
            currentCount = 0;
        }

        public override void ActiveBooster()
        {
            base.ActiveBooster();
            OnStartUseBooster?.Invoke(this, CurrentCount);

            Done();
        }

        protected override void Done()
        {
            base.Done();
        }
    }
}
