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
        protected Action<Gun, Block> onHit;
        // set bullet data for bullet
        public virtual void OnInit(Gun gunP, Action<Gun, Block> onHitP)
        {
            gun = gunP;
            onHit = onHitP;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constant.Tag.BLOCK))
            {
                other.TryGetComponent(out Block block);
                if (block)
                {
                    onHit?.Invoke(gun, block);
                }
            }
        }
    }
}
