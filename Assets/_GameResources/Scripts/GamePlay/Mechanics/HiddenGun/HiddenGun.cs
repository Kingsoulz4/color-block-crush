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

        public void Init(List<Renderer> listMeshRenderer)
        {
            hiddenGunRenderer = Instantiate(m_hiddenGunRendererPrefab, transform);
            hiddenGunRenderer.Init(listMeshRenderer);
        }

        public void Resolve()
        {
            m_fxOpenHidden.Stop();
            m_fxOpenHidden.Play();
            hiddenGunRenderer.Resolve();
        }
    }
}
