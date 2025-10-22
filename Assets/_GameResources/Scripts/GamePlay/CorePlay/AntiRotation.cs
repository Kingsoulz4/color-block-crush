using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class AntiRotation : MonoBehaviour
    {
        private Quaternion _initialWorldRot;

        void Awake() => _initialWorldRot = transform.rotation;

        void LateUpdate()
        {
            transform.rotation = _initialWorldRot; // B không xoay theo A
        }
    }
}
