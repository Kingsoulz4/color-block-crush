using UnityEngine.UI;
using UnityEngine;

namespace ColorBlockCrush
{
    public class PopupAds : PopupUI
    {
        [SerializeField] private Button closeBtn;

        private void Awake()
        {
            closeBtn.onClick.AddListener(() =>
            {
                Hide();
                onClose?.Invoke();
            });
        }
    }
}
