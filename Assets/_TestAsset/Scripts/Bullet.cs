using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace ColorBlockCrush
{
    public class Bullet : MonoBehaviour
    {
        protected Gun gun;
        protected Block block;
        protected Action<Gun, Block> onHit;

        private float speed = 25f;
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
                    onHit?.Invoke(gun, block);
                    isMoving = false;
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
