using ColorBlockCrush.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class RopeConnectOther : MonoBehaviour
    {
        [SerializeField] private ListMaterialByColor m_listMat;
        [SerializeField] private Material m_hiddenMat;
        [SerializeField] private MeshRenderer m_mainRope;

        private Gun mainGun;
        private Gun connectedGun;

        public void Init(Gun mainGun, Gun connectedGun)
        {
            this.mainGun = mainGun;
            this.connectedGun = connectedGun;
            var matConnectedGun = m_listMat.listMaterial[connectedGun.ColorType];
            if(connectedGun.GunData.isHidden)
            {
                matConnectedGun = m_hiddenMat;
                connectedGun.OnHiddenResolved.AddListener(OnConnectedHiddenResolve);
            }
            var matMainGun = m_listMat.listMaterial[mainGun.ColorType];
            if(mainGun.GunData.isHidden)
            {
                matMainGun = m_hiddenMat;
                mainGun.OnHiddenResolved.AddListener(OnMainGunHiddenResolved);
            }
            var listMat = new Material[] { matConnectedGun, matMainGun };
            m_mainRope.materials = listMat;
        }

        private void Update()
        {
            float length = Vector3.Distance(mainGun.ConnectedGunHandler.ConnectionPoint.transform.position, connectedGun.ConnectedGunHandler.ConnectionPoint.transform.position);
            m_mainRope.transform.localScale = new Vector3(m_mainRope.transform.localScale.x, m_mainRope.transform.localScale.y, length);
            transform.LookAt(connectedGun.ConnectedGunHandler.ConnectionPoint);
        }

        private void OnMainGunHiddenResolved()
        {
            var curentListMat = m_mainRope.materials;
            curentListMat[1] = m_listMat.listMaterial[mainGun.ColorType];
            m_mainRope.materials = curentListMat;
        }

        private void OnConnectedHiddenResolve()
        {
            var curentListMat = m_mainRope.materials;
            curentListMat[0] = m_listMat.listMaterial[connectedGun.ColorType];
            m_mainRope.materials = curentListMat;
        }
    }
}
