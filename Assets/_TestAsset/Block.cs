using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ColorBlockCrush
{
    public enum ColorType
    {
        Red,
        Blue,
        Green,
        Yellow,
        Purple
    }

    public enum BlockType
    {
        Normal,
    }

    public class Block : MonoBehaviour, IAttackable
    {
        public Action<Block> OnBlockInitialized;
        public Action<int, Block> OnBlockTookNowDamage;
        public Action<int, Block> OnBlockTookNormalDamage;
        public Action<Block> OnBlockMovementStarted;
        public Action<Block> OnBlockMovementCompleted;
        public Action<Block> OnBlockDestroyStarted;
        public Action<Block> OnBlockDestroyCompleted;

        [Header("Renderer References")]
        [SerializeField] protected List<MeshRenderer> colorableBlockMeshRenderersList;

        [Header("Transform References")]
        [SerializeField] protected Transform contentTransform;
        [SerializeField] protected Transform BlockContentTransform;
        [SerializeField] protected Transform pushContentTransform;
        [SerializeField] protected Transform damagePointTransform;
        [SerializeField] protected Transform raycastPointTransform;

        [Header("Collider References")]
        [SerializeField] protected Collider collider;
        [SerializeField] protected Collider trigger;

        [Header("Variables")]
        [SerializeField] private float maxStrength = 1f;
        [SerializeField] private float maxPushOffset = 1.5f;
        [SerializeField] private float maxTiltAngle = 10f;
        [SerializeField] private float lerpSpeed = 15f;
        [SerializeField] private float lerpSpeedSecond = 5f;
        [SerializeField] private bool pushX = true;
        [SerializeField] private bool pushZ = true;
        [SerializeField] private bool tiltX = true;
        [SerializeField] private bool tiltY = true;
        [SerializeField] private bool tiltZ = true;

        protected int maxHitPointAmount;
        protected int NowHitPointAmount;
        protected int NormalHitPointAmount;

        private Vector3 initialLocalPos;
        private Quaternion initialLocalRot;
        private Vector3 targetLocalPos;
        private Quaternion targetLocalRot;

        private Sequence moveToGridNodeSequence;
        private Sequence moveToGridHeightSequence;
        protected Sequence destroySequence;

        private List<GridNode> moveToGridNodePathGridNodesList;
        private List<Transform> pushingBalls;

        public static float BlockMoveToGridNodeSpeed = 5f;
        public static float BlockDestroyDuration = 0.3f;
        public static float BlockMoveToGridHeightDuration = 0.2f;
        public static Ease BlockMoveToGridNodeEase = Ease.OutQuad;
        public static Ease BlockMoveToGridHeightEase = Ease.OutQuad;
        public static Ease BlockDestroyEase = Ease.InOutQuad;

        public BlockType BlockType { get; private set; }
        public ColorType ColorType { get; protected set; }
        public GridNode GridNode { get; set; }
        public int GridHeight { get; set; }
        public bool IsStatic { get; private set; }
        public bool CanDestroy { get; private set; }
        public bool IsAttacked { get; protected set; }
        public bool IsInIdleState => !IsMoving() && !IsDestroying();

        // Stack children
        public List<Block> StackChildren { get; private set; }

        private void Awake()
        {
            InitializeVariables();
        }

        private void InitializeVariables()
        {
            StackChildren = new List<Block>();
            pushingBalls = new List<Transform>();
            moveToGridNodePathGridNodesList = new List<GridNode>();

            if (pushContentTransform != null)
            {
                initialLocalPos = pushContentTransform.localPosition;
                initialLocalRot = pushContentTransform.localRotation;
            }

            targetLocalPos = initialLocalPos;
            targetLocalRot = initialLocalRot;
        }

        private void Update()
        {
            UpdatePushAndTilt();
        }

        protected virtual void OnDisable()
        {
            StopAllAnimations();
        }

        public void Initialize(BlockType blockType, ColorType colorType, int hitPointAmount, bool isStatic, bool canDestroy)
        {
            BlockType = blockType;
            ColorType = colorType;
            maxHitPointAmount = hitPointAmount;
            NowHitPointAmount = hitPointAmount;
            NormalHitPointAmount = hitPointAmount;
            IsStatic = isStatic;
            CanDestroy = canDestroy;
            IsAttacked = false;

            StartBlock();
            OnBlockInitialized?.Invoke(this);
        }

        protected virtual void StartBlock()
        {
            UpdateColors();
        }

        protected virtual void UpdateColors()
        {
            if (colorableBlockMeshRenderersList == null) return;

            Color color = GetColorFromType(ColorType);

            foreach (MeshRenderer renderer in colorableBlockMeshRenderersList)
            {
                if (renderer != null)
                {
                    renderer.material.color = color;
                }
            }
        }

        private Color GetColorFromType(ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.Red: return Color.red;
                case ColorType.Blue: return Color.blue;
                case ColorType.Green: return Color.green;
                case ColorType.Yellow: return Color.yellow;
                case ColorType.Purple: return new Color(0.5f, 0, 0.5f);
                default: return Color.white;
            }
        }

        public virtual void TakeDamageNow(int damageAmount)
        {
            if (!CanDestroy) return;

            NowHitPointAmount -= damageAmount;
            OnBlockTookNowDamage?.Invoke(damageAmount, this);

            if (NowHitPointAmount <= 0)
            {
                StartDestroySequence();
            }
        }

        public virtual void TakeDamageNormal(int damageAmount)
        {
            if (!CanDestroy) return;

            NormalHitPointAmount -= damageAmount;
            OnBlockTookNormalDamage?.Invoke(damageAmount, this);

            if (NormalHitPointAmount <= 0)
            {
                StartDestroySequence();
            }
        }

        public int GetMaxHitPoint()
        {
            return maxHitPointAmount;
        }

        public int GetNowHitPoint()
        {
            return NowHitPointAmount;
        }

        public int GetNormalHitPoint()
        {
            return NormalHitPointAmount;
        }

        public void RegisterPush(Transform ball)
        {
            if (!pushingBalls.Contains(ball))
            {
                pushingBalls.Add(ball);
            }
        }

        public void UnregisterPush(Transform ball)
        {
            pushingBalls.Remove(ball);
        }

        private void UpdatePushAndTilt()
        {
            if (pushContentTransform == null || pushingBalls.Count == 0)
            {
                // Lerp back to initial position/rotation
                targetLocalPos = initialLocalPos;
                targetLocalRot = initialLocalRot;
            }
            else
            {
                Transform nearestBall = GetNearestBall();
                if (nearestBall != null)
                {
                    Vector3 direction = transform.position - nearestBall.position;
                    direction.y = 0;
                    direction.Normalize();

                    float distance = Vector3.Distance(transform.position, nearestBall.position);
                    float strength = Mathf.Clamp01(1f - (distance / maxStrength));

                    // Calculate push offset
                    Vector3 pushOffset = Vector3.zero;
                    if (pushX) pushOffset.x = direction.x * maxPushOffset * strength;
                    if (pushZ) pushOffset.z = direction.z * maxPushOffset * strength;

                    targetLocalPos = initialLocalPos + pushOffset;

                    // Calculate tilt
                    Vector3 tiltEuler = Vector3.zero;
                    if (tiltX) tiltEuler.x = -direction.z * maxTiltAngle * strength;
                    if (tiltZ) tiltEuler.z = direction.x * maxTiltAngle * strength;

                    targetLocalRot = initialLocalRot * Quaternion.Euler(tiltEuler);
                }
            }

            // Smooth lerp
            pushContentTransform.localPosition = Vector3.Lerp(
                pushContentTransform.localPosition,
                targetLocalPos,
                lerpSpeed * Time.deltaTime
            );

            pushContentTransform.localRotation = Quaternion.Lerp(
                pushContentTransform.localRotation,
                targetLocalRot,
                lerpSpeedSecond * Time.deltaTime
            );
        }

        private Transform GetNearestBall()
        {
            if (pushingBalls.Count == 0) return null;

            Transform nearest = pushingBalls[0];
            float minDistance = Vector3.Distance(transform.position, nearest.position);

            for (int i = 1; i < pushingBalls.Count; i++)
            {
                float distance = Vector3.Distance(transform.position, pushingBalls[i].position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = pushingBalls[i];
                }
            }

            return nearest;
        }

        public void StartMoveToGridNodeSequence(List<GridNode> pathGridNodes, bool moveToGridHeight = true, bool disableMoveToGridHeightForFirstGridNode = false, Action onComplete = null)
        {
            if (pathGridNodes == null || pathGridNodes.Count == 0) return;

            StopMoveToGridNodeSequence();

            moveToGridNodePathGridNodesList = new List<GridNode>(pathGridNodes);

            OnBlockMovementStarted?.Invoke(this);

            moveToGridNodeSequence = DOTween.Sequence();

            for (int i = 0; i < pathGridNodes.Count; i++)
            {
                GridNode node = pathGridNodes[i];
                Vector3 targetPos = node.transform.position;

                // Add height offset if needed
                if (moveToGridHeight && !(i == 0 && disableMoveToGridHeightForFirstGridNode))
                {
                    targetPos.y += GridHeight * 0.5f; // Adjust based on stack height
                }

                float distance = Vector3.Distance(transform.position, targetPos);
                float duration = distance / BlockMoveToGridNodeSpeed;

                moveToGridNodeSequence.Append(
                    transform.DOMove(targetPos, duration).SetEase(BlockMoveToGridNodeEase)
                );
            }

            moveToGridNodeSequence.OnComplete(() =>
            {
                GridNode = pathGridNodes[pathGridNodes.Count - 1];
                OnBlockMovementCompleted?.Invoke(this);
                onComplete?.Invoke();
            });
        }

        public void StartMoveToGridNodeSequence(bool moveToGridHeight = true, bool disableMoveToGridHeightForFirstGridNode = false, Action onComplete = null)
        {
            if (GridNode == null) return;

            List<GridNode> path = new List<GridNode> { GridNode };
            StartMoveToGridNodeSequence(path, moveToGridHeight, disableMoveToGridHeightForFirstGridNode, onComplete);
        }

        public void StopMoveToGridNodeSequence()
        {
            if (moveToGridNodeSequence != null && moveToGridNodeSequence.IsActive())
            {
                moveToGridNodeSequence.Kill();
            }
        }

        public void StartMoveToGridHeightSequence()
        {
            StopMoveToGridHeightSequence();

            if (GridNode == null) return;

            Vector3 targetPos = GridNode.transform.position;
            targetPos.y += GridHeight * 0.5f;

            moveToGridHeightSequence = DOTween.Sequence();
            moveToGridHeightSequence.Append(
                transform.DOMove(targetPos, BlockMoveToGridHeightDuration).SetEase(BlockMoveToGridHeightEase)
            );
        }

        private void StopMoveToGridHeightSequence()
        {
            if (moveToGridHeightSequence != null && moveToGridHeightSequence.IsActive())
            {
                moveToGridHeightSequence.Kill();
            }
        }

        public virtual void StartDestroySequence()
        {
            if (IsDestroying()) return;

            OnBlockDestroyStarted?.Invoke(this);

            destroySequence = DOTween.Sequence();

            // Scale down animation
            destroySequence.Append(
                transform.DOScale(Vector3.zero, BlockDestroyDuration).SetEase(BlockDestroyEase)
            );

            destroySequence.OnComplete(() =>
            {
                OnBlockDestroyCompleted?.Invoke(this);

                // Handle stack children
                if (StackChildren.Count > 0)
                {
                    Block newBase = StackChildren[0];
                    StackChildren.RemoveAt(0);

                    newBase.StackChildren.AddRange(StackChildren);
                    StackChildren.Clear();

                    newBase.GridNode = GridNode;
                    newBase.transform.SetParent(null);
                }

                Destroy(gameObject);
            });
        }

        private void StopDestroySequence()
        {
            if (destroySequence != null && destroySequence.IsActive())
            {
                destroySequence.Kill();
            }
        }

        protected virtual void StopAllAnimations()
        {
            StopMoveToGridNodeSequence();
            StopMoveToGridHeightSequence();
            StopDestroySequence();
        }

        public bool GetIsAttacked()
        {
            return IsAttacked;
        }

        public Vector3 GetRaycastPointPosition()
        {
            return raycastPointTransform != null ? raycastPointTransform.position : transform.position;
        }

        public Vector3 GetDamagePointPosition()
        {
            return damagePointTransform != null ? damagePointTransform.position : transform.position;
        }

        public Vector3 GetTransformPosition()
        {
            return transform.position;
        }

        public Block GetTopMostBlock()
        {
            if (StackChildren.Count > 0)
                return StackChildren[StackChildren.Count - 1];
            return this;
        }

        public void AddStackChild(Block child)
        {
            StackChildren.Add(child);
            child.transform.SetParent(transform);

            float zOffset = 0.5f * StackChildren.Count;
            child.transform.localPosition = new Vector3(0, 0, zOffset);
            child.GridHeight = StackChildren.Count;
        }

        private bool IsMoving()
        {
            return (moveToGridNodeSequence != null && moveToGridNodeSequence.IsActive()) ||
                   (moveToGridHeightSequence != null && moveToGridHeightSequence.IsActive());
        }

        private bool IsDestroying()
        {
            return destroySequence != null && destroySequence.IsActive();
        }
    }
}