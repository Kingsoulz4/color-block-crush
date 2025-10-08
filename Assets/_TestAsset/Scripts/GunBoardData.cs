
using ColorBlockCrush;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunBoardData", menuName = "ColorBlockCrush/Gun Board")]
public class GunBoardData : ScriptableObject
{
    public List<GunColumnData> ColumnGunData = new List<GunColumnData>();
}
