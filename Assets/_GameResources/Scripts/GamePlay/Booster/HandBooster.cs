using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
            originCamZ = Camera.main.transform.position.z;
            LevelEvent.OnGunClick += OnGunClick;

        }

        protected override void OnLevelStart(int obj)
        {
            base.OnLevelStart(obj);
            Camera.main.GetComponent<GameCamera>().MoveZ(originCamZ, 0.2f);
        }

        private void OnGunClick(Gun gun)
        {
            if (IsShowConfirm)
            {
                StartCoroutine(DoBooster());
                gun.OnGunClicked(true);
                gun.ForceResoveHidden();
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
            Camera.main.GetComponent<GameCamera>().MoveZ(zOffetCam, 0.2f);
            LevelController.Instance.ConveyorController.PauseAllTray();
        }

        protected override void Done()
        {
            base.Done();
            Camera.main.GetComponent<GameCamera>().MoveZ(originCamZ, 0.2f);
        }
    }
}
