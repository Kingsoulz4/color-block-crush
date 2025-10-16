using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFeatureSO", menuName = "ScriptableObject/NewFeatureData", order = 1)]
public class NewFeatureSO : ScriptableObject
{
    public List<NewFeatureItemData> newFeatureItemDatas;

    public NewFeatureItemData GetNewFeatureItemData(int level)
    {
        return newFeatureItemDatas.FirstOrDefault(x => x.level == level);
    }
}

public enum NewFeatureTutDisplayType
{
    POPOP_VID,
    POPUP_TEXT
}

[Serializable]
public class NewFeatureItemData
{
    public int level;
    public Sprite icon;
    [TermsPopup] public string title;
    [TermsPopup] public string des;
    [TermsPopup] public string desInTutorial;
    public Sprite spriteBG;
    public Sprite spriteFill;
    public NewFeatureTutDisplayType displayType;
}