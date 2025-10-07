using UnityEngine;

public enum EntityColorTypes { Red, Blue, Green, Yellow, None }

public interface IAttackable
{
    void TakeDamageInstant(int damageAmount);

    void TakeDamageRealtime(int damageAmount);

    int GetMaxHitPointAmount();

    int GetInstantHitPointAmount();

    int GetRealtimeHitPointAmount();

    bool GetIsAttacked();

    EntityColorTypes GetColorType();

    PieceTypes GetPieceType();

    Vector3 GetTransformPosition();

    Vector3 GetRaycastPointPosition();

    Vector3 GetDamagePointPosition();
}
