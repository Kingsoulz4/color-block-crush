using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ColorBlockCrush
{
    public class SlotController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int _maxSlots = 5;
        [SerializeField] private Vector3 _slotStartPosition = new Vector3(-2f, 0f, 0);
        [SerializeField] private float _slotSpacing = 1f;
        [SerializeField] private ShooterSlot slotItemPrb;
        [SerializeField] private Transform gunContainer;

        [Header("Animation")]
        [SerializeField] private float _shiftDuration = 0.2f;
        [SerializeField] private Ease _shiftEase = Ease.OutQuad;

        private List<Gun> _gunsInSlots;
        private List<ShooterSlot> _slotItems;


        private void Awake()
        {
            _gunsInSlots = new List<Gun>();
            _slotItems = new List<ShooterSlot>();
        }

        public void Init()
        {
            _gunsInSlots.Clear();
            SpawnSlotItem();
        }

        public void Warn()
        {
            for (int i = 0; i < _slotItems.Count; i++)
            {
                _slotItems[i].StartWarning();
            }
        }

        public void StopWarn()
        {
            for (int i = 0; i < _slotItems.Count; i++)
            {
                _slotItems[i].StopWarning();
            }
        }

        private void SpawnSlotItem()
        {
            // Clear old slots
            foreach (var slot in _slotItems)
            {
                if (slot != null) Destroy(slot);
            }
            _slotItems.Clear();

            // Spawn new slots
            for (int i = 0; i < _maxSlots; i++)
            {
                Vector3 slotPos = GetSlotPosition(i);
                var slotItem = Instantiate(slotItemPrb, transform);
                slotItem.transform.position = slotPos;
                _slotItems.Add(slotItem);
            }
        }

        private Vector3 GetSlotPosition(int index)
        {
            return _slotStartPosition + new Vector3(index * _slotSpacing, 0, 0);
        }

        public bool CanPlaceGuns(int count)
        {
            return _gunsInSlots.Count + count <= _maxSlots;
        }

        //public void MoveGunsIn(List<Gun> guns)
        //{
        //    if (!CanPlaceGuns(guns.Count))
        //    {
        //        return;
        //    }

        //    foreach (Gun gun in guns)
        //    {
        //        _gunsInSlots.Add(gun);

        //        int slotIndex = _gunsInSlots.Count - 1;
        //        Vector3 targetPos = GetSlotPosition(slotIndex);
        //        gun.transform.DOMove(targetPos, _shiftDuration).SetEase(_shiftEase);

        //        OnGunAddedToSlot?.Invoke(gun);
        //    }
        //}
        public void MoveGunIn(Gun gun)
        {
            if (gun.ConnectedGuns.Count > 1)
            {
                if (!CanPlaceGuns(gun.ConnectedGuns.Count))
                {
                    LevelController.Instance.LoseLevel();
                    return;
                }
            }
            else if (!CanPlaceGuns(1))
            {
                LevelController.Instance.LoseLevel();
                return;
            }

            _gunsInSlots.Add(gun);
            gun.transform.SetParent(gunContainer);

            if (_gunsInSlots.Count >= _maxSlots)
            {
                Warn();
            }

            // Move gun tới vị trí slot mới
            int slotIndex = _gunsInSlots.Count - 1;
            Vector3 targetPos = GetSlotPosition(slotIndex);

            gun.MoveToSlot(targetPos, () =>
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

            if (_gunsInSlots.Count == _maxSlots)
            {
                StopWarn();
            }


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
                _gunsInSlots[i].MoveSortSlot(targetPos, _shiftDuration, _shiftEase);
            }
        }
    }
}