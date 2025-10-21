using ColorBlockCrush.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class ConnectedGuns : MonoBehaviour
    {
        [SerializeField] private ListMaterialByColor m_colorReference;
        [SerializeField] private RopeConnectOther m_ropeConnectOthersPrefab;
        [SerializeField] private Transform m_listRopeContainer;
        [SerializeField] private Transform m_connectionPoint;
        [SerializeField] private MeshRenderer m_ropeSelfConnect;
        [SerializeField] private Material m_hiddenMat;
        [SerializeField] private Gun m_selfGun;

        public Transform ConnectionPoint => m_connectionPoint;
        public List<Gun> ListGun { get; set; } = new();

        private Dictionary<int, RopeConnectOther> dictRope = new();

        public void Init()
        {
            var listIdLinkGun = m_selfGun.GunData.gunConnect;
            m_ropeSelfConnect.material = m_colorReference.listMaterial[m_selfGun.ColorType];
            if(m_selfGun.GunData.isHidden)
            {
                m_ropeSelfConnect.material = m_hiddenMat;
                m_selfGun.OnHiddenResolved.AddListener(OnSeflGunHiddenResolved);
            }
            for (int i = 0; i < listIdLinkGun.Count; i++)
            {
                var gun = LevelManager.Instance.LevelGame.GunBoardController.GetGunByID(listIdLinkGun[i]);

                ListGun.Add(gun);

                if (!gun.ConnectedGunHandler.HasConnected(m_selfGun.ID))
                {
                    var newRope = Instantiate(m_ropeConnectOthersPrefab, m_listRopeContainer);
                    newRope.Init(m_selfGun, gun);
                    dictRope[gun.ID] = newRope;
                }
            }
        }

        private void OnSeflGunHiddenResolved()
        {
            m_ropeSelfConnect.material = m_colorReference.listMaterial[m_selfGun.ColorType];
        }

        public bool HasConnected(int id)
        {
            return dictRope.ContainsKey(id);
        }

        public void RemoveConnection(Gun gun)
        {
            ListGun.Remove(gun);
            if (dictRope.ContainsKey(gun.ID))
            {
                Destroy(dictRope[gun.ID].gameObject);
                dictRope.Remove(gun.ID);
            }
        }

        internal void ClearConnection()
        {
            ListGun.Clear();
            foreach(var item in dictRope)
            {
                Destroy(item.Value.gameObject);
            }    
            dictRope.Clear();
        }
    }
}
