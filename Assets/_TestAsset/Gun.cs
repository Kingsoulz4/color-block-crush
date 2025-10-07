using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ColorBlockCrush
{
    public class Gun : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private TextMeshPro _bulletCountText;

        public int BulletCount { get; private set; }
        public ColorType Color { get; private set; }
        public float FireRate { get; private set; }
        public Vector2Int GridPosition { get; set; }

        public List<Gun> ConnectedGuns { get; private set; }

        public Block CurrentTarget { get; set; }
        public int BulletsReservedForTarget { get; set; }
        private float _nextFireTime;

        public Action<Gun> OnGunFired;
        public Action<Gun> OnGunEmpty;

        private void Awake()
        {
            ConnectedGuns = new List<Gun>();
        }

        public void Initialize(ColorType color, int bulletCount, float fireRate)
        {
            Color = color;
            BulletCount = bulletCount;
            FireRate = fireRate;
            _nextFireTime = 0f;

            UpdateVisuals();
        }

        public bool IsConnectedGroup()
        {
            return ConnectedGuns.Count > 0;
        }

        public bool CanPushToSlot()
        {
            if (!IsConnectedGroup())
                return GridPosition.x == 0;

            if (GridPosition.x != 0) return false;

            foreach (Gun gun in ConnectedGuns)
            {
                if (gun.GridPosition.x != 0) return false;
            }

            return true;
        }

        public int GetRequiredSlots()
        {
            return IsConnectedGroup() ? (ConnectedGuns.Count + 1) : 1;
        }

        public bool CanFire()
        {
            if (BulletCount <= 0) return false;
            if (Time.time < _nextFireTime) return false;
            return true;
        }

        public void Fire()
        {
            if (!CanFire()) return;

            BulletCount--;
            _nextFireTime = Time.time + (1f / FireRate);

            UpdateBulletCountDisplay();
            OnGunFired?.Invoke(this);

            if (BulletCount == 0 || BulletsReservedForTarget <= 0)
            {
                CurrentTarget = null;
                BulletsReservedForTarget = 0;

                if (BulletCount == 0)
                {
                    OnGunEmpty?.Invoke(this);
                }
            }
            else
            {
                BulletsReservedForTarget--;
            }
        }

        public Vector3 GetSlotPosition()
        {
            return transform.position;
        }

        private void UpdateVisuals()
        {
            if (_meshRenderer != null)
            {
                Material mat = _meshRenderer.material;
                mat.color = GetColorFromType(Color);
            }

            UpdateBulletCountDisplay();
        }

        private void UpdateBulletCountDisplay()
        {
            if (_bulletCountText != null)
            {
                _bulletCountText.text = BulletCount.ToString();
            }
        }

        private UnityEngine.Color GetColorFromType(ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.Red: return UnityEngine.Color.red;
                case ColorType.Blue: return UnityEngine.Color.blue;
                case ColorType.Green: return UnityEngine.Color.green;
                case ColorType.Yellow: return UnityEngine.Color.yellow;
                case ColorType.Purple: return new UnityEngine.Color(0.5f, 0, 0.5f);
                default: return UnityEngine.Color.white;
            }
        }
    }
}