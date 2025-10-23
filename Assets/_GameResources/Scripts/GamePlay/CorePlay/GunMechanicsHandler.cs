using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ColorBlockCrush
{
    public partial class Gun
    {
        [SerializeField] private HiddenGun m_hiddenGun;
        [SerializeField] private ConnectedGuns m_connectedGuns;

        public ConnectedGuns ConnectedGunHandler => m_connectedGuns;

        public UnityEvent OnHiddenResolved { get; set; } = new();

        public void InitMechanics()
        {
            if(GunData.isHidden)
            {
                EnableTextBulletCount(false);
                m_hiddenGun.Init(this, meshRendererList);
            }

            if(GunData.gunConnect.Count > 0)
            {
                ConnectedGunHandler.gameObject.SetActive(true);
                InitMechanicConnectedGuns();
            }
            else
            {
                ConnectedGunHandler.gameObject.SetActive(false);
            }
        }

        private void InitMechanicConnectedGuns()
        {
            StartCoroutine(IEInitMechanicConnectedGuns());
        }

        private IEnumerator IEInitMechanicConnectedGuns()
        {
            yield return null;
            ConnectedGunHandler.Init();
        }

        public void UpdateMechanics()
        {
            if(GunData.isHidden && IsFrontRow)
            {
                UpdateVisuals();
                EnableTextBulletCount(true);
                m_hiddenGun.Resolve();
                OnHiddenResolved?.Invoke();
            }

        }

        public void ForceHiddenResolve()
        {
            if (GunData.isHidden)
            {
                UpdateVisuals();
                EnableTextBulletCount(true);
                m_hiddenGun.Resolve();
                OnHiddenResolved?.Invoke();
            }
        }

        public void RemoveAllConnection()
        {
            foreach(var connectGun in AllConnectedGuns)
            {
                if (connectGun != this)
                {
                    connectGun.RemoveConnection(this);
                    connectGun.CheckDisappear();
                }
            }
            AllConnectedGunCount = 0;
            AllConnectedGuns.Clear();
            ConnectedGunHandler.ClearConnection();
        }

        public void RemoveConnection(Gun gunn)
        {
            AllConnectedGuns.Remove(gunn);
            AllConnectedGunCount -= 1;
            ConnectedGunHandler.RemoveConnection(gunn);
        }
    }
}
