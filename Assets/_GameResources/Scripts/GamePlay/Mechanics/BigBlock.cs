using ColorBlockCrush.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class BigBlock : MonoBehaviour
    {
        [SerializeField] private float sizeScaleFactor = 1f;

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

        public int CurrentHealth { get; set; }

        public void Init(BigBlockConfig bigBlockConfig)
        {
            bigBlockData = bigBlockConfig;
            
            
        }    

        private void ResizeBlock()
        {
            var blockSize = CalculateSize();
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
                var deltaY = ((blockSize.x - 1) * sizeScaleFactor) / 2;
                m_upTopLeft.transform.position += Vector3.forward * deltaY;
                m_upBotLeft.transform.position += Vector3.forward * deltaY;
                m_upTopRight.transform.position += Vector3.right * deltaY;
                m_upBotRight.transform.position += Vector3.right * deltaY;

                m_downTopLeft.transform.position += Vector3.back * deltaY;
                m_downBotLeft.transform.position += Vector3.back * deltaY;
                m_downTopRight.transform.position += Vector3.back * deltaY;
                m_downBotRight.transform.position += Vector3.forward * deltaY;

            }
        }

        private Vector2Int CalculateSize()
        {
            var minX = 0;
            var maxX = int.MaxValue;
            var minY = 0;
            var maxY = int.MaxValue;
            for(int i=0; i<bigBlockData.blocksId.Count; i++)
            {
                var block = LevelManager.Instance.LevelGame.BlockBoardController.GetBlockByID(bigBlockData.blocksId[i]);
                minX = Mathf.Min(block.BlockData.coordinate.x, minX);
                maxX = Mathf.Max(block.BlockData.coordinate.x, maxX);
                minY = Mathf.Min(block.BlockData.coordinate.y, minY);
                maxY = Mathf.Max(block.BlockData.coordinate.y, maxY);
            }

            return new Vector2Int(maxX - minX + 1, maxY - minY);
        }
    }
}
