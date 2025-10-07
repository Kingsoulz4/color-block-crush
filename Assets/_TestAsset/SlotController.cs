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

        private List<Gun> _gunsInSlots;

        public Action<Gun> OnGunAddedToSlot;
        public Action<Gun> OnGunRemovedFromSlot;
        public Action OnLoseConditionMet;

        private void Awake()
        {
            _gunsInSlots = new List<Gun>();
        }

        public void Initialize()
        {
            _gunsInSlots.Clear();
        }

        public bool CanPlaceGuns(int count)
        {
            return _gunsInSlots.Count + count <= _maxSlots;
        }

        public void PushGuns(List<Gun> guns)
        {
            foreach (Gun gun in guns)
            {
                _gunsInSlots.Add(gun);

                int slotIndex = _gunsInSlots.Count - 1;
                Vector3 slotPos = _slotStartPosition + new Vector3(slotIndex * _slotSpacing, 0, 0);
                gun.transform.position = slotPos;

                gun.OnGunEmpty += OnGunEmpty;

                OnGunAddedToSlot?.Invoke(gun);
            }
        }

        public void RemoveGun(Gun gun)
        {
            if (_gunsInSlots.Remove(gun))
            {
                gun.OnGunEmpty -= OnGunEmpty;
                OnGunRemovedFromSlot?.Invoke(gun);

                RepositionGuns();
                CheckLoseCondition();
            }
        }

        private void RepositionGuns()
        {
            for (int i = 0; i < _gunsInSlots.Count; i++)
            {
                Vector3 slotPos = _slotStartPosition + new Vector3(i * _slotSpacing, 0, 0);
                _gunsInSlots[i].transform.position = slotPos;
            }
        }

        private void OnGunEmpty(Gun gun)
        {
            RemoveGun(gun);
            Destroy(gun.gameObject);
        }

        public void CheckLoseCondition()
        {
            if (_gunsInSlots.Count < _maxSlots) return;
        }

        public List<Gun> GetAllGuns()
        {
            return new List<Gun>(_gunsInSlots);
        }

        public int GetSlotCount()
        {
            return _gunsInSlots.Count;
        }

        public bool IsFull()
        {
            return _gunsInSlots.Count >= _maxSlots;
        }
    }
}