using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class EndPointConveyor : MonoBehaviour
    {
        [SerializeField] private ConveyorController conveyorController;
        [SerializeField] private SlotController slotController;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constant.Tag.GUN))
            {
                if (other.TryGetComponent(out Gun gun))
                {
                    if (gun.ConnectedGuns.Count > 0)
                    {
                        if (!LevelController.Instance.SlotController.CanPlaceGuns(gun.AllConnectedGunCount))
                        {
                            LevelController.Instance.ConveyorController.PauseAllTray();
                            LevelController.Instance.LoseLevel();
                            return;
                        }
                    }
                    else if (!LevelController.Instance.SlotController.CanPlaceGuns(1))
                    {
                        LevelController.Instance.ConveyorController.PauseAllTray();
                        LevelController.Instance.LoseLevel();
                        return;
                    }
                    conveyorController.MoveTrayIn(gun.TrayItem);
                    slotController.MoveGunIn(gun);
                }
            }
        }
    }
}
