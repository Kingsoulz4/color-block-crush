using UnityEngine;

public interface IAttackable
{
    void TakeDamage(int damageAmount);

    int GetMaxHitPoint();

    int GetHitPoint();

    bool GetIsAttacked();

    Vector3 GetTransformPosition();

    Vector3 GetRaycastPointPosition();

    Vector3 GetDamagePointPosition();
}
