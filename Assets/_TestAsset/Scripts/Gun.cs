using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

namespace ColorBlockCrush
{
    public class Gun : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private TextMeshPro _bulletCountText;
        [SerializeField] private SplineAnimate splineAnimate;

        private float _nextFireTime;
        private bool isFireFirstTime = false;
        private Tween moveConveyorTween;

        public int BulletCount { get; private set; }
        public ColorType Color { get; private set; }
        public float FireRate { get; private set; }
        public int ColumnIndex { get; set; }
        public bool IsFrontRow { get; set; }

        public List<Gun> ConnectedGuns { get; private set; }

        public Block CurrentTarget { get; set; }

        public Action<Gun> OnGunFired;
        public Action<Gun> OnGunEmpty;

        public void Initialize(ColorType color, int bulletCount, float fireRate, int column)
        {
            OnGunFired = null;
            OnGunEmpty = null;
            CurrentTarget = null;
            ConnectedGuns = new List<Gun>();
            Color = color;
            BulletCount = bulletCount;
            FireRate = fireRate;
            ColumnIndex = column;
            _nextFireTime = 0f;
            IsFrontRow = false;
            isFireFirstTime = false;

            UpdateVisuals();
        }

        public bool IsConnectedGroup()
        {
            return ConnectedGuns.Count > 0;
        }

        public bool CanPushToConveyor()
        {
            if (!IsConnectedGroup())
                return IsFrontRow;

            if (!IsFrontRow) return false;

            foreach (Gun gun in ConnectedGuns)
            {
                if (!gun.IsFrontRow) return false;
            }

            return true;
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

            RotateToBoard(CurrentTarget.transform);
            BulletCount--;
            _nextFireTime = Time.time + (1f / FireRate);

            UpdateBulletCountDisplay();
            OnGunFired?.Invoke(this);

            if (BulletCount == 0)
            {
                CurrentTarget = null;
                OnGunEmpty?.Invoke(this);
            }
        }

        private void RotateToBoard(Transform target)
        {
            if (!isFireFirstTime)
            {
                isFireFirstTime = true;
            }
            else
            {
                return;
            }

            Vector3 direction = target.position - transform.position;
            direction.y = 0;

            float absX = Mathf.Abs(direction.x);
            float absZ = Mathf.Abs(direction.z);

            float targetAngle;

            if (absX > absZ)
            {
                targetAngle = direction.x > 0 ? 90f : -90f; // Right : Left
            }
            else
            {
                targetAngle = direction.z > 0 ? 0f : 180f; // Forward : Back
            }

            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
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

        #region Move
        public void MoveToConeyor(Vector3 endPos)
        {
            splineAnimate.Alignment = SplineAnimate.AlignmentMode.SplineElement;
            Sequence moveToConeyorSq = null;
            moveConveyorTween = moveToConeyorSq.Append(transform.DOJump(endPos, 1, 1, 0.3f));

            moveConveyorTween.OnComplete(() =>
            {
                splineAnimate.Play();
            });
        }

        public void Destroy()
        {

        }
        #endregion
    }
}
