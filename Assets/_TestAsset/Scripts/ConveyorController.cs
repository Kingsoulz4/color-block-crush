using ColorBlockCrush;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{

    [SerializeField] private int _maxSlots = 5;
    [SerializeField] private Transform startPos;
    private List<Gun> movingGuns;

    public Action<Gun> OnStartAddGunToConveyor;
    public Action<Gun> OnGunRemovedConveyor;

    public List<Gun> Guns { get => movingGuns;}

    public void Init()
    {
        movingGuns = new List<Gun>();
        movingGuns.Clear();

    }

    public void PushGuns(List<Gun> guns)
    {
        foreach (Gun gun in guns)
        {
            AddGun(gun);
            gun.MoveToConeyor(startPos.position);
            OnStartAddGunToConveyor?.Invoke(gun);
        }
    }


    public bool CanPlaceGuns(int count)
    {
        return movingGuns.Count + count <= _maxSlots;
    }

    public void AddGun(Gun gun)
    {
        if (!movingGuns.Contains(gun))
        {
            gun.OnGunEmpty += OnGunEmpty;
            movingGuns.Add(gun);
        }
    }
    private void OnGunEmpty(Gun gun)
    {
        RemoveGun(gun);
        Destroy(gun.gameObject);
    }

    public void RemoveGun(Gun gun)
    {
        if (movingGuns.Contains(gun))
        {
            gun.OnGunEmpty -= OnGunEmpty;
            movingGuns.Remove(gun);
            OnGunRemovedConveyor?.Invoke(gun);
        }
    }
}
