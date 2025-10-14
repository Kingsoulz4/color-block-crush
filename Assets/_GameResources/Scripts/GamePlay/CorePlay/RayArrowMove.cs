using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class RayArrowMove : MonoBehaviour
    {
        [SerializeField] private MeshRenderer mesh;
        [SerializeField] private Vector2 scrollSpeed = new Vector2(0f, 0.5f);

        private Material mat;
        private Vector2 offset;

        void Start()
        {
            mat = mesh.material;
        }

        void Update()
        {
            offset += scrollSpeed * Time.deltaTime;
            mat.mainTextureOffset = offset;
        }
    }
}