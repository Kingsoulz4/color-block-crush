using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class ResponsiveCam : MonoBehaviour
    {
        [SerializeField] private float _buffer = 0.5f;

        private Camera _cam;
        private void Awake()
        {
            _cam = GetComponent<Camera>();  
        }

        [ContextMenu("AutoResponsiveCam")]
        public void AutoResponsiveCam()
        {
            (Vector3 center, float size) = CalculateOrthoSize();
            _cam.transform.position = center;
            _cam.orthographicSize = size;
        }

        private (Vector3 center, float size) CalculateOrthoSize()
        {
            var bounds = new Bounds();
            foreach (var col in FindObjectsOfType <Collider2D> ()) bounds.Encapsulate(col.bounds);
            bounds.Expand(_buffer);
            var vertical = bounds.size.y;
            var horizontal = bounds.size.x * _cam.pixelHeight / _cam.pixelWidth;
            var size = Mathf.Max(horizontal, vertical) * 0.5f;
            var center = bounds.center + new Vector3(0, 0, -10);
            return (center, size);
        }
    }
}
