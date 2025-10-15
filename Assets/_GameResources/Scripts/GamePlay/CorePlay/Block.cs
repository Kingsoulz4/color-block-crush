using AYellowpaper.SerializedCollections;
using ColorBlockCrush.Tools;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

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
        [SerializeField] private ListMaterialsByColor colorRef;
        public SerializedDictionary<int, int> colorRate;

        protected int maxHitPoint;
        protected int hitPoint;
        protected int hitPointRaycast;
        private int id;
        private BlockConfig blockData;
        public BlockType BlockType { get; private set; }
        public ColorType ColorType { get; protected set; }
        public GridNode GridNode { get; set; }
        public int GridHeight { get; set; }
        public bool CanDestroy { get; private set; }
        public bool IsAttacked { get; set; }
        public BlockConfig BlockData { get => blockData; }
        public int Id { get => id; }

        public void Init(BlockType blockType, BlockConfig blockDataP, bool isStatic, bool canDestroy)
        {
            BlockType = blockType;
            ColorType = blockDataP.colorType;
            id = blockDataP.id;
            maxHitPoint = 1;
            hitPoint = 1;
            hitPointRaycast = 1;
            CanDestroy = canDestroy;
            IsAttacked = false;
            blockData = blockDataP;

            StartBlock();
            OnBlockInitialized?.Invoke(this);
        }

        protected virtual void StartBlock()
        {
            UpdateColors(ColorType);
        }

        protected virtual void UpdateColors(ColorType colorType)
        {
            if (_blockMeshRenderer != null && colorRef != null && colorType != ColorType.None)
            {
                int result = GetRandomByRatio(colorRate);
                Material mat = colorRef.GetMaterial(ColorType, 0);
                if (mat != null && _blockMeshRenderer.sharedMaterial != mat)
                    _blockMeshRenderer.sharedMaterial = mat;
            }
        }

        int GetRandomByRatio(Dictionary<int, int> ratioMap)
        {
            int total = 0;
            foreach (var kv in ratioMap)
                total += kv.Value;

            int randomPoint = Random.Range(0, total);
            int cumulative = 0;

            foreach (var kv in ratioMap)
            {
                cumulative += kv.Value;
                if (randomPoint <= cumulative)
                    return kv.Key;
            }

            return 0;
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

        public virtual void TakeDamageRaycast(int damageAmount)
        {
            if (!CanDestroy) return;
            if (hitPointRaycast > 0)
            {
                hitPointRaycast -= damageAmount;
            }
        }

        public bool CanBeRaycastHit()
        {
            return hitPointRaycast > 0;
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