using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class GunAnim : MonoBehaviour
    {
        [SerializeField] private Animator gunAnimator;

        public void PlayAnim(string name)
        {
            gunAnimator.Play(name);
        }

        public void StopAnim()
        {
            gunAnimator.StopPlayback();
        }
    }
}
