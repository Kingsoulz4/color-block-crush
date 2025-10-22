using ColorBlockCrush.Tools;
using DG.Tweening;
using System;
using System.Collections.Generic;
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
                LevelController.Instance.ConveyorController.WarnTrayText();
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

            foreach(var gunn in gunsToPush)
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

            if (_gunsInSlots.Count == _maxSlots)
            {
                StopWarn();
            }


            if (_gunsInSlots.Remove(gun))
            {
                ShiftAllToTheLeft(removedIndex);
            }
        }

        public Gun GetLastGun()
        {
            Gun lastGun = _gunsInSlots[_gunsInSlots.Count-1];
            RemoveGun(lastGun);
            return lastGun;
        }

        private void ShiftAllToTheLeft(int fromIndex = 0)
        {
            for (int i = fromIndex; i < _gunsInSlots.Count; i++)
            {
                Vector3 targetPos = GetSlotPosition(i);
                _gunsInSlots[i].MoveSortSlot(targetPos, _shiftDuration, _shiftEase, GunPos.ON_SLOT);
            }
        }
    }
}