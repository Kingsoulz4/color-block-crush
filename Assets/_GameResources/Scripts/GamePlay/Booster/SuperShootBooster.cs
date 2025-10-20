using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class SuperShootBooster : BoosterBase
    {
        [SerializeField] GameObject superGunPrb;
        [SerializeField] private float zOffetCam = -3;
        private float originCamZ;
        protected override int CurrentCount { get => UserDataManager.MagnetBooster; set => UserDataManager.MagnetBooster = value; }

        public override void Init()
        {
            base.Init();
            CurrentCount = UserDataManager.MagnetBooster;
        }

        private void Update()
        {
            if (IsShowConfirm && Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    Transform tile = hitInfo.collider.GetComponent<Transform>();
                    if (tile != null)
                    {
                        StartCoroutine(DoBooster(tile));
                    }
                }
            }
        }

        public override void CancelBooster()
        {
            base.CancelBooster();
            Camera.main.GetComponent<GameCamera>().MoveZ(zOffetCam, 0.2f);
        }

        public override void ActiveBooster()
        {
            base.ActiveBooster();
            UpdateVisualBooster();
            IsShowConfirm = false;
            OnStartUseBooster?.Invoke(this, CurrentCount);
        }

        protected override void ShowBooster()
        {
            base.ShowBooster();
            IsShowConfirm = true;
            Camera.main.GetComponent<GameCamera>().MoveZ(zOffetCam, 0.2f);
        }

        protected override void Done()
        {
            base.Done();
            Camera.main.GetComponent<GameCamera>().MoveZ(zOffetCam, 0.2f);
        }

        private IEnumerator DoBooster(Transform tile)
        {
            ActiveBooster();

            yield return new WaitForEndOfFrame();

            GameObject superGun = Instantiate(superGunPrb);
            //superGun.Shoot(tile.transform.position, 0.35f, () =>
            //{
            //    RemoveSuperGun(superGun);
            //    Done();
            //});
        }

        private void RemoveSuperGun(Hammer hammer)
        {
            hammer.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                Destroy(hammer.gameObject);
            }).SetEase(Ease.InOutBack);
        }
    }
}
    