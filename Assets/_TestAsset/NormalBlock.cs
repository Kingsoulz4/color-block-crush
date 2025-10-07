using ColorBlockCrush;
using UnityEngine;

public class NormalBlock : Block
{
    [Header("Block Piece Mesh Renderer References")]
    [SerializeField]
    private MeshRenderer _blockMeshRenderer;

    [Header("Block Piece Pool References")]
    [SerializeField]
    private MeshRenderer _hiddenBlockMeshPrefab;

    [SerializeField]
    private Transform _hiddenBlockMeshContainerTransform;

    private bool _isHidden;

    private MeshRenderer _hiddenBlockMeshRenderer;

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

    public void InitializeBlockPiece(BlockType pieceType, ColorType colorType, int hitPointAmount, bool isStatic, bool canDestroy, bool isHidden)
    {
    }

   
    protected override void StartBlock()
    {
    }

    public override void StartDestroySequence()
    {
    }

    protected override void StopAllAnimations()
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

    private void SpawnHiddenBlockMeshRenderer()
    {
    }

    private void DespawnHiddenBlockMeshRenderer()
    {
    }
}
