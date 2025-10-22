using UnityEngine;
using UnityEngine.EventSystems;

namespace ColorBlockCrush
{
    public class UserInput : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera _camera;

        [Header("Settings")]
        [SerializeField] private LayerMask _gunLayerMask;

        private float lastClickTime = 0f;
        private float clickCooldown = 0.05f;
        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        private void Update()
        {
            if (GameManager.GameState != GameState.Playing)
            {
                return;
            }

            HandleInput();
        }

        private bool IsPointerOverUI()
        {
            if (EventSystem.current == null) return false;

#if UNITY_EDITOR || UNITY_STANDALONE
            // Mouse / Editor
            return EventSystem.current.IsPointerOverGameObject();
#else
    // Mobile (Touch)
    if (Input.touchCount > 0)
        return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

    return EventSystem.current.IsPointerOverGameObject();
#endif
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Time.time - lastClickTime < clickCooldown) return;

                if (IsPointerOverUI()) return;

                lastClickTime = Time.time;
                Vector3 inputPosition = Input.mousePosition;
                DetectAndTapGun(inputPosition);
            }
        }

        private void DetectAndTapGun(Vector3 screenPosition)
        {
            if (BoosterManager.Instance && BoosterManager.Instance.SuperShootBooster.InProgress) return;

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
                    gun.OnGunClicked();
                    LevelEvent.OnGunClick?.Invoke(gun);
                }
            }
        }
    }
}