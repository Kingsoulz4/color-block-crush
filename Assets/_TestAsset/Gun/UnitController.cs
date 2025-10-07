using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [Header("Renderer References")]
    [SerializeField]
    protected List<MeshRenderer> _colorableUnitMeshRenderersList;

    [Header("Text References")]
    [SerializeField]
    protected TextMeshPro _remainingAmmoAmountText;

    [SerializeField]
    protected TextMeshPro _questionMarkText;

    [Header("Transform References")]
    [SerializeField]
    protected Transform _contentTransform;

    [SerializeField]
    protected Transform _unitContentTransform;

    [SerializeField]
    protected Transform _unitAttackTopRotationContentTransform;

    [SerializeField]
    protected Transform _unitAttackBottomRotationContentTransform;

    [SerializeField]
    protected Transform _unitCenterPointTransform;

    protected int _remainingAmmoAmount;

    protected int _ammoAmount;

    protected float _attackCooldownDuration;

    protected float _attackCooldownTimerSeconds;

    protected Sequence _interactableIdleAnimationSequence;

    protected Sequence _moveToGridNodeSequence;

    protected Sequence _rotateTopToAttackDirectionSequence;

    protected Sequence _rotateBottomToAttackDirectionSequence;

    protected Sequence _growSequence;

    protected Sequence _destroySequence;

    protected Tween _leaveFromTileInventoryTween;

    private readonly Color _remainingAmmoAmountTextColorDefault;

    public readonly Vector4 OutlineColorInteractable;

    public readonly Vector4 OutlineColorSelectBoosterInteractable;

    private readonly float _minAttackDirectionRotation;

    private readonly float _maxAttackDirectionRotation;

    private readonly float _rotateTopToAttackDirectionMaxDegreesDelta;

    private readonly float _rotateTopToAttackDirectionFirstInterval;

    private readonly float _rotateTopToAttackDirectionStartDuration;

    private readonly float _rotateTopToAttackDirectionResetDuration;

    private readonly Ease _rotateTopToAttackDirectionStartEase;

    private readonly Ease _rotateTopToAttackDirectionResetEase;

    private readonly float _rotateBottomToAttackDirectionMaxDegreesDelta;

    private readonly float _rotateBottomToAttackDirectionFirstInterval;

    private readonly float _rotateBottomToAttackDirectionStartDuration;

    private readonly float _rotateBottomToAttackDirectionResetDuration;

    private readonly Ease _rotateBottomToAttackDirectionStartEase;

    private readonly Ease _rotateBottomToAttackDirectionResetEase;

    private readonly float _leaveFromTileInventoryTweenSpeed;

    private readonly Ease _leaveFromTileInventoryTweenEase;

    //public UnitStateTypes CurrentState { get; protected set; }

    //public UnitTypes UnitType { get; protected set; }

    //public EntityColorTypes ColorType { get; protected set; }

    //public LoopGridNode GridNode { get; set; }

    //public List<UnitController> ConnectedUnitsList { get; protected set; }

    //public UnitColorData UnitColorData { get; protected set; }

    //public UnitColorData UnitHiddenColorData { get; protected set; }

    //public UnitOutlineData[] UnitOutlineDataArray { get; protected set; }

    public int AmmoAmount => 0;

    public int RemainingAmmoAmount => 0;

    public bool IsUnitStatic { get; private set; }

    public bool IsUnitUnplaceable { get; private set; }

    public bool IsHidden { get; set; }

    public bool IsAtInteractablePosition { get; set; }

    public bool IsAtAttackPosition { get; set; }

    public bool IsAtSelectBoosterInteractablePosition { get; set; }

    public bool IsConnected => false;

    public bool IsAttackCooldownActive => false;

    public Vector3 UnitCenterPointPosition => default(Vector3);

    public event Action<UnitController> OnUnitMovementStarted
    {
        [CompilerGenerated]
        add
        {
        }
        [CompilerGenerated]
        remove
        {
        }
    }

    public event Action<UnitController> OnUnitMovementCompleted
    {
        [CompilerGenerated]
        add
        {
        }
        [CompilerGenerated]
        remove
        {
        }
    }

    public event Action<UnitController, IAttackable> OnAttack
    {
        add
        {
        }
        remove
        {
        }
    }

    public event Action<UnitController> OnVisualStateUpdated
    {
        add
        {
        }
        remove
        {
        }
    }

    private void OnDisable()
    {
    }

    //public void Initialize(UnitTypes unitType, EntityColorTypes colorType, int ammoAmount, float attackCooldownDuration, bool isUnitStatic, bool isUnitUnplaceable, bool isHidden, UnitColorData unitColorData, UnitColorData unitHiddenColorData, UnitOutlineData[] unitOutlineDataArray)
    //{
    //}

    protected virtual void InitializeVariables()
    {
    }

    public void UpdateVisualState()
    {
    }

    //public void SetCurrentState(UnitStateTypes stateType)
    //{
    //}

    public void UpdateAttackCooldown()
    {
    }

    public virtual void Attack(IAttackable target)
    {
    }

    protected void DecreaseRemainingAmmoAmount()
    {
    }

    private void UpdateRemainingAmmoAmount()
    {
    }

    public List<UnitController> GetOrderedUnitPath()
    {
        return null;
    }

    private void Traverse(UnitController current, HashSet<UnitController> visited)
    {
    }

    private void FollowPathDeterministic(UnitController current, UnitController? previous, List<UnitController> path, HashSet<UnitController> visited)
    {
    }

    protected void UpdateColors()
    {
    }

    public void StartInteractableIdleAnimationSequence()
    {
    }

    private void StopInteractableIdleAnimationSequence()
    {
    }

    public Sequence StartMoveToGridNodeSequence(float duration = 0.25f, Action onComplete = null)
    {
        return null;
    }

    private void StopMoveToGridNodeSequence()
    {
    }

    private void StartRotateTopToAttackDirectionSequence(Vector3 attackDirection)
    {
    }

    private void StopRotateTopToAttackDirectionSequence()
    {
    }

    private void StartRotateBottomToAttackDirectionSequence(Vector3 attackDirection)
    {
    }

    private void StopRotateBottomToAttackDirectionSequence()
    {
    }

    public void StartGrowSequence(Action onComplete = null)
    {
    }

    protected void StopGrowSequence()
    {
    }

    public void StartDestroySequence(Action onComplete = null)
    {
    }

    protected void StopDestroySequence()
    {
    }

    public void StartLeaveFromTileInventoryTween(Vector3 targetPosition, Action onComplete = null)
    {
    }

    private void StopLeaveFromTileInventoryTween()
    {
    }

    protected virtual void StopAllAnimations()
    {
    }
}
