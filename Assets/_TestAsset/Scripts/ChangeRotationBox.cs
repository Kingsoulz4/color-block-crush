using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class ChangeRotationBox : MonoBehaviour
    {
        enum RotationDirection
        {
            Up,
            Down,
            Left,
            Right
        }

        [SerializeField] RotationDirection direction;
        [SerializeField] float rotateDuration;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Gun"))
            {
                Vector3 currentRotation = other.transform.eulerAngles;
                Vector3 newRotation = currentRotation;
                switch (direction)
                {
                    case RotationDirection.Up:
                        newRotation = new Vector3(0, 0, 0);
                        break;
                    case RotationDirection.Down:
                        newRotation = new Vector3(0, 180, 0);
                        break;
                    case RotationDirection.Left:
                        newRotation = new Vector3(0, -90, 0);
                        break;
                    case RotationDirection.Right:
                        newRotation = new Vector3(0, 90, 0);
                        break;
                }

                other.transform.DORotate(newRotation, rotateDuration);
            }
        }
    }
}
