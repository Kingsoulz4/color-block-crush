using ColorBlockCrush;
using ColorBlockCrush.Tools;
using UnityEngine;

public class NormalBlock : Block
{

    private bool _isHidden;

    public bool IsHidden
    {
        get
        {
            return false;
        }
        set
        {
        }
    }
   
    protected override void StartBlock()
    {
    }

    private void UpdateHiddenVisualState()
    {
    }

    private void ShowHiddenVisualState()
    {
    }

    public void HideHiddenVisualState()
    {
    }
}
