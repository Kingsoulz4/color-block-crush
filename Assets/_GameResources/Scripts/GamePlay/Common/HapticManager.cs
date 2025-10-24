using Lofelt.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ColorBlockCrush
{
    public class HapticManager : SingletonMono<HapticManager>
    {
        public bool CanHaptic = false;
        public static int HapticSetting
        {
            get { return PlayerPrefs.GetInt("isActiveVibrate", 1); }
            set { PlayerPrefs.SetInt("isActiveVibrate", value); }
        }

        private void Start()
        {
            this.Wait(Time.deltaTime, () =>
            {
                EnableHaptic(HapticSetting == 1 ? true : false);
            });
        }

        public void Haptic()
        {
            if (CanHaptic)
            {
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
            }
        }

        public void EnableHaptic(bool status)
        {
            HapticSetting = status ? 1 : 0;
            CanHaptic = status;
        }
    }
}
