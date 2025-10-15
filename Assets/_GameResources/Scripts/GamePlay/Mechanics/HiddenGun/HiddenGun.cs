using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class HiddenGun : MonoBehaviour
    {
        [SerializeField] private HiddenGunRenderer m_hiddenGunRendererPrefab;

        private HiddenGunRenderer hiddenGunRenderer;

        public void Init(List<MeshRenderer> listMeshRenderer)
        {
            hiddenGunRenderer = Instantiate(m_hiddenGunRendererPrefab, transform);
            hiddenGunRenderer.Init(listMeshRenderer);
        }

        public void Resolve()
        {
            hiddenGunRenderer.Resolve();
        }
    }
}
