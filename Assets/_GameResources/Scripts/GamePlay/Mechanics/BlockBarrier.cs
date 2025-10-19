using ColorBlockCrush.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ColorBlockCrush
{
    public class BlockBarrier : Block
    {
        [SerializeField] private GameObject m_bodyPart;
        [SerializeField] private GameObject m_headPart;
        [SerializeField] private GameObject m_tailPart;

        private PixelSnakeConfig blockBarierData;

        private List<GameObject> listBodyPart = new();

        public void Init(PixelSnakeConfig pixelSnakeConfig)
        {
            blockBarierData = pixelSnakeConfig;
            ColorType = pixelSnakeConfig.colorType;
            this.id = pixelSnakeConfig.pixelSnakeId;
            maxHitPoint = pixelSnakeConfig.health;
            hitPoint = pixelSnakeConfig.health;
            hitPointRaycast = pixelSnakeConfig.health;
            Size = new Vector2Int(blockBarierData.blocksId.Count,2);
            UpdateCollider();
            SpawnBody();

            StartBlock();
            OnBlockInitialized?.Invoke(this);
        }    

        private void SpawnBody()
        {
            var block0 = LevelManager.Instance.LevelGame.BlockBoardController.GetBlockByID(blockBarierData.blocksId[0]);
            var block1 = LevelManager.Instance.LevelGame.BlockBoardController.GetBlockByID(blockBarierData.blocksId[1]);

            var headPosX = (block0.transform.position.x + block1.transform.position.x)/2;
            var headPosZ = (block0.transform.position.z + block1.transform.position.z)/2;

            m_headPart.transform.position = new Vector3(headPosX, m_headPart.transform.position.y, headPosZ);

            for (int i = 2; i < blockBarierData.blocksId.Count - 1; i++)
            {
                var block = LevelManager.Instance.LevelGame.BlockBoardController.GetBlockByID(blockBarierData.blocksId[i]);
                var newPart = Instantiate(m_bodyPart, transform);
                listBodyPart.Add(newPart);
                newPart.transform.position = block.transform.position;
                newPart.gameObject.SetActive(true);
            }

            var lastBlock = LevelManager.Instance.LevelGame.BlockBoardController.GetBlockByID(blockBarierData.blocksId[^1]);
            m_tailPart.transform.position = lastBlock.transform.position;
        }

        private void ResizeBlock()
        {
            m_tailPart.transform.position = listBodyPart.Last().transform.position;
            Destroy(listBodyPart[^1]);
            listBodyPart.RemoveAt(listBodyPart.Count -1);
            UpdateCollider();
            
        }

        private void UpdateCollider()
        {
            if (mCollider is BoxCollider boxCollider)
            {
                boxCollider.size = new Vector3(Size.x, boxCollider.size.y, Size.y);
            }
        }

        public override void TakeDamage(int damageAmount)
        {
            base.TakeDamage(damageAmount);
            ResizeBlock();
        }

        
    }
}
