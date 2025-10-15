using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public enum RotationDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    public class ChangeRotationBox : MonoBehaviour
    {
        [SerializeField] RotationDirection direction;
        [SerializeField] RotationDirection directionNonFire;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constant.Tag.GUN))
            {
                if (other.TryGetComponent(out Gun gun))
                {
                    gun.Turn(direction, directionNonFire);
                    gun.CurrentFireDir = direction;
                }
            }
        }
    }
}
