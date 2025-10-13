using AYellowpaper.SerializedCollections;
using ColorBlockCrush.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class ButtonPlay : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<LevelDifficult, GameObject> m_listTypeDisplay;

        public void SetDisplayLevelType(LevelDifficult levelType)
        {
            foreach (var item in m_listTypeDisplay)
            {
                item.Value.SetActive(false);
            }
            m_listTypeDisplay[levelType].SetActive(true);
        }

        
    }
}
