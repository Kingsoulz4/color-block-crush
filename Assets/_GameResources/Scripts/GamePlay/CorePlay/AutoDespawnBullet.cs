using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class AutoDespawnBullet : MonoBehaviour
    {
        [SerializeField] private float normalDuration;

        private void OnEnable()
        {
            this.Wait(normalDuration, () =>
            {
                if (gameObject)
                {
                    Destroy(gameObject);
                }
            });
        }
    }
}
