using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public partial class Gun
    {
        [SerializeField] private HiddenGun m_hiddenGun;

        public void InitMechanics()
        {
            if(gunData.isHidden)
            {
                EnableTextBulletCount(false);
                m_hiddenGun.Init(meshRendererList);
            }
        }

        public void UpdateMechanics()
        {
            if(gunData.isHidden && IsFrontRow)
            {
                UpdateVisuals();
                EnableTextBulletCount(true);
                m_hiddenGun.Resolve();
            }

        }
    }
}
