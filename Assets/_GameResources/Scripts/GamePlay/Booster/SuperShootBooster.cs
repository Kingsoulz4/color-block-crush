using ColorBlockCrush.Tools;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace ColorBlockCrush
{
    public class SuperShootBooster : BoosterBase
    {
        [SerializeField] GameObject superGunPrb;
        [SerializeField] private float zOffetCam = -3;
        [SerializeField] private Bullet bulletPrb;
        [SerializeField] private Transform spawnPoint;
        private float originCamZ;

        protected override int CurrentCount { get => UserDataManager.MagnetBooster; set => UserDataManager.MagnetBooster = value; }

        public override void Init()
        {
            base.Init();
        }

        private void Update()
        {
            if (IsShowConfirm && Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hitInfo))
                {
                    Block block = hitInfo.collider.GetComponent<Block>();
                    if (block != null)
                    {
                        StartCoroutine(DoBooster(block.ColorType));
                    }
                }
            }
        }

        public override void CancelBooster()
        {
            base.CancelBooster();
            Camera.main.GetComponent<GameCamera>().MoveZ(originCamZ, 0.2f);
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
            originCamZ = Camera.main.transform.position.z;
            Camera.main.GetComponent<GameCamera>().MoveZ(zOffetCam, 0.2f);
        }

        protected override void Done()
        {
            base.Done();
            Camera.main.GetComponent<GameCamera>().MoveZ(originCamZ, 0.2f);
        }

        private IEnumerator DoBooster(ColorType colorType)
        {
            ActiveBooster();
            List<Block> blocks = new List<Block>();
            blocks = LevelController.Instance.BlockBoardController.GetBlockListByColor(colorType);
            yield return new WaitForEndOfFrame();
            GameObject superGun = Instantiate(superGunPrb);
            yield return StartCoroutine(Fire(blocks));
            RemoveSuperGun(superGun);
            Done();
            //superGun.Shoot(tile.transform.position, 0.35f, () =>
            //{
            //    RemoveSuperGun(superGun);
            //    Done();
            //});
        }

        private IEnumerator Fire(List<Block> blocks)
        {
            foreach (Block block in blocks)
            {
                Bullet bullet = Instantiate(bulletPrb, spawnPoint.position, Quaternion.identity);
                bullet.transform.SetParent(LevelController.Instance.transform);
                bullet.OnInit(null, block, (gun, block) =>
                {
                    block.TakeDamage(1);
                    Destroy(bullet.gameObject);
                });
                yield return null;
            }

        }

        private void RemoveSuperGun(GameObject a)
        {
            Destroy(a);
        }
    }
}
