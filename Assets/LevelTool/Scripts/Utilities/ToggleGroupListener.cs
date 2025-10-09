using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ColorBlockCrush.Tools
{
    [RequireComponent(typeof(ToggleGroup))]
    public class ToggleGroupListener : MonoBehaviour
    {
        [Serializable]
        public class ToggleSelectedEvent : UnityEvent<Toggle, int, int>
        {
        }

        public ToggleGroup group;

        public List<Toggle> toggles = new();

        public ToggleSelectedEvent onSelected;
        public event Action<Toggle, int, int> Selected;

        private readonly Dictionary<Toggle, UnityAction<bool>> _handlers = new();
        private int _currentIndex = -1;

        void Reset() => group = GetComponent<ToggleGroup>();

        void Awake()
        {
            if (!group) group = GetComponent<ToggleGroup>();
        }

        void OnEnable()
        {
            if (toggles == null || toggles.Count == 0)
                toggles = new List<Toggle>(GetComponentsInChildren<Toggle>(true));
            
            foreach (var t in toggles)
                if (t)
                    t.group = group;
            
            foreach (var t in toggles)
            {
                if (!t) continue;
                UnityAction<bool> cb = v => OnToggleChanged(t, v);
                _handlers[t] = cb;
                t.onValueChanged.AddListener(cb);
            }
            
            for (int i = 0; i < toggles.Count; i++)
                if (toggles[i] && toggles[i].isOn)
                {
                    _currentIndex = i;
                    break;
                }
        }

        void OnDisable()
        {
            foreach (var kv in _handlers)
                if (kv.Key)
                    kv.Key.onValueChanged.RemoveListener(kv.Value);
            _handlers.Clear();
        }

        private void OnToggleChanged(Toggle t, bool isOn)
        {
            if (!isOn) return;
            int newIndex = toggles.IndexOf(t);
            if (newIndex == -1 || newIndex == _currentIndex) return;

            int oldIndex = _currentIndex;
            _currentIndex = newIndex;

            onSelected?.Invoke(t, newIndex, oldIndex);
            Selected?.Invoke(t, newIndex, oldIndex);
        }
        
        public void SelectByIndex(int index)
        {
            if (index < 0 || index >= toggles.Count) return;
            if (toggles[index]) toggles[index].isOn = true;
        }

        public int CurrentIndex => _currentIndex;
    }
}
