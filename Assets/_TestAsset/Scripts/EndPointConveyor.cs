using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class EndPointConveyor : MonoBehaviour
    {
        [SerializeField] private ConveyorController conveyorController;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constant.Tag.GUN))
            {
                if (other.TryGetComponent(out Gun gun))
                {
                    conveyorController.MoveTrayBack(gun.TrayItem);
                    gun.gameObject.SetActive(false);
                }
            }

        }
    }
}
