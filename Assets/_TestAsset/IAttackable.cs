using UnityEngine;

public interface IAttackable
{
    void TakeDamageNow(int damageAmount);

    void TakeDamageNormal(int damageAmount);

    int GetMaxHitPoint();

    int GetNowHitPoint();

    int GetNormalHitPoint();

    bool GetIsAttacked();

    Vector3 GetTransformPosition();

    Vector3 GetRaycastPointPosition();

    Vector3 GetDamagePointPosition();
}
