using ColorBlockCrush.Tools;
using DG.Tweening;
using System;
using System.Collections.Generic;
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

        public void RemoveGunByColor(ColorType colorType)
        {
            for (int i = _gunsInSlots.Count - 1; i >= 0; i--)
            {
                if (_gunsInSlots[i].ColorType == colorType)
                {
                    _gunsInSlots[i].RemoveAllConnection();
                    _gunsInSlots[i].ForceDisappear();
                }
            }
            ShiftAllToTheLeft();
        }

        public void MoveGunIn(Gun gun)
        {
            _gunsInSlots.Add(gun);
            gun.transform.SetParent(gunContainer);

            int slotIndex = _gunsInSlots.Count - 1;
            Vector3 targetPos = GetSlotPosition(slotIndex);

            gun.MoveToBonusSlot(targetPos, () =>
            {
                ShiftAllToTheLeft();
            });
        }

        public void MoveGunsIn(List<Gun> guns)
        {
            foreach (Gun gun in guns)
            {
                MoveGunIn(gun);
            }
        }

        public void OnTapGun(Gun gun)
        {
            List<Gun> gunsToPush = new List<Gun>();

            if (gun.IsConnectedGroup())
            {
                AddAllGunToPush(gunsToPush, gun);
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

            //gunsToPush.Sort((a, b) =>
            //{
            //    int result = a.ColumnIndex.CompareTo(b.ColumnIndex);
            //    if (result == 0)
            //        result = a.ConnectedGuns.Count.CompareTo(b.ConnectedGuns.Count); // second field

            //    return result;
            //});

            LevelController.Instance.ConveyorController.MoveGunIn(gunsToPush);

            foreach (var gunn in gunsToPush)
            {
                RemoveGun(gunn);
            }
        }

        private void AddAllGunToPush(List<Gun> listGunToPush, Gun gun)
        {
            var stack = new Stack<Gun>();
            stack.Push(gun);
            List<Gun> visited = new();
            while (stack.Count > 0)
            {
                var gunTemp = stack.Pop();

                if (listGunToPush.Count < 2)
                {
                    listGunToPush.Add(gunTemp);
                }
                else if (listGunToPush[0].ConnectedGuns.Count <= 1 && listGunToPush[^1].ConnectedGuns.Count <= 1)
                {
                    listGunToPush.Insert(listGunToPush.Count - 2, gunTemp);
                }
                else if (listGunToPush[0].ConnectedGuns.Count <= 1)
                {
                    listGunToPush.Add(gunTemp);
                }
                else if (listGunToPush[^1].ConnectedGuns.Count <= 1)
                {
                    listGunToPush.Insert(0, gunTemp);
                }

                for (int i = 0; i < gunTemp.ConnectedGuns.Count; i++)
                {
                    var linkGun = gunTemp.ConnectedGuns[i];
                    if (!listGunToPush.Contains(linkGun))
                    {
                        stack.Push(linkGun);
                    }
                }
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