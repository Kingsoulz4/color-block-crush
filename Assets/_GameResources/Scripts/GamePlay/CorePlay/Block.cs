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
        Stone
    }

    public class Block : MonoBehaviour
    {
        public Action<Block> OnBlockInitialized;
        public Action<int, Block> OnBlockTakeDamage;
        public Action<Block> OnBlockDestroyed;

        [Header("Renderer References")]
        [SerializeField] protected Transform centerPoint;
        [SerializeField] private Renderer _blockMeshRenderer;
        [SerializeField] private ListMaterialsByColor colorRef;
        [SerializeField] protected Collider mCollider;
        public SerializedDictionary<int, int> colorRate;

        protected int maxHitPoint;
        protected int hitPoint;
        protected int hitPointRaycast;
        protected Vector3 originScale;
        protected int id;
        private BlockConfig blockData;
        public BlockType BlockType { get; private set; }
        public ColorType ColorType { get; protected set; }
        public GridNode GridNode { get; set; }
        public bool CanDestroy { get; private set; }
        public bool IsDestroyed { get; set; }
        public Vector2Int Size { get; set; } = new Vector2Int(1, 1);

        public BlockConfig BlockData { get => blockData; }
        public int Id { get => id; }

        public void Init(BlockType blockType, BlockConfig blockDataP)
        {
            BlockType = blockType;
            ColorType = blockDataP.colorType;
            id = blockDataP.id;
            maxHitPoint = 1;
            hitPoint = 1;
            hitPointRaycast = 1;
            blockData = blockDataP;
            originScale = transform.localScale;

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
                Material mat = colorRef.GetMaterial(ColorType, result);
                if (mat != null && _blockMeshRenderer.sharedMaterial != mat)
                    _blockMeshRenderer.sharedMaterial = mat;
            }
            else
            {
                _blockMeshRenderer.gameObject.SetActive(false);
                mCollider.enabled = false;
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
            hitPoint -= damageAmount;
            OnBlockTakeDamage?.Invoke(damageAmount, this);

            if (hitPoint <= 0)
            {
                
                DestroyBlock(() =>
                {
                    IsDestroyed = true;
                    gameObject.SetActive(false);
                    OnBlockDestroyed?.Invoke(this);
                });
            }
        }

        public virtual void OnGunRevive()
        {
            hitPointRaycast += 1;
            ChangeLayer(Constant.Layer.BLOCK);
        }

        public virtual void TakeDamageRaycast(int damageAmount)
        {
            if (hitPointRaycast > 0)
            {
                hitPointRaycast -= damageAmount;
                if (hitPointRaycast <= 0)
                {
                    this.Wait(Time.deltaTime, () =>
                    {
                        ChangeLayer(Constant.Layer.BLOCK_RAY0);
                    });
                }
            }
        }

        public void ChangeLayer(string name)
        {
            gameObject.layer = LayerMask.NameToLayer(name);
        }

        private void OnDisable()
        {
            DOTween.Kill(this);
        }

        private void DestroyBlock(Action callback = null)
        {
            var sq = DOTween.Sequence();
            var targetScale = originScale * 1.3f;
            targetScale.y = originScale.y * 1.5f;
            sq.Append(transform.DOScale(targetScale, 0.08f));
            sq.Append(transform.DOScale(0f, 0.06f).SetEase(Ease.InQuint));
            sq.OnComplete(() =>
            {
                callback?.Invoke();
            });
            sq.SetId(this);
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