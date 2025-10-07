using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Assets.LoopGames.Game.Scripts.Controllers.PieceControllers
{
	public class PieceController : MonoBehaviour, IAttackable
	{
		public Action<PieceController> OnPieceInitialized;

		public Action<int, PieceController> OnPieceTookInstantDamage;

		public Action<int, PieceController> OnPieceTookRealtimeDamage;

		public Action<PieceController> OnPieceMovementStarted;

		public Action<PieceController> OnPieceMovementCompleted;

		public Action<PieceController> OnPieceDestroyStarted;

		public Action<PieceController> OnPieceDestroyCompleted;

		[Header("Renderer References")]
		[SerializeField]
		protected List<MeshRenderer> _colorablePieceMeshRenderersList;

		[Header("Transform References")]
		[SerializeField]
		protected Transform _contentTransform;

		[SerializeField]
		protected Transform _pieceContentTransform;

		[SerializeField]
		protected Transform _pushContentTransform;

		[SerializeField]
		protected Transform _damagePointTransform;

		[SerializeField]
		protected Transform _raycastPointTransform;

		[Header("Collider References")]
		[SerializeField]
		protected Collider _collider;

		[SerializeField]
		protected Collider _trigger;

		[Header("Variables")]
		[SerializeField]
		private float _maxStrength;

		[SerializeField]
		private float _maxPushOffset;

		[SerializeField]
		private float _maxTiltAngle;

		[SerializeField]
		private float _lerpSpeed;

		[SerializeField]
		private float _lerpSpeedSecond;

		[SerializeField]
		private bool _pushX;

		[SerializeField]
		private bool _pushZ;

		[SerializeField]
		private bool _tiltX;

		[SerializeField]
		private bool _tiltY;

		[SerializeField]
		private bool _tiltZ;

		protected int _maxHitPointAmount;

		protected int _instantHitPointAmount;

		protected int _realtimeHitPointAmount;

		private Vector3 _initialLocalPos;

		private Quaternion _initialLocalRot;

		private Vector3 _targetLocalPos;

		private Quaternion _targetLocalRot;

		private Sequence _moveToGridNodeSequence;

		private Sequence _moveToGridHeightSequence;

		protected Sequence _destroySequence;

		//private List<LoopGridNode> _moveToGridNodePathGridNodesList;

		private List<Transform> _pushingBalls;

		public static float PieceMoveToGridNodeSpeed;

		public static float PieceDestroyDuration;

		public static float PieceMoveToGridHeightDuration;

		public static Ease PieceMoveToGridNodeEase;

		public static Ease PieceMoveToGridHeightEase;

		public static Ease PieceDestroyEase;

		public PieceTypes PieceType { get; private set; }

		public EntityColorTypes ColorType { get; protected set; }

		//public LoopGridNode GridNode { get; set; }

		//public int GridHeight { get; set; }

		//public PieceMatrixData PieceMatrixData { get; protected set; }

		public bool IsStatic { get; private set; }

		public bool IsIndestructible { get; private set; }

		public bool IsAttacked { get; protected set; }

		public bool IsInIdleState => false;

		//public PieceColorData PieceColorData { get; protected set; }

		private void Update()
		{
		}

		protected virtual void OnDisable()
		{
		}

		//public void Initialize(PieceTypes pieceType, EntityColorTypes colorType, PieceMatrixData pieceMatrixData, PieceColorData pieceColorData, int hitPointAmount, bool isStatic, bool isIndestructible)
		//{
		//}

		protected virtual void InitializeVariables()
		{
		}

		protected virtual void StartPiece()
		{
		}

		protected virtual void UpdateColors()
		{
		}

		public EntityColorTypes GetColorType()
		{
			return default(EntityColorTypes);
		}

		public PieceTypes GetPieceType()
		{
			return default(PieceTypes);
		}

		public virtual void TakeDamageInstant(int damageAmount)
		{
		}

		public virtual void TakeDamageRealtime(int damageAmount)
		{
		}

		public int GetMaxHitPointAmount()
		{
			return 0;
		}

		public int GetInstantHitPointAmount()
		{
			return 0;
		}

		public int GetRealtimeHitPointAmount()
		{
			return 0;
		}

		public void RegisterPush(Transform ball)
		{
		}

		public void UnregisterPush(Transform ball)
		{
		}

		private Transform GetNearestBall()
		{
			return null;
		}

		//public void StartMoveToGridNodeSequence(List<LoopGridNode> pathGridNodes, bool moveToGridHeight = true, bool disableMoveToGridHeightForFirstGridNode = false, Action onComplete = null)
		//{
		//}

		public void StartMoveToGridNodeSequence(bool moveToGridHeight = true, bool disableMoveToGridHeightForFirstGridNode = false, Action onComplete = null)
		{
		}

		public void StopMoveToGridNodeSequence()
		{
		}

		public void StartMoveToGridHeightSequence()
		{
		}

		private void StopMoveToGridHeightSequence()
		{
		}

		public virtual void StartDestroySequence()
		{
		}

		private void StopDestroySequence()
		{
		}

		protected virtual void StopAllAnimations()
		{
		}

		public bool GetIsAttacked()
		{
			return false;
		}

		public Vector3 GetRaycastPointPosition()
		{
			return default(Vector3);
		}

		public Vector3 GetDamagePointPosition()
		{
			return default(Vector3);
		}

		public Vector3 GetTransformPosition()
		{
			return default(Vector3);
		}
	}
}
