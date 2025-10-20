using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Yoolax.Framework;

namespace ColorBlockCrush
{
    public class VisualCountBooster : MonoBehaviour
    {
        [Header("Unlock Booster")] 
        [SerializeField] GameObject visualBooster;
        [SerializeField] Text txt_CountBooster;
        [SerializeField] GameObject obj_BoosterActive;
        [SerializeField] GameObject obj_BoosterAdd;
        [SerializeField] Button btn_AddBooster;
        [SerializeField] Image icon;
        [Header("Lock Booster")]
        [SerializeField] private GameObject lockIcon;
        [SerializeField] private Text lvUnlock;
        private BoosterType boosterType;
        private BoosterItemData boosterItemData;

        [Header("First Claim Booster")] 
        [SerializeField] private Image firstClaimBoosterIcon;
        [SerializeField] private GameObject goEffect;
        [SerializeField] private GameObject handTut;
        [SerializeField] private GameObject vfx;
        [SerializeField] private Canvas boosterCanvas;
        private Action onCompleteForceTut;

        public Image Icon { get => icon;}
        
        void Awake()
        {
            Server.Get<OnForceTutBooster>().AddListener(ShowEffect);
        }

        private void OnEnable()
        {
            UpdateState();
        }

        private void OnDestroy()
        {
            Server.Get<OnForceTutBooster>().RemoveListener(ShowEffect);
        }

        public void Init(int count, BoosterType boosterType)
        {
            Debug.Log($"Init {boosterType}");
            this.boosterType = boosterType;
            if (obj_BoosterActive == null || obj_BoosterAdd == null) { return; }
            btn_AddBooster.onClick.AddListener(ShowPopupAddBooster);
            bool isActive = count > 0;
            boosterItemData = BoosterManager.Instance.BoosterData.GetBoosterItemData(boosterType);
            var iconSprite = boosterItemData.icon;
            Icon.sprite = iconSprite;
            firstClaimBoosterIcon.sprite = iconSprite;
            obj_BoosterActive.SetActive(isActive);
            obj_BoosterAdd.SetActive(!isActive);
            lvUnlock.text = $"Lv {boosterItemData.levelUnlock}";
            UpdateText(count);
            UpdateState();
        }

        private void UpdateState()
        {
            Debug.Log($"Update State {boosterType}");
            if(boosterItemData == null) return;
            bool unlockBooster = false;
            switch (boosterType)
            {
                case BoosterType.ADD_TRAY:
                    unlockBooster = boosterItemData.levelUnlock < UserDataManager.Level ||
                                    (boosterItemData.levelUnlock == UserDataManager.Level 
                                    && UserDataManager.FirstClaimAddTrayBooster);
                    break;
                case BoosterType.HAND_MOVE:
                    unlockBooster = boosterItemData.levelUnlock < UserDataManager.Level ||
                                    (boosterItemData.levelUnlock == UserDataManager.Level 
                                    && UserDataManager.FirstClaimHandBooster);
                    break;
                case BoosterType.SHUFFLE:
                    unlockBooster = boosterItemData.levelUnlock < UserDataManager.Level ||
                                    (boosterItemData.levelUnlock == UserDataManager.Level 
                                     && UserDataManager.FirstClaimShuffleBooster);
                    break;
                case BoosterType.MAGNET:
                    unlockBooster = boosterItemData.levelUnlock < UserDataManager.Level ||
                                    (boosterItemData.levelUnlock == UserDataManager.Level 
                                     && UserDataManager.FirstClaimMagnetBooster);
                    break;
            }
            
            visualBooster.SetActive(unlockBooster);
            lockIcon.SetActive(!unlockBooster);
        }

        private void ShowPopupAddBooster()
        {
            // PopupBuyBooster poup = UIManager.Instance.GetPopupActive<PopupBuyBooster>();
            // if (poup == null)
            // {
            //     poup = UIManager.Instance.ShowPopup<PopupBuyBooster>(null);                
            //     poup.VisualBooster(boosterType);
            // }
        }

        public void UpdateTextCountBooster(int count)
        {
            bool isActive = count > 0;
            if (obj_BoosterActive == null || obj_BoosterAdd == null) { return; }
            obj_BoosterActive.SetActive(isActive);
            obj_BoosterAdd.SetActive(!isActive);
            UpdateText(count);
        }

        private void UpdateText(int count)
        {
            string value = $"{count}";
            if (count > 99)
            {
                value = $"99+";
            }
            txt_CountBooster.text = value;
        }
        
        public void OnClickButton()
        {
            if(onCompleteForceTut != null)
            {
                // onCompleteForceTut.Invoke();
                onCompleteForceTut = null;
            }       
            boosterCanvas.sortingOrder = 0;
            handTut.SetActive(false);       
        }

        public void ShowEffect(Action complete, BoosterType type, Vector3 startPosition)
        {
            if(type != boosterType) return;
            Debug.Log($"Show effect {startPosition}");
            onCompleteForceTut = complete;
            onCompleteForceTut?.Invoke();
            boosterCanvas.sortingOrder = 10;
            goEffect.SetActive(true);
            goEffect.transform.position = startPosition;
            goEffect.transform.localScale = Vector3.one * 1.5f;
            goEffect.transform.DOScale(Vector3.one, .4f).SetEase(Ease.InBack).SetDelay(.5f).OnComplete(() => { });
            goEffect.transform.DOLocalMove(Vector3.zero, .4f).SetDelay(0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                transform.DOScale(Vector3.one * 1.25f, .15f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    transform.DOScale(Vector3.one * 1.1f, .15f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        DOVirtual.DelayedCall(0.3f, () => { handTut.SetActive(true); });
                    });
                });
                vfx.SetActive(true);
                goEffect.SetActive(false);
                lockIcon.SetActive(false);
                visualBooster.SetActive(true);
                
                Debug.Log($"Booster Type {boosterType}");
                if (boosterType == BoosterType.ADD_TRAY)
                    UserDataManager.FirstClaimAddTrayBooster = true;
                else if (boosterType == BoosterType.HAND_MOVE)
                    UserDataManager.FirstClaimHandBooster = true;
                else if (boosterType == BoosterType.SHUFFLE)
                    UserDataManager.FirstClaimShuffleBooster = true;
                else if (boosterType == BoosterType.MAGNET)
                    UserDataManager.FirstClaimMagnetBooster = true;
            });
        }    
    }   
}