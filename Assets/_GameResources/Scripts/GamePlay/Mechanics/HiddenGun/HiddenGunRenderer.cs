using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class HiddenGunRenderer : MechanicRendererBase
    {
        [SerializeField] private List<Material> m_listHiddenMaterial;

        public override void Resolve()
        {
            base.Resolve();
        }

        internal void Init(List<MeshRenderer> listMeshRenderer)
        {
            for(int i=0; i<listMeshRenderer.Count; i++)
            {
                listMeshRenderer[i].material = m_listHiddenMaterial[Math.Clamp(i, 0, m_listHiddenMaterial.Count - 1)];
            }
        }
    }
}
