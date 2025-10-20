using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ColorBlockCrush
{
    public class BonusSlotController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _maxSlots = 6;
        [SerializeField] private Vector3 _slotStartPosition = new Vector3(-3.5f, 0f, -5);
        [SerializeField] private float _slotSpacing = 0.95f;
        [SerializeField] private GameObject slotItemPrb;
        [SerializeField] private Transform gunContainer;

        [Header("Animation")]
        [SerializeField] private float _shiftDuration = 0.2f;
        [SerializeField] private Ease _shiftEase = Ease.OutQuad;

        private List<Gun> _gunsInSlots;

        private void Awake()
        {
            _gunsInSlots = new List<Gun>();
        }

        public void Init()
        {
            _gunsInSlots.Clear();
        }

        private Vector3 GetSlotPosition(int index)
        {
            return _slotStartPosition + new Vector3(index * _slotSpacing, 0, 0);
        }

        public bool CanPlaceGuns(int count)
        {
            return _gunsInSlots.Count + count <= _maxSlots;
        }

        public void MoveGunIn(Gun gun)
        {
            //if (gun.ConnectedGuns.Count > 1)
            //{
            //    if (!CanPlaceGuns(gun.ConnectedGuns.Count))
            //    {
            //        LevelController.Instance.ConveyorController.PauseAllTray();
            //        LevelController.Instance.LoseLevel();
            //        return;
            //    }
            //}
            //else if (!CanPlaceGuns(1))
            //{
            //    LevelController.Instance.ConveyorController.PauseAllTray();
            //    LevelController.Instance.LoseLevel();
            //    return;
            //}

            _gunsInSlots.Add(gun);
            gun.transform.SetParent(gunContainer);

            int slotIndex = _gunsInSlots.Count - 1;
            Vector3 targetPos = GetSlotPosition(slotIndex);

            gun.MoveToBonusSlot(targetPos, () =>
            {
                ShiftAllToTheLeft();
            });
        }

        public void OnTapGun(Gun gun)
        {
            List<Gun> gunsToPush = new List<Gun>();

            if (gun.IsConnectedGroup())
            {
                gunsToPush.Add(gun);
                gunsToPush.AddRange(gun.ConnectedGuns);
            }
            else
            {
                gunsToPush.Add(gun);
            }

            int requiredSlots = gunsToPush.Count;

            if (!LevelController.Instance.ConveyorController.CanPlaceGuns(requiredSlots))
            {
                this.Wait(0.15f, () =>
                {
                    gun.PlayAnim(Constant.GunAnimation.IDLE);
                });
                Debug.Log("Not enough slots available");
                return;
            }

            gunsToPush.Sort((a, b) => a.ColumnIndex.CompareTo(b.ColumnIndex));

            LevelController.Instance.ConveyorController.MoveGunIn(gunsToPush);

            RemoveGun(gun);

            foreach (var g in gun.ConnectedGuns)
            {
                RemoveGun(g);
            }
        }

        public void RemoveGun(Gun gun)
        {
            int removedIndex = _gunsInSlots.IndexOf(gun);

            if (_gunsInSlots.Remove(gun))
            {
                ShiftAllToTheLeft(removedIndex);
            }
        }

        private void ShiftAllToTheLeft(int fromIndex = 0)
        {
            for (int i = fromIndex; i < _gunsInSlots.Count; i++)
            {
                Vector3 targetPos = GetSlotPosition(i);
                _gunsInSlots[i].MoveSortSlot(targetPos, _shiftDuration, _shiftEase, GunPos.ON_BONUS_SLOT);
            }
        }
    }
}