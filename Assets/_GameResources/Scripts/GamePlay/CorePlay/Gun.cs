using ColorBlockCrush.Tools;
using DG.Tweening;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.GridBrushBase;
using static UnityEngine.UI.CanvasScaler;

namespace ColorBlockCrush
{
    public partial class Gun : ObjectOnGunBoardColumn
    {
        [Header("Visual")]
        [SerializeField] private List<Renderer> meshRendererList;
        [SerializeField] private TextMeshPro bulletCountText;
        [SerializeField] private Bullet bulletPrb;
        [SerializeField] private Transform bulletSpawnPos;
        [SerializeField] private float turnDuration = 0.4f;
        [SerializeField] private LayerMask blockMask;
        [SerializeField] private ListMaterialByColor colorReference;
        [SerializeField] private float fireRate = 3f;
        [SerializeField] private Transform raycastPos;
        [SerializeField] private GunAnim anim;

        [Header("Move")]
        [SerializeField] private float moveToConveyorDuration = 0.4f;
        [SerializeField] private float moveToConveyorJumpForce = 2;
        [SerializeField] private Ease moveToConveyorEase = Ease.OutQuad;
        [SerializeField] private float moveToSlotDuration = 0.25f;

        private const float raycastSpacing = 0.15f;
        private const int maxRaycastSteps = 65;

        private float maxShootingAngle = 5f;
        private bool isTurning;
        private bool isFireFirstTime = false;
        private TrayItem trayItem;
        private GunPos gunPos;
        private RotationDirection currentFireDir = RotationDirection.Up;
        private RotationDirection currentMoveFireDir = RotationDirection.Right;
        private GunConfig gunData;
        private Queue<Block> targetQueue;
        private List<Block> targetQueu1e = new List<Block>();

        public int ID { get; set; }
        public GunConfig GunData => gunData;
        public int BulletCount { get; private set; }
        public int BulletRayCount { get; private set; }
        public ColorType ColorType { get; private set; }
        public bool IsFrontRow { get; set; }
        public List<Gun> ConnectedGuns { get => ConnectedGunHandler.ListGun; }
        public Block CurrentTarget { get; set; }
        public TrayItem TrayItem { get => trayItem; set => trayItem = value; }
        public GunPos GunPos { get => gunPos; set => gunPos = value; }
        public RotationDirection CurrentFireDir { get => currentFireDir; set => currentFireDir = value; }
        public RotationDirection CurrentMoveFireDir { get => currentMoveFireDir; set => currentMoveFireDir = value; }

        public Action<Gun> OnGunFired;
        public Action<Gun> OnGunEmpty;
        public Action<Gun> OnGunDissapear;

        public void Init(GunConfig gunDataP, int column, int id)
        {
            ID = id;
            GunPos = GunPos.ON_GUN_BOARD;
            currentFireDir = RotationDirection.Up;
            OnGunFired = null;
            OnGunEmpty = null;
            OnGunDissapear = null;
            CurrentTarget = null;
            //ConnectedGuns = new List<Gun>();
            ColorType = gunDataP.colorType;
            BulletCount = gunDataP.bulletNumber;
            BulletRayCount = gunDataP.bulletNumber;
            ColumnIndex = column;
            IsFrontRow = false;
            isFireFirstTime = false;
            isTurning = false;
            gunData = gunDataP;
            targetQueue = new Queue<Block>();
            UpdateVisuals();
            InitMechanics();
            LevelEvent.OnFastMode += OnFastMode;

        }

        private void OnFastMode()
        {
            if (gameObject.activeInHierarchy)
            {
                maxShootingAngle = 20;
            }
        }

        void Update()
        {
            CheckFire();
            if (targetQueu1e.Count > 0)
            {
                targetQueu1e = new List<Block>(targetQueue);
            }
        }

        private void OnDisable()
        {
            transform.DOKill(this);
            targetQueue.Clear();
            LevelEvent.OnFastMode -= OnFastMode;
        }

        public override void UpdateWhenColumnChange()
        {
            base.UpdateWhenColumnChange();
            UpdateMechanics();
        }

        public override void SetIndex(int index)
        {
            base.SetIndex(index);
            IsFrontRow = index == 0;

            if (index == 0)
            {
                PlayAnim(Constant.GunAnimation.IDLE);
            }
        }

        private void CheckFire()
        {
            if (targetQueue.Count == 0)
                return;

            Block nextBlock = targetQueue.Peek();
            if (!nextBlock || !CanFire(nextBlock))
            {
                return;
            }

            Fire(targetQueue.Dequeue());
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

        public bool CanFire(Block target)
        {
            return !isTurning && BulletCount > 0
                && GunPos == GunPos.ON_CONVEYOR
                && IsBlockInShootingAngle(target)
                ;
        }


        public void GetTargetBock()
        {
            var dir = GetFireDirection(currentFireDir);
            var dirMove = GetFireDirection(currentMoveFireDir);
            for (int i = 0; i < maxRaycastSteps; i++)
            {
                var startPos = raycastPos.position + dirMove * raycastSpacing * i;
                Debug.DrawRay(startPos, dir * 12, UnityEngine.Color.red, 1);

                Ray ray = new Ray(startPos, dir);
                if (Physics.Raycast(ray, out RaycastHit hit, 12, blockMask))
                {
                    hit.transform.TryGetComponent(out Block block);

                    //Debug.Log(block.name);
                    //Debug.Log("CanBeRaycastHit " + block.CanBeRaycastHit());
                    //Debug.Log("Contains " + targetQueue.Contains(block));
                    //Debug.Log("ColorType" + block.ColorType);
                    if (!block.CanBeRaycastHit() || block.ColorType != ColorType || targetQueue.Contains(block))
                    {
                        continue;
                    }

                    int timeTakeDamage = 1;
                    if (currentFireDir == RotationDirection.Up || currentFireDir == RotationDirection.Down)
                    {
                        timeTakeDamage = block.Size.x;
                    }
                    else if (currentFireDir == RotationDirection.Left || currentFireDir == RotationDirection.Right)
                    {
                        timeTakeDamage = block.Size.y;
                    }

                    if (timeTakeDamage > 1)
                    {
                        Debug.Log("Fire > 1 times here");
                    }

                    for (int j = 0; j < timeTakeDamage; j++)
                    {
                        if (BulletRayCount > 0)
                        {
                            BulletRayCount--;
                            block.TakeDamageRaycast(1);
                            targetQueue.Enqueue(block);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                //else 
                //    Debug.Log("!Physics.Raycast");
            }
        }

        public void Fire(Block target)
        {

            RotateToFire(target.transform);
            BulletCount--;
            PlayAnim(Constant.GunAnimation.SHOOT);
            UpdateBulletCountDisplay();

            Bullet bullet = Instantiate(bulletPrb, bulletSpawnPos.position, Quaternion.identity);
            bullet.transform.SetParent(LevelController.Instance.transform);
            bullet.OnInit(this, target, (gun, block) =>
            {
                target.TakeDamage(1);
                Destroy(bullet.gameObject);
            });

            OnGunFired?.Invoke(this);

            if (BulletCount == 0)
            {
                CurrentTarget = null;

                CheckDisappear();

                OnGunEmpty?.Invoke(this);
            }
        }

        private void CheckDisappear()
        {
            if (CheckCanDisappear())
            {
                PlayAnim(Constant.GunAnimation.DISAPPEAR);

                this.Wait(0.2f, () =>
                {
                    gameObject.SetActive(false);
                    OnGunDissapear?.Invoke(this);
                });
            }
        }

        private void RotateToFire(Transform target)
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

            transform.DORotate(new Vector3(0, targetAngle, 0), 0);
        }

        public void Turn(RotationDirection direction, RotationDirection directionNonfire)
        {
            if (GunPos != GunPos.ON_CONVEYOR)
            {
                return;
            }

            Vector3 newRotation;
            newRotation = GetTurnDirection(!isFireFirstTime ? directionNonfire : direction);
            currentMoveFireDir = directionNonfire;
            isTurning = true;
            transform.DORotate(newRotation, turnDuration).OnComplete(() =>
            {
                isTurning = false;
                if (GunPos == GunPos.ON_CONVEYOR)
                {
                    GetTargetBock();
                }
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

        private void UpdateVisuals()
        {
            for (int i = 0; i < meshRendererList.Count; i++)
            {
                var renderer = meshRendererList[i];
                if (!renderer) continue;

                Material mat = colorReference.GetMaterial(ColorType);
                if (mat != null && renderer.sharedMaterial != mat)
                    renderer.sharedMaterial = mat;
            }

            UpdateBulletCountDisplay();
        }

        private void UpdateBulletCountDisplay()
        {
            if (bulletCountText != null)
            {
                bulletCountText.text = BulletCount.ToString();
            }
        }

        private void EnableTextBulletCount(bool enable)
        {
            bulletCountText.gameObject.SetActive(enable);
        }

        #region Move spline
        Tween moveToConveyorTw;
        Tween moveToSlotTw;

        public void MoveToConeyor(Vector3 endPos, Action callback = null)
        {
            isFireFirstTime = false;
            Sequence moveToConveyorSq = DOTween.Sequence();
            GunPos = GunPos.TWEEN_SORT;
            currentFireDir = RotationDirection.Up;
            currentMoveFireDir = RotationDirection.Right;

            moveToConveyorTw = moveToConveyorSq.Append(
                transform.DOJump(endPos, moveToConveyorJumpForce, 1, moveToConveyorDuration)).SetEase(moveToConveyorEase).OnComplete(() =>
            {
                Vector3 newRotation = GetTurnDirection(RotationDirection.Right);
                transform.DORotate(newRotation, 0f);

                callback?.Invoke();
                GunPos = GunPos.ON_CONVEYOR;
                GetTargetBock();

            });
            moveToConveyorSq.Join(transform.DOScale(Vector3.one * 0.85f, moveToConveyorDuration));
            moveToConveyorSq.Append(transform.DOPunchScale(Vector3.one * 0.2f, moveToSlotDuration));
            moveToConveyorSq.SetId(this);

        }

        public void MoveToSlot(Vector3 endPos, Action callback = null)
        {
            Sequence moveToSlotSq = DOTween.Sequence();

            GunPos = GunPos.ON_SLOT;
            moveToSlotTw = moveToSlotSq.Append(transform.DOJump(endPos, 3, 1, moveToSlotDuration)).SetEase(Ease.Linear).OnComplete(() =>
            {
                callback?.Invoke();
                PlayAnim(Constant.GunAnimation.IDLE);
            });
            moveToSlotSq.Join(transform.DORotate(Vector3.zero, moveToSlotDuration));
            moveToSlotSq.Join(transform.DOScale(Vector3.one, moveToSlotDuration));
            moveToSlotSq.Append(transform.DOPunchScale(Vector3.one * 0.2f, moveToSlotDuration));
            moveToSlotSq.SetId(this);
        }

        public void MoveToBonusSlot(Vector3 endPos, Action callback = null)
        {
            Sequence moveToSlotSq = DOTween.Sequence();

            GunPos = GunPos.ON_BONUS_SLOT;
            moveToSlotTw = moveToSlotSq.Append(transform.DOJump(endPos, 3, 1, moveToSlotDuration)).SetEase(Ease.Linear).OnComplete(() =>
            {
                callback?.Invoke();
                PlayAnim(Constant.GunAnimation.IDLE);
            });
            moveToSlotSq.Join(transform.DORotate(Vector3.zero, moveToSlotDuration));
            moveToSlotSq.SetId(this);
        }

        public void MoveSortSlot(Vector3 targetPos, float _shiftDuration, Ease _shiftEase, GunPos gunPosOnDone)
        {
            GunPos = GunPos.TWEEN_SORT;
            Sequence moveSortSlotSq = DOTween.Sequence();

            moveSortSlotTw = moveSortSlotSq.Append(transform.DOMove(targetPos, _shiftDuration).SetEase(_shiftEase)).OnComplete(() =>
            {
                GunPos = gunPosOnDone;
            });
            moveSortSlotSq.SetId(this);
        }

        public override void MoveColumn(Vector3 targetPos, float _shiftDuration, Ease _shiftEase)
        {
            Sequence moveSortSlotSq = DOTween.Sequence();
            moveSortSlotTw.Kill();
            moveSortSlotTw = moveSortSlotSq.Append(transform.DOMove(targetPos, _shiftDuration).SetEase(_shiftEase)).OnComplete(() =>
            {
            });
            moveSortSlotSq.SetId(this);
        }

        #endregion

        public void Scale(Vector3 scaleTarget, float duration)
        {
            Sequence scaleSq = DOTween.Sequence();
            scaleSq.Append(transform.DOScale(scaleTarget, duration));
            scaleSq.SetId(this);
        }

        public bool CheckCanDisappear()
        {
            if (ConnectedGuns.Count <= 0)
            {
                return true;
            }
            else
            {
                for (int i = 0; i < ConnectedGuns.Count; i++)
                {
                    if (ConnectedGuns[i].BulletCount > 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool IsBlockInShootingAngle(Block block)
        {
            Vector3 gunPos = transform.position;
            Vector3 blockPos = block.transform.position;

            gunPos.y = 0;
            blockPos.y = 0;

            Vector3 toBlock = blockPos - gunPos;
            Vector3 shootDir = GetFireDirection(currentFireDir);

            shootDir.y = 0;
            shootDir.Normalize();

            float angle = Vector3.Angle(shootDir, toBlock);

            bool isInAngle = angle <= maxShootingAngle;
            return isInAngle;
        }

        public void PlayAnim(string name)
        {
            anim.PlayAnim(name);
        }

        public void OnGunClicked(Gun gun)
        {
            gun.PlayAnim(Constant.GunAnimation.CLICK);
            if (!gun.CanPushToConveyor())
            {
                return;
            }

            switch (gun.GunPos)
            {
                case GunPos.ON_GUN_BOARD:
                    LevelController.Instance.GunBoardController.OnTapGun(gun);
                    break;
                case GunPos.ON_SLOT:
                    LevelController.Instance.SlotController.OnTapGun(gun);
                    break;
                case GunPos.ON_BONUS_SLOT:
                    LevelController.Instance.BonusSlotController.OnTapGun(gun);
                    break;
                default:
                    break;
            }
        }

        public void OnRevive()
        {
            foreach (Block block in targetQueue)
            {
                block.OnGunRevive();
            }
            BulletRayCount = BulletCount;
            targetQueue.Clear();
        }
    }
    public enum GunPos
    {
        ON_GUN_BOARD = 0,
        ON_SLOT = 1,
        ON_CONVEYOR = 2,
        TWEEN_SORT = 3,
        ON_BONUS_SLOT = 4,
    }
}
