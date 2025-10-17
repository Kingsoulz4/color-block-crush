using ColorBlockCrush.Tools;
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
        [SerializeField] private Gun m_selfGun;

        public Transform ConnectionPoint => m_connectionPoint;
        public List<Gun> ListGun { get; set; } = new();

        private Dictionary<int, RopeConnectOther> dictRope = new();

        public void Init()
        {
            var listIdLinkGun = m_selfGun.GunData.gunConnect;
            m_ropeSelfConnect.material = m_colorReference.listMaterial[m_selfGun.ColorType];
            for (int i = 0; i < listIdLinkGun.Count; i++)
            {
                var gun = LevelManager.Instance.LevelGame.GunBoardController.GetGunByID(listIdLinkGun[i]);

                ListGun.Add(gun);

                if (!gun.ConnectedGunHandler.HasConnected(m_selfGun.ID))
                {
                    var newRope = Instantiate(m_ropeConnectOthersPrefab, transform);
                    newRope.Init(m_selfGun, gun);
                    dictRope[gun.ID] = newRope;
                }
            }
        }

        public bool HasConnected(int id)
        {
            return dictRope.ContainsKey(id);
        }
    }
}
