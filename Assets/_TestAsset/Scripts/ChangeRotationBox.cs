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
        [SerializeField] float rotateDuration;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Gun"))
            {
                if (TryGetComponent(out Gun gun))
                {
                    gun.Turn(direction, rotateDuration);
                }
            }
        }
    }
}
