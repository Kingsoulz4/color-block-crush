using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class UIGamePlay : MonoBehaviour
    {
        [SerializeField] private Button m_designLevel;
        [SerializeField] private TextMeshProUGUI m_textTime;

        private void Awake()
        {
            m_designLevel.onClick.AddListener(OnClickDesignLevel);
        }

        private void Update()
        {
        }

        private void OnClickDesignLevel()
        {
            SceneManager.LoadScene("ToolEditLevel");
        }
    }
}
