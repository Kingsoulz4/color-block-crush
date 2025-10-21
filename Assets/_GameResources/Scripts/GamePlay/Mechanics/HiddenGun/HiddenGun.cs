using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class HiddenGun : MonoBehaviour
    {
        [SerializeField] private HiddenGunRenderer m_hiddenGunRendererPrefab;
        [SerializeField] private ParticleSystem m_fxOpenHidden;

        private HiddenGunRenderer hiddenGunRenderer;

        private Gun gunController;

        public bool IsResolved { get; private set; } = false;

        public void Init(Gun gun, List<Renderer> listMeshRenderer)
        {
            gunController = gun;
            hiddenGunRenderer = Instantiate(m_hiddenGunRendererPrefab, transform);
            hiddenGunRenderer.Init(listMeshRenderer);
        }

        public void Resolve()
        {
            //var defaultPos = transform.localPosition.y;
            //gunController.transform.DOLocalMoveY(transform.localPosition.y + 10, 0.1f).SetEase(Ease.OutQuart)
            //    .OnComplete(() =>
            //    {
            //        gunController.transform.DOLocalMoveY(defaultPos, 0.1f).SetEase(Ease.OutBack);
            //    });

            IsResolved = true;
            m_fxOpenHidden.gameObject.SetActive(true);
            m_fxOpenHidden.Stop();
            m_fxOpenHidden.Play();
            hiddenGunRenderer.Resolve();
        }
    }
}
