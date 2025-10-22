using ColorBlockCrush.Tools;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ColorBlockCrush
{
    public class BigBlock : Block
    {
        [SerializeField] private float sizeScaleFactor = 1f;
        [SerializeField] private TextMeshPro m_textHealth;

        [Header("Spine")]
        [SerializeField] private Transform m_upTopLeft;
        [SerializeField] private Transform m_upTopRight;
        [SerializeField] private Transform m_upBotLeft;
        [SerializeField] private Transform m_upBotRight;
        [SerializeField] private Transform m_downTopLeft;
        [SerializeField] private Transform m_downTopRight;
        [SerializeField] private Transform m_downBotLeft;
        [SerializeField] private Transform m_downBotRight;
        

        private BigBlockConfig bigBlockData;

        private List<Block> listBlockPlace = new();

        private Vector3 defaultScale;

        public void Init(BigBlockConfig bigBlockConfig)
        {
            bigBlockData = bigBlockConfig;
            //BlockType = blockType;
            ColorType = bigBlockConfig.colorType;
            this.id = bigBlockConfig.bigBlockId;
            maxHitPoint = bigBlockConfig.blockHealth;
            hitPoint = bigBlockConfig.blockHealth;
            hitPointRaycast = bigBlockConfig.blockHealth;
            //defaultScale = transform.localScale;

            for (int i = 0; i < bigBlockData.blocksId.Count; i++)
            {
                var block = LevelManager.Instance.LevelGame.BlockBoardController.GetBlockByID(bigBlockData.blocksId[i]);
                listBlockPlace.Add(block);
            }

            ResizeBlock();

            StartBlock();
            OnBlockInitialized?.Invoke(this);
            UpdateHeathText();
        }

        private void Start()
        {
            defaultScale = transform.localScale;
        }

        private void ResizeBlock()
        {
            var blockSize = CalculateSize();
            Size = new Vector2Int(blockSize.x, blockSize.y);

            if(blockSize.x > 1)
            {
                var deltaX = ((blockSize.x - 1) * sizeScaleFactor) / 2;
                m_upTopLeft.transform.position += Vector3.left * deltaX;
                m_upBotLeft.transform.position += Vector3.left * deltaX;
                m_downTopLeft.transform.position += Vector3.left * deltaX;
                m_downBotLeft.transform.position += Vector3.left * deltaX;

                m_upTopRight.transform.position += Vector3.right * deltaX;
                m_upBotRight.transform.position += Vector3.right * deltaX;
                m_downTopRight.transform.position += Vector3.right * deltaX;
                m_downBotRight.transform.position += Vector3.right * deltaX;

            }

            if(blockSize.y > 1)
            {
                var deltaY = ((blockSize.y - 1) * sizeScaleFactor) / 2;
                m_upTopLeft.transform.position += Vector3.forward * deltaY;
                m_upTopRight.transform.position += Vector3.forward * deltaY;
                m_downTopLeft.transform.position += Vector3.forward * deltaY;
                m_downTopRight.transform.position += Vector3.forward * deltaY;

                m_downBotLeft.transform.position += Vector3.back * deltaY;
                m_downBotRight.transform.position += Vector3.back * deltaY;
                m_upBotLeft.transform.position += Vector3.back * deltaY;
                m_upBotRight.transform.position += Vector3.back * deltaY;

            }

            if(mCollider is BoxCollider boxCollider)
            {
                boxCollider.size = new Vector3(blockSize.x, boxCollider.size.y, blockSize.y);
            }

            var textRect = (RectTransform)m_textHealth.transform;
            textRect.sizeDelta = new Vector2(Mathf.Max(Size.x, 1f), Mathf.Max(Size.y, 2f)) * 0.9f;
                
        }

        private Vector2Int CalculateSize()
        {
            var minX = int.MaxValue;
            var maxX = -1;
            var minY = int.MaxValue;
            var maxY = -1;
            for(int i=0; i<bigBlockData.blocksId.Count; i++)
            {
                var block = LevelManager.Instance.LevelGame.BlockBoardController.GetBlockByID(bigBlockData.blocksId[i]);
                minX = Mathf.Min(block.BlockData.coordinate.x, minX);
                maxX = Mathf.Max(block.BlockData.coordinate.x, maxX);
                minY = Mathf.Min(block.BlockData.coordinate.y, minY);
                maxY = Mathf.Max(block.BlockData.coordinate.y, maxY);
            }

            return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
        }

        public override void TakeDamage(int damageAmount)
        {
            base.TakeDamage(damageAmount);

            transform.DOKill();

            transform.DOScale(defaultScale * 1.1f, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                transform.DOScale(defaultScale, 0.1f).SetEase(Ease.OutBack);
            });

            UpdateHeathText();
            if(hitPoint<=0)
            {
                listBlockPlace.ForEach(x => x.OnBlockDestroyed?.Invoke(x));
            }
        }

        private void UpdateHeathText()
        {
            m_textHealth.text = hitPoint.ToString();
        }
    }
}
