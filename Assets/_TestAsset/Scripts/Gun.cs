using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.TextCore.Text;
using static UnityEngine.GridBrushBase;
using static UnityEngine.UI.CanvasScaler;

namespace ColorBlockCrush
{
    public class Gun : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private TextMeshPro _bulletCountText;
        [SerializeField] private Bullet bulletPrb;
        [SerializeField] private Transform bulletSpawnPos;
        [SerializeField] private float turnDuration = 0.4f;

        private float nextFireTime;
        private bool isTurning;
        private bool isFireFirstTime = false;
        private TrayItem trayItem;
        private GunPos gunPos;
        private RotationDirection currentFireDir = RotationDirection.Up;

        public int BulletCount { get; private set; }
        public bool isMoving { get; private set; }
        public ColorType Color { get; private set; }
        public float FireRate { get; private set; }
        public int ColumnIndex { get; set; }
        public bool IsFrontRow { get; set; }

        public List<Gun> ConnectedGuns { get; private set; }

        public Block CurrentTarget { get; set; }
        public TrayItem TrayItem { get => trayItem; set => trayItem = value; }
        public GunPos GunPos { get => gunPos; set => gunPos = value; }
        public RotationDirection CurrentFireDir { get => currentFireDir; set => currentFireDir = value; }

        public Action<Gun> OnGunFired;
        public Action<Gun> OnGunEmpty;

        public void Init(ColorType color, int bulletCount, float fireRate, int column)
        {
            GunPos = GunPos.ON_GUN_BOARD;
            currentFireDir = RotationDirection.Up;
            OnGunFired = null;
            OnGunEmpty = null;
            CurrentTarget = null;
            ConnectedGuns = new List<Gun>();
            Color = color;
            BulletCount = bulletCount;
            FireRate = fireRate;
            ColumnIndex = column;
            nextFireTime = 0f;
            IsFrontRow = false;
            isFireFirstTime = false;
            isTurning = false;
            UpdateVisuals();
            InvokeRepeating(nameof(TestRay), 0f, 0.05f);
        }

        private void Update()
        {
        }

        private void OnDisable()
        {
            transform.DOKill(this);
        }

        private void TestRay()
        {
            if (!CanFire())
            {
                return;
            }
            var dir = GetFireDirection(currentFireDir);
            Debug.DrawRay(transform.position, dir * 10, UnityEngine.Color.red, 10);
        }


        public bool IsConnectedGroup()
        {
            return ConnectedGuns.Count > 0;
        }

        public bool CanPushToConveyor()
        {
            if (GunPos == GunPos.ON_CONVEYOR || GunPos == GunPos.TWEEN_SORT) return false;

            if (GunPos == GunPos.ON_GUN_BOARD)
            {
                if (!IsConnectedGroup())
                    return IsFrontRow;


                foreach (Gun gun in ConnectedGuns)
                {
                    if (!gun.IsFrontRow) return false;
                }
            }

            return true;
        }


        public bool CanFire()
        {
            return !isTurning && BulletCount > 0
                && Time.time >= nextFireTime
                && GunPos == GunPos.ON_CONVEYOR
                ;
        }

        public Block GetTargetPointFromForward(Transform start, float maxDist, LayerMask mask)
        {
            Ray ray = new Ray(start.position, start.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDist, mask, QueryTriggerInteraction.Ignore))
            {
                hit.transform.TryGetComponent(out Block block);
                return block;
            }

            return null;
        }

        public void Fire(Block target)
        {
            if (!CanFire()) return;

            RotateToBoard(CurrentTarget.transform);
            BulletCount--;
            nextFireTime = Time.time + (1f / FireRate);

            UpdateBulletCountDisplay();

            Bullet bullet = Instantiate(bulletPrb, bulletSpawnPos.position, Quaternion.identity);

            bullet.OnInit(this, target, (gun, block) =>
            {

            });

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

        public void Turn(RotationDirection direction)
        {
            if (isFireFirstTime)
            {
                return;
            }

            isTurning = true;
            Vector3 newRotation = GetTurnDirection(direction);
            transform.DORotate(newRotation, turnDuration).OnComplete(() =>
            {
                isTurning = false;
            }).SetId(this);
        }

        private Vector3 GetFireDirection(RotationDirection rotationDirection)
        {
            Vector3 newRotation = Vector3.zero;
            switch (rotationDirection)
            {
                case RotationDirection.Up:
                    newRotation = Vector3.forward;
                    break;
                case RotationDirection.Down:
                    newRotation = Vector3.back;
                    break;
                case RotationDirection.Left:
                    newRotation = Vector3.left;
                    break;
                case RotationDirection.Right:
                    newRotation = Vector3.right;
                    break;
            }
            return newRotation;
        }

        private Vector3 GetTurnDirection(RotationDirection rotationDirection)
        {
            Vector3 newRotation = Vector3.zero;
            switch (rotationDirection)
            {
                case RotationDirection.Up:
                    newRotation = new Vector3(0, 0, 0);
                    break;
                case RotationDirection.Down:
                    newRotation = new Vector3(0, 180, 0);
                    break;
                case RotationDirection.Left:
                    newRotation = new Vector3(0, -90, 0);
                    break;
                case RotationDirection.Right:
                    newRotation = new Vector3(0, 90, 0);
                    break;
            }
            return newRotation;
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

        #region Move spline
        Tween moveToConveyorTw;
        Tween moveToSlotTw;
        Tween moveSortSlotTw;
        public void MoveToConeyor(Vector3 endPos, Action callback = null)
        {
            Sequence moveToConveyorSq = DOTween.Sequence();
            GunPos = GunPos.ON_CONVEYOR;
            moveToConveyorTw = moveToConveyorSq.Append(transform.DOJump(endPos, 3, 1, 0.3f)).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                callback?.Invoke();
            });
            moveToConveyorSq.SetId(this);
        }

        public void MoveToSlot(Vector3 endPos, Action callback = null)
        {
            Sequence moveToSlotSq = DOTween.Sequence();

            GunPos = GunPos.ON_SLOT;
            moveToSlotTw = moveToSlotSq.Append(transform.DOJump(endPos, 3, 1, 0.3f)).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                callback?.Invoke();
            });
            moveToSlotSq.Join(transform.DORotate(Vector3.zero, 0.3f));
            moveToSlotSq.SetId(this);
        }

        public void MoveSortSlot(Vector3 targetPos, float _shiftDuration, Ease _shiftEase)
        {
            GunPos = GunPos.TWEEN_SORT;
            Sequence moveSortSlotSq = DOTween.Sequence();

            moveSortSlotTw = moveSortSlotSq.Append(transform.DOMove(targetPos, _shiftDuration).SetEase(_shiftEase)).OnComplete(() =>
            {
                GunPos = GunPos.ON_SLOT;
            });
            moveSortSlotSq.SetId(this);
        }

        public void Destroy()
        {

        }
        #endregion
    }

    public enum GunPos
    {
        ON_GUN_BOARD = 0,
        ON_SLOT = 1,
        ON_CONVEYOR = 2,
        TWEEN_SORT = 3,
    }
}
