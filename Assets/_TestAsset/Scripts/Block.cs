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

    public class Block : MonoBehaviour
    {
        public Action<Block> OnBlockInitialized;
        public Action<int, Block> OnBlockTakeDamage;
        public Action<Block> OnBlockDestroyed;

        [Header("Renderer References")]
        [SerializeField] protected List<MeshRenderer> meshList;
        [SerializeField] protected Transform centerPoint;

        protected int maxHitPoint;
        protected int hitPoint;

        public BlockType BlockType { get; private set; }
        public ColorType ColorType { get; protected set; }
        public GridNode GridNode { get; set; }
        public int GridHeight { get; set; }
        public bool CanDestroy { get; private set; }
        public bool IsAttacked { get; protected set; }

        public void Initialize(BlockType blockType, ColorType colorType, int hitPointAmount, bool isStatic, bool canDestroy)
        {
            BlockType = blockType;
            ColorType = colorType;
            maxHitPoint = hitPointAmount;
            hitPoint = hitPointAmount;
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
            if (meshList == null) return;

            Color color = GetColorFromType(ColorType);

            foreach (MeshRenderer renderer in meshList)
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

        public virtual void TakeDamage(int damageAmount)
        {
            if (!CanDestroy) return;

            hitPoint -= damageAmount;
            OnBlockTakeDamage?.Invoke(damageAmount, this);

            if (hitPoint <= 0)
            {
            }
        }

        public int GetMaxHitPoint()
        {
            return maxHitPoint;
        }

        public int GetHitPoint()
        {
            return hitPoint;
        }

        public bool GetIsAttacked()
        {
            return IsAttacked;
        }

        public Vector3 GetDamagePointPosition()
        {
            return centerPoint != null ? centerPoint.position : transform.position;
        }
    }
}