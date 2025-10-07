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
        Stone
    }

    public class Block : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private TextMesh _hpText;

        [Header("Animation Settings")]
        public static float MoveSpeed = 5f;
        public static Ease MoveEase = Ease.OutQuad;

        // Properties
        public int CurrentHP { get; private set; }
        public int MaxHP { get; private set; }
        public ColorType Color { get; private set; }
        public BlockType Type { get; private set; }
        public GridNode CurrentNode { get; set; }

        // Stack children (blocks above this one on Z-axis)
        public List<Block> StackChildren { get; private set; }

        // Events
        public Action<Block> OnBlockDestroyed;
        public Action<Block> OnMovementStarted;
        public Action<Block> OnMovementCompleted;

        // State
        public bool IsMoving { get; private set; }
        private Sequence _moveSequence;

        private void Awake()
        {
            StackChildren = new List<Block>();
        }

        public void Initialize(BlockType type, ColorType color, int hp)
        {
            Type = type;
            Color = color;
            MaxHP = hp;
            CurrentHP = hp;

            UpdateVisuals();
        }

        public void TakeDamage(int damage)
        {
            if (Type == BlockType.Stone) return;

            CurrentHP -= damage;
            UpdateHPDisplay();

            if (CurrentHP <= 0)
            {
                DestroyBlock();
            }
        }

        private void DestroyBlock()
        {
            OnBlockDestroyed?.Invoke(this);

            // Handle stack children
            if (StackChildren.Count > 0)
            {
                Block newBase = StackChildren[0];
                StackChildren.RemoveAt(0);

                newBase.StackChildren.AddRange(StackChildren);
                StackChildren.Clear();

                newBase.CurrentNode = CurrentNode;
                newBase.transform.SetParent(null);
            }

            Destroy(gameObject);
        }

        public void StartMoveToNodeSequence(List<GridNode> pathNodes, Action onComplete = null)
        {
            if (pathNodes == null || pathNodes.Count == 0) return;

            StopMoveSequence();

            IsMoving = true;
            OnMovementStarted?.Invoke(this);

            _moveSequence = DOTween.Sequence();

            foreach (GridNode node in pathNodes)
            {
                Vector3 targetPos = node.transform.position;
                float distance = Vector3.Distance(transform.position, targetPos);
                float duration = distance / MoveSpeed;

                _moveSequence.Append(transform.DOMove(targetPos, duration).SetEase(MoveEase));
            }

            _moveSequence.OnComplete(() =>
            {
                IsMoving = false;
                CurrentNode = pathNodes[pathNodes.Count - 1];
                OnMovementCompleted?.Invoke(this);
                onComplete?.Invoke();
            });
        }

        private void StopMoveSequence()
        {
            if (_moveSequence != null && _moveSequence.IsActive())
            {
                _moveSequence.Kill();
            }
        }

        public void AddStackChild(Block child)
        {
            StackChildren.Add(child);
            child.transform.SetParent(transform);

            float zOffset = 0.5f * StackChildren.Count;
            child.transform.localPosition = new Vector3(0, 0, zOffset);
        }

        public Block GetTopMostBlock()
        {
            if (StackChildren.Count > 0)
                return StackChildren[StackChildren.Count - 1];
            return this;
        }

        private void UpdateVisuals()
        {
            if (_meshRenderer != null)
            {
                Material mat = _meshRenderer.material;
                mat.color = GetColorFromType(Color);
            }

            UpdateHPDisplay();
        }

        private void UpdateHPDisplay()
        {
            if (_hpText != null && Type != BlockType.Stone)
            {
                _hpText.text = CurrentHP.ToString();
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

        private void OnDestroy()
        {
            StopMoveSequence();
        }
    }
}