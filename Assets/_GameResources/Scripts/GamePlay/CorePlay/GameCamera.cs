using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class GameCamera : MonoBehaviour
    {
        Tween moveTween;
        public void MoveZ(float z, float duration)
        {
            if (moveTween != null) moveTween.Kill();
            moveTween = transform.DOMoveZ(z, duration);
        }
    }
}
