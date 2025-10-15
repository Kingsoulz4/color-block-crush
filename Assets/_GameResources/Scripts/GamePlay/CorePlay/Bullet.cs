using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace ColorBlockCrush
{
    public class Bullet : MonoBehaviour
    {
        protected Gun gun;
        protected Block block;
        protected Action<Gun, Block> onHit;

        [SerializeField] private float speed = 10;
        private Vector3 endPos;
        private bool isMoving = false;
        // set bullet data for bullet
        public virtual void OnInit(Gun gunP, Block blockP, Action<Gun, Block> onHitP)
        {
            gun = gunP;
            onHit = onHitP;
            block = blockP;
            var y = transform.position.y;
            endPos = blockP.transform.position;
            endPos.y = y;

            isMoving = true;
        }

        private void Update()
        {
            if (!isMoving)
            {
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constant.Tag.BLOCK))
            {
                other.TryGetComponent(out Block blockP);
                if (blockP && block == blockP)
                {
                    isMoving = false;
                    onHit?.Invoke(gun, block);
                }
            }
        }

        private void OnDisable()
        {
            gun = null;
            block = null;
            isMoving = false;
        }
    }
}
