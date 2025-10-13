using ColorBlockCrush.Tools;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace ColorBlockCrush
{

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
        [SerializeField] protected Transform centerPoint;
        [SerializeField] private MeshRenderer _blockMeshRenderer;
        [SerializeField] private ColorReference colorRef;

        protected int maxHitPoint;
        protected int hitPoint;
        private int id;
        private BlockConfig blockData;
        public BlockType BlockType { get; private set; }
        public ColorType ColorType { get; protected set; }
        public GridNode GridNode { get; set; }
        public Gun Gun { get; set; }
        public int GridHeight { get; set; }
        public bool CanDestroy { get; private set; }
        public bool IsAttacked { get; set; }
        public BlockConfig BlockData { get => blockData;}
        public int Id { get => id;}

        public void Init(BlockType blockType, BlockConfig blockDataP, bool isStatic, bool canDestroy)
        {
            BlockType = blockType;
            ColorType = blockDataP.colorType;
            id = blockDataP.id;
            maxHitPoint = 1;
            hitPoint = 1;
            CanDestroy = canDestroy;
            IsAttacked = false;
            blockData = blockDataP;
            Gun = null;

            StartBlock();
            OnBlockInitialized?.Invoke(this);
        }

        protected virtual void StartBlock()
        {
            UpdateColors(ColorType);
        }

        protected virtual void UpdateColors(ColorType colorType)
        {
            if (_blockMeshRenderer != null && colorRef != null)
            {
                _blockMeshRenderer.material.color = colorRef.GetColor(colorType);
            }
        }

        public virtual void TakeDamage(int damageAmount)
        {
            if (!CanDestroy) return;

            hitPoint -= damageAmount;
            OnBlockTakeDamage?.Invoke(damageAmount, this);

            if (hitPoint <= 0)
            {
                Destroy(gameObject);
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

        public Vector3 GetDamagePointPosition()
        {
            return centerPoint != null ? centerPoint.position : transform.position;
        }
    }
}