using UnityEngine;

namespace Assets.LoopGames.Game.Scripts.Controllers.PieceControllers
{
	public class BlockPieceController : PieceController
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

		//public void InitializeBlockPiece(PieceTypes pieceType, EntityColorTypes colorType, PieceMatrixData pieceMatrixData, PieceColorData pieceColorData, int hitPointAmount, bool isStatic, bool isIndestructible, bool isHidden)
		//{
		//}

		//protected override void InitializeVariables()
		//{
		//}

		//protected override void StartPiece()
		//{
		//}

		//public override void StartDestroySequence()
		//{
		//}

		//protected override void StopAllAnimations()
		//{
		//}

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
}
