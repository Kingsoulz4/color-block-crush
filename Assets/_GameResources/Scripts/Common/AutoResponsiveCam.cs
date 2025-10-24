using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ColorBlockCrush
{
    public class AutoResponsiveCam : MonoBehaviour
    {
        [SerializeField] private float _buffer;
        [SerializeField] private Camera _cam;
        public float defaultAspect = 9f / 21f; // reference aspect (portrait 9:21)
        public float defaultOrthoSize = 15f;     // default orthographic size
        public float minOrthoSize = 12;

        [ContextMenu("CalculateResponsiveCam")]
        public void CalculateResponsiveCam()
        {
            //(Vector3 center, float size) = CalculateOrthoSize();
            //_cam.orthographicSize = size;
            float currentAspect = (float)Screen.width / Screen.height;
            Debug.Log($"Width {Screen.width} Height {Screen.height}");
#if UNITY_EDITOR
            //currentAspect = (float)GetGameViewSize().x / GetGameViewSize().y;
#endif
            _cam.orthographicSize = Mathf.Clamp(defaultOrthoSize * (defaultAspect / currentAspect), minOrthoSize, defaultOrthoSize);

        }

        private void Update()
        {
            CalculateResponsiveCam();
        }

        private (Vector3 center, float size) CalculateOrthoSize()
        {
            var bounds = new Bounds();

            foreach (var col in FindObjectsOfType<Collider2D>()) bounds.Encapsulate(col.bounds);
            bounds.Expand(_buffer);
            var vertical = bounds.size.y;
            var horizontal = bounds.size.x * _cam.pixelHeight / _cam.pixelWidth;
            var size = Mathf.Max(horizontal, vertical) * 0.5f;
            var center = bounds.center + new Vector3(0, 0, -10);
            return (center, size);
        }

        private void OnValidate()
        {
            _cam = Camera.main; 
        }

        public Vector2Int GetGameViewSize()
        {
#if UNITY_EDITOR
            // Only works in the Editor
            var gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            var getMainGameView = gameViewType.GetMethod("GetMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
            var gameView = getMainGameView.Invoke(null, null);
            if (gameView == null)
            {
                // When running outside the Editor context
                return new Vector2Int(Screen.width, Screen.height);
            }

            var prop = gameView.GetType().GetProperty("currentGameViewSize", BindingFlags.NonPublic | BindingFlags.Instance);
            var gvSize = prop.GetValue(gameView, null);
            var widthProp = gvSize.GetType().GetProperty("width", BindingFlags.Public | BindingFlags.Instance);
            var heightProp = gvSize.GetType().GetProperty("height", BindingFlags.Public | BindingFlags.Instance);
            int width = (int)widthProp.GetValue(gvSize, null);
            int height = (int)heightProp.GetValue(gvSize, null);
            return new Vector2Int(width, height);


#endif

            // Works at runtime in builds
            return new Vector2Int(Screen.width, Screen.height);
        }

    }
}
