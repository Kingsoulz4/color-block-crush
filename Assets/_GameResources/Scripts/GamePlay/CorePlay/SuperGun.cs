using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class SuperGun : MonoBehaviour
    {
        [SerializeField] private Transform spawnBulletPos;

        public Transform SpawnBulletPos { get => spawnBulletPos;}
    }
}
