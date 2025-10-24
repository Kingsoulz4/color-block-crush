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

        private List<GameObject> listBodyPartDestroyed = new();

        private List<Block> listBlockPlace = new();

        private int axis = 0;

        private Vector2Int direction;

        private Vector2Int DefaultSize { get; set; } 

        public void Init(PixelSnakeConfig pixelSnakeConfig)
        {
            blockBarierData = pixelSnakeConfig;
            ColorType = pixelSnakeConfig.colorType;
            this.id = pixelSnakeConfig.pixelSnakeId;
            maxHitPoint = pixelSnakeConfig.health;
            hitPoint = pixelSnakeConfig.health;
            hitPointRaycast = pixelSnakeConfig.health;
            
            SpawnBody();
            UpdateCollider();

            StartBlock();
            OnBlockInitialized?.Invoke(this);
        }    

        private void SpawnBody()
        {
            m_headPart.GetComponent<MeshRenderer>().material = colorRef.listMaterial[ColorType].First();
            float angle = 0;

            var headBlock = LevelManager.Instance.LevelGame.BlockBoardController.GetBlockByID(blockBarierData.headBlockId);
            var minX = headBlock.transform.position.x;
            var maxX = headBlock.transform.position.x;
            var minZ = headBlock.transform.position.z;
            var maxZ = headBlock.transform.position.z;

            List<float> listPartPosX = new();
            List<float> listPartPosZ = new();

            for (int i=0; i<blockBarierData.blocksId.Count; i++)
            {
                var block = LevelManager.Instance.LevelGame.BlockBoardController.GetBlockByID(blockBarierData.blocksId[i]);
                listBlockPlace.Add(block);
                minX = Mathf.Min(minX, block.transform.position.x);
                maxX = Mathf.Max(maxX, block.transform.position.x);
                minZ = Mathf.Min(minZ, block.transform.position.z);
                maxZ = Mathf.Max(maxZ, block.transform.position.z);

                if(!listPartPosX.Contains(block.transform.position.x))
                {
                    listPartPosX.Add(block.transform.position.x);
                }

                if (!listPartPosZ.Contains(block.transform.position.z))
                {
                    listPartPosZ.Add(block.transform.position.z);
                }
            }

            Size = CalculateSize();

            DefaultSize = Size;

            if(Size.x == 2)
            {
                if(maxZ > headBlock.transform.position.z)
                {
                    direction = Vector2Int.up;
                }
                else
                {
                    direction = Vector2Int.down;
                }
            }
            else
            {
                if(maxX > headBlock.transform.position.x)
                {
                    direction = Vector2Int.right;
                }
                else
                {
                    direction = Vector2Int.left;
                }
            }

            if(direction == Vector2Int.right)
            {
                angle = 180;
            }
            else if(direction == Vector2Int.up)
            {
                angle = 90;
            }
            else if(direction == Vector2Int.down)
            {
                angle = -90;
            }
            else
            {
                angle = 0;
            }


            float commonPos;


            if(direction == Vector2Int.down || direction == Vector2Int.up)
            {
                commonPos = (minX + maxX) / 2;

                listPartPosZ.Sort();

                if (direction == Vector2Int.down)
                {
                    listPartPosZ.Reverse();
                }

                m_headPart.transform.position = new Vector3(commonPos, m_headPart.transform.position.y, (listPartPosZ[0] + listPartPosZ[1]) / 2);
                m_tailPart.transform.position = new Vector3(commonPos, m_tailPart.transform.position.y, listPartPosZ[^1]);

                bool isColor = true;

                for (int i = 2; i < listPartPosZ.Count - 1; i++)
                {
                    var newPart = Instantiate(m_bodyPart, transform);
                    listBodyPart.Add(newPart);
                    newPart.transform.position = new Vector3(commonPos, newPart.transform.position.y, listPartPosZ[i]);
                    newPart.gameObject.SetActive(true);

                    if (i%2 == 0)
                    {
                        newPart.GetComponent<MeshRenderer>().material = colorRef.listMaterial[ColorType].First();
                    }
                    else
                    {
                        newPart.GetComponent<MeshRenderer>().material = colorRef.listMaterial[ColorType.White].First();
                    }
                    newPart.transform.localRotation = Quaternion.Euler(0, angle, 0);
                }

                

            }
            else
            {
                commonPos = (minZ + maxZ) / 2;

                listPartPosX.Sort();

                if (direction == Vector2Int.left)
                {
                    listPartPosX.Reverse();
                }

                m_headPart.transform.position = new Vector3((listPartPosX[0] + listPartPosX[1]) / 2, m_headPart.transform.position.y, commonPos);
                m_tailPart.transform.position = new Vector3(listPartPosX[^1], m_headPart.transform.position.y, commonPos);

                bool isColor = true;

                for (int i = 2; i < listPartPosX.Count - 1; i++)
                {
                    var newPart = Instantiate(m_bodyPart, transform);
                    listBodyPart.Add(newPart);
                    newPart.transform.position = new Vector3(listPartPosX[i], newPart.transform.position.y, commonPos);
                    newPart.gameObject.SetActive(true);

                    if (i % 2 == 0)
                    {
                        newPart.GetComponent<MeshRenderer>().material = colorRef.listMaterial[ColorType].First();
                    }
                    else
                    {
                        newPart.GetComponent<MeshRenderer>().material = colorRef.listMaterial[ColorType.White].First();
                    }
                    newPart.transform.localRotation = Quaternion.Euler(0, angle, 0);
                }

                
            }

            m_headPart.transform.localRotation = Quaternion.Euler(0, angle, 0);
            m_tailPart.transform.localRotation = Quaternion.Euler(0, angle, 0);


        }

        private Vector2Int CalculateSize()
        {
            var minX = int.MaxValue;
            var maxX = -1;
            var minY = int.MaxValue;
            var maxY = -1;
            for (int i = 0; i < blockBarierData.blocksId.Count; i++)
            {
                var block = LevelManager.Instance.LevelGame.BlockBoardController.GetBlockByID(blockBarierData.blocksId[i]);
                minX = Mathf.Min(block.BlockData.coordinate.x, minX);
                maxX = Mathf.Max(block.BlockData.coordinate.x, maxX);
                minY = Mathf.Min(block.BlockData.coordinate.y, minY);
                maxY = Mathf.Max(block.BlockData.coordinate.y, maxY);
            }

            return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
        }

        public override Vector3 GetTargetHitBullet()
        {
            return m_headPart.transform.position;
        }

        private void ResizeBlock()
        {
            var partCount = DefaultSize.x - (maxHitPoint - hitPoint) / (maxHitPoint / DefaultSize.x);
            if (direction == Vector2Int.up || direction == Vector2Int.down)
            {
                partCount = DefaultSize.y - (maxHitPoint - hitPoint) / (maxHitPoint / DefaultSize.y);
            }

            if (partCount < listBodyPart.Count + 3 && listBodyPart.Count > 0)
            {
                
                if(direction == Vector2Int.up || direction == Vector2Int.down)
                {
                    var listBlockToDestroy = listBlockPlace.FindAll(x => Mathf.Abs(x.transform.position.z - m_tailPart.transform.position.z) <= float.Epsilon);
                    listBlockToDestroy.ForEach(x => {
                        x.TakeDamage(1);
                        listBlockPlace.Remove(x);
                        x.DestroyBlock();
                    });
                }    
                else
                {
                    var listBlockToDestroy = listBlockPlace.FindAll(x => Mathf.Abs(x.transform.position.x - m_tailPart.transform.position.x) <= float.Epsilon);
                    listBlockToDestroy.ForEach(x => {
                        x.TakeDamage(1);
                        listBlockPlace.Remove(x);
                        x.DestroyBlock();
                    });
                }

                var lastPartPosition = listBodyPart.Last().transform.position;
                m_tailPart.transform.position = new Vector3(lastPartPosition.x, m_tailPart.transform.position.y, lastPartPosition.z);
                listBodyPart[^1].SetActive(false);
                listBodyPartDestroyed.Add(listBodyPart[^1]);
                listBodyPart.RemoveAt(listBodyPart.Count - 1);
                UpdateCollider();
            }
            else if(hitPoint <= 0)
            {
                listBlockPlace.ForEach(x => {
                    x.DestroyBlock();
                });
            }    
            
        }

        private void UpdateCollider()
        {
            if (mCollider is BoxCollider boxCollider)
            {
                if (direction == Vector2Int.up || direction == Vector2Int.down)
                {
                    Size = new Vector2Int(Size.x, listBodyPart.Count + 2);
                    boxCollider.size = new Vector3(Size.x, boxCollider.size.y, Size.y);
                }
                else
                {
                    Size = new Vector2Int(listBodyPart.Count + 2, Size.y);
                    boxCollider.size = new Vector3(Size.x, boxCollider.size.y, Size.y);
                }

                if(hitPoint < maxHitPoint)
                boxCollider.center -= new Vector3(direction.x, 0, direction.y) * transform.localScale.x * 2;
            }
        }

        public override void TakeDamage(int damageAmount)
        {
            base.TakeDamage(damageAmount);
            ResizeBlock();
        }

        
    }
}
