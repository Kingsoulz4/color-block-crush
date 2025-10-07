using DG.Tweening;
using UnityEngine;

public class ShooterUnitController : UnitController
{
    [Header("Shooter Unit Transform References")]
    [SerializeField]
    protected Transform[] _projectileSpawnPointTransformsArray;

    private int _currentProjectileSpawnPointIndex;

    //private ProjectileFactoryController _projectileFactoryController;

    private Sequence _recoilSequence;

    public static float ShooterUnitRecoilAmount;

    public static float ShooterUnitRecoilBackDuration;

    public static float ShooterUnitRecoilResetDuration;

    public static Ease ShooterUnitRecoilBackEase;

    public static Ease ShooterUnitRecoilResetEase;

    //public void InitializeShooterUnit(UnitTypes unitType, EntityColorTypes colorType, int ammoAmount, float attackCooldownDuration, bool isUnitStatic, bool isUnitUnplaceable, bool isHidden, UnitColorData unitColorData, UnitColorData unitHiddenColorData, UnitOutlineData[] unitOutlineDataArray, ProjectileFactoryController projectileFactoryController)
    //{
    //}

    protected override void InitializeVariables()
    {
    }

    public override void Attack(IAttackable target)
    {
    }

    private void StartRecoilSequence()
    {
    }

    private void StopRecoilSequence()
    {
    }

    protected override void StopAllAnimations()
    {
    }
}
