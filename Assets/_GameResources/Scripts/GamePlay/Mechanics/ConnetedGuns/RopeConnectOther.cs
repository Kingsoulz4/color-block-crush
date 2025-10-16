using ColorBlockCrush.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class RopeConnectOther : MonoBehaviour
    {
        [SerializeField] private ListMaterialByColor m_listMat;
        [SerializeField] private MeshRenderer m_mainRope;

        private Gun mainGun;
        private Gun connectedGun;

        public void Init(Gun mainGun, Gun connectedGun)
        {
            this.mainGun = mainGun;
            this.connectedGun = connectedGun;
            m_mainRope.materials[0] = m_listMat.listMaterial[mainGun.ColorType];
            m_mainRope.materials[1] = m_listMat.listMaterial[connectedGun.ColorType];
        }

        private void Update()
        {
            float length = Vector3.Distance(mainGun.ConnectedGunHandler.ConnectionPoint.transform.position, connectedGun.ConnectedGunHandler.ConnectionPoint.transform.position);
            m_mainRope.transform.localScale = new Vector3(m_mainRope.transform.localScale.x, m_mainRope.transform.localScale.y, length);
            transform.LookAt(connectedGun.ConnectedGunHandler.ConnectionPoint);
        }
    }
}
