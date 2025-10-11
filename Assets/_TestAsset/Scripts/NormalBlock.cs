using ColorBlockCrush;
using UnityEngine;

public class NormalBlock : Block
{
    [SerializeField]
    private MeshRenderer _blockMeshRenderer;
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

    public void InitializeBlockPiece(BlockType blockType, ColorType colorType, int hitPointAmount, bool canDestroy, bool isHidden)
    {
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
