using UnityEngine;

namespace ColorBlockCrush
{
    public class UserInput : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _camera;

        [Header("Settings")]
        [SerializeField] private LayerMask _gunLayerMask;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            // Mouse/Touch input
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 inputPosition = Input.mousePosition;
                DetectAndTapGun(inputPosition);
            }
        }

        private void DetectAndTapGun(Vector3 screenPosition)
        {
            Ray ray = _camera.ScreenPointToRay(screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _gunLayerMask))
            {
                Gun gun = hit.collider.GetComponent<Gun>();

                if (gun == null)
                {
                    // Try to get Gun from parent
                    gun = hit.collider.GetComponentInParent<Gun>();
                }

                if (gun != null)
                {
                    OnGunClicked(gun);
                }
            }
        }

        private void OnGunClicked(Gun gun)
        {
            gun.PlayAnim(Constant.GunAnimation.CLICK);

            if (!gun.CanPushToConveyor())
            {
                return;
            }


            if (gun.GunPos == GunPos.ON_GUN_BOARD)
            {
                LevelController.Instance.GunBoardController.OnTapGun(gun);
            }

            if (gun.GunPos == GunPos.ON_SLOT)
            {
                LevelController.Instance.SlotController.OnTapGun(gun);
            }

        }
    }
}