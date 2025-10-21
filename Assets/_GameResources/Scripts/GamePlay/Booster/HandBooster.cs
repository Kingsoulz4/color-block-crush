using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColorBlockCrush
{
    public class HandBooster : BoosterBase
    {

        [SerializeField] private float zOffetCam;
        private float originCamZ;
        protected override int CurrentCount { get => UserDataManager.HandBooster; set => UserDataManager.HandBooster = value; }

        public override void Init()
        {
            base.Init();
            LevelEvent.OnGunClick += OnGunClick;

        }

        private void OnGunClick(Gun gun)
        {
            if (IsShowConfirm)
            {
                StartCoroutine(DoBooster());
                gun.OnGunClicked(true);
            }
        }

        private void OnDisable()
        {
            LevelEvent.OnGunClick += OnGunClick;
        }

        public override void CancelBooster()
        {
            base.CancelBooster();
            IsShowConfirm = false;
            Camera.main.GetComponent<GameCamera>().MoveZ(originCamZ, 0.2f);
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
            originCamZ = Camera.main.transform.position.z;
            Camera.main.GetComponent<GameCamera>().MoveZ(zOffetCam, 0.2f);
        }

        protected override void Done()
        {
            base.Done();
            Camera.main.GetComponent<GameCamera>().MoveZ(originCamZ, 0.2f);
        }
    }
}
