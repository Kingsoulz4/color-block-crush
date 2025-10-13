using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush
{
    public class PopupTutorialTextNewFeature : PopupUI
    {
        [SerializeField] private Text m_textContent;

        public void SetData(NewFeatureItemData newFeatureData)
        {
            m_textContent.text = newFeatureData.desInTutorial;
        }
    }
}
