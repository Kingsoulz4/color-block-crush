using DG.Tweening;
using ColorBlockCrush;
using System;
using System.Collections;
using System.Collections.Generic;
using ColorBlockCrush.Tools;
using UnityEngine;
using UnityEngine.UI;

public class MainScreenUI : ScreenUI
{
    [Space, Header("UI")]
    [SerializeField] Button btn_Play;
    [SerializeField] Text txt_Level;
    //[SerializeField] ChangeThemeButtonPlay changeThemeButtonPlay;
    [SerializeField] AudioClip soundBG;
    [SerializeField] ScrollScreenHorizontal horizontal;
    [SerializeField] MenuTabSystem menuTab;
    [SerializeField] GameObject blockUI;
    [SerializeField] Text[] arrTextLevel;
    [SerializeField] private GameObject bigBgHardLevel;
    [SerializeField] private GameObject bigBgSuperHardLevel;
    [SerializeField] GameObject arrBgHardLevel;
    [SerializeField] GameObject arrBgSuperHardLevel;
    [SerializeField] Button btn_RemoveAds;
    [SerializeField] Button btn_StarterPackage;
    [SerializeField] GameObject objOtherGame;
    [SerializeField] ButtonMainBase[] listButtonPackage;

    [Space, Header("Visual")]
    [SerializeField] RectTransform rect_Left;
    [SerializeField] RectTransform rect_Right;
    [SerializeField] RectTransform rect_Center;
    [Header("Visual - Reward Gold")]
    [SerializeField] GameObject obj_GoldAnimationPrefab;
    [SerializeField] RectTransform rect_TargetGoldPos;
    [SerializeField] RectTransform rect_TargetGoldPosShop;
    [SerializeField] ParticleSystem fx_Gold;
    [SerializeField] Text txt_Gold;
    [SerializeField] Text txt_GoldShop;
    [SerializeField] GoldDisplay m_goldBar;
    public float timeMoveCoinBack = 0.5f;
    public float timeMoveCoinUp = 0.75f;
    public AudioClip clip_SpawnItem;
    public AudioClip clip_GoldEnd;
    [Header("Visual - Reward Heart")]
    [SerializeField] RectTransform rect_TargetHeartPos;
    [Header("Visual - Reward Booster")]
    [SerializeField] RectTransform rect_TargetBoosterPos;


    public RectTransform RectTargetGold => rect_TargetGoldPos;
    public RectTransform RectTargetGoldShop => rect_TargetGoldPosShop;
    public RectTransform RectTargetHeart => rect_TargetHeartPos;
    public RectTransform RectTargetBooster => rect_TargetBoosterPos;
    public int CurrentPanel => horizontal.currentPanel;

    public override void Initialize(UIManager uiManager)
    {
        base.Initialize(uiManager);
        btn_Play.onClick.AddListener(PlayLevel);

        UpdateUI();
        
        UIManager.OnRefeshBannerAndAds += UpdateButtonRemoveAds;
        UpdateButtonRemoveAds();
        UpdateButtonStarterPack();
        btn_RemoveAds.onClick.AddListener(() =>
        {
            var popupRemoveAds = UIManager.Instance.ShowPopup<PopupRemoveAds>(null);
            //uiRemove.OnBuySS = deActionButtonRemoveAds;
        });
        btn_StarterPackage.onClick.AddListener(() =>
        {
            var popupStarterPack = UIManager.Instance.ShowPopup<PopupStarterPack>(null);
        });
    }
    
    public void UpdateUI()
    {
        //txt_Level.text = $"Level {LevelManager.Instance.CurrentLevel}";
        if (arrTextLevel != null)
        {
            for (int i = 0; i < arrTextLevel.Length; i++)
            {
                arrTextLevel[i].text = (LevelManager.Instance.CurrentLevel + i).ToString();
            }
        }

        LevelDifficult levelDiff = LevelManager.Instance.GetCurrentLevelType();
        if (levelDiff == LevelDifficult.Normal)
        {
            bigBgHardLevel.SetActive(false);
            bigBgSuperHardLevel.SetActive(false);
            arrBgHardLevel.SetActive(false);
            arrBgSuperHardLevel.SetActive(false);
        }else if (levelDiff == LevelDifficult.Hard)
        {
            bigBgHardLevel.SetActive(true);
            bigBgSuperHardLevel.SetActive(false);
            arrBgHardLevel.SetActive(true);
            arrBgSuperHardLevel.SetActive(false);
        }else if (levelDiff == LevelDifficult.SuperHard)
        {
            bigBgHardLevel.SetActive(false);
            bigBgSuperHardLevel.SetActive(true);
            arrBgHardLevel.SetActive(false);
            arrBgSuperHardLevel.SetActive(true);
        }
    }

    public void ResetVisual()
    {
           
    }
    private void UpdateButtonRemoveAds()
    {
        UpdateUI();
        btn_RemoveAds.gameObject.SetActive(!ShopManager.Instance.HasPurchasedNoAdsPack);
    }
    void deActionButtonRemoveAds()
    {
        btn_RemoveAds.gameObject.SetActive(false);
    }

    private void UpdateButtonStarterPack()
    {
        btn_StarterPackage.gameObject.SetActive(!ShopManager.Instance.HasPurchasedStarterPack);
    }

    public override void Active()
    {
        base.Active();
        UpdateUI();
        AudioManager.Instance.StopMusic("BG_Gameplay");
        AudioManager.Instance.PlayMusic("BG_Home", 1, true);
    }

    private void PlayLevel()
    {
        if (UserDataManager.Heart > 0)
        {
            var loading = UIManager.Instance.ShowScreen<LoadingScreen>();
            loading.Show(() =>
            {
                LevelManager.Instance.StartCurrentLevel();
                UIManager.Instance.ShowScreen<InGameScreenUI>();
            });
        }
        else
        {
            var popupGetMoreLives = UIManager.Instance.ShowPopup<PopupGetMoreLives>(null);
            popupGetMoreLives.OnRefilled = () =>
            {
                var loading = UIManager.Instance.ShowScreen<LoadingScreen>();
                loading.Show(() =>
                {
                    LevelManager.Instance.StartCurrentLevel();
                    UIManager.Instance.ShowScreen<InGameScreenUI>();
                });
            };
            popupGetMoreLives.OnClose = () =>
            {
                UIManager.Instance.ShowScreen<MainScreenUI>();
            };
        }
        //UIManager.Instance.ShowPopup<PopupSelectBooster>(null);
    }

    public void MoveCoin(int amount, string reason, string where)
    {
    }
    
    public void ShowClaimReward(int quantity)
    {
        m_goldBar.Sync = false;
        m_goldBar.SetText(UserDataManager.Gold - quantity);
        m_goldBar.gameObject.SetActive(false);
        var popupReceiveCoin = UIManager.Instance.ShowPopup<PopupReceiveCoin>(null);
        popupReceiveCoin.PlayCoinFX(m_goldBar.transform.position, Vector3.zero, quantity, () =>
        {
            m_goldBar.gameObject.SetActive(true);
            m_goldBar.SetText(UserDataManager.Gold);
            m_goldBar.Sync = true;
        });
    }    

    private void MoveValueTop(float timeDelay, GameObject objSpawn, RectTransform targetPos, int currentCoinText, bool isCoin, Action playFx, bool isFinish)
    {
        DOVirtual.DelayedCall(timeDelay + 0.25f, () =>
        {
            AudioManager.Instance.PlayOneShot(clip_SpawnItem, 1);
            objSpawn.SetActive(true);
            Vector3 spawnPos = rect_Left.position;
            spawnPos.x += Mathf.Round(UnityEngine.Random.Range(-7.5f, 7.5f) / 10f) * 10f;
            spawnPos.y += Mathf.Round(UnityEngine.Random.Range(-7.5f, 7.5f) / 10f) * 10f;
            objSpawn.transform.position = spawnPos;
            objSpawn.transform.localScale = Vector3.zero;
            Vector3 backPos = objSpawn.transform.position;
            backPos.x -= 5f;
            backPos.y -= 8.5f;

            Sequence MoveSequence = DOTween.Sequence();
            MoveSequence.Append(objSpawn.transform.DOScale(Vector3.one * (isCoin ? UnityEngine.Random.Range(1.2f, 1.25f) : 1.1f), 0.4f).SetEase(Ease.InOutQuad))
                        .Join(objSpawn.transform.DOMove(backPos, timeMoveCoinBack).SetEase(Ease.InOutQuad))
                        .Append(objSpawn.transform.DOMove(targetPos.position, timeMoveCoinUp).SetEase(Ease.InOutQuad))
                        .OnComplete(() =>
                        {
                            if (isCoin)
                            {
                                AudioManager.Instance.PlayOneShot(clip_GoldEnd, 1);
                            }
                            Sequence ScaleSequence = DOTween.Sequence();
                            ScaleSequence.Append(targetPos.DOScale(Vector3.one * 1.3f, 0.06f).SetEase(Ease.InBack))
                                        .Append(targetPos.DOScale(Vector3.one, 0.06f).SetEase(Ease.InBack));
                            Destroy(objSpawn);
                            playFx?.Invoke(); 
                            blockUI.gameObject.SetActive(false);
                            if (isFinish)
                            {
                                    
                            }
                        });
        });
    }

    public void PlayFx(ParticleSystem particleSystem)
    {
        ParticleSystem particleSystem1 = Instantiate(particleSystem, particleSystem.transform.position, Quaternion.identity, particleSystem.transform.parent);
        particleSystem1.gameObject.SetActive(true);
        particleSystem1.Play();
        DOVirtual.DelayedCall(2, () =>
        {
            if (particleSystem1)
            {
                Destroy(particleSystem1.gameObject);
            }
        });
    }

    public void RollToShopGold()
    {
        menuTab.ChangeTab(0);
        ShopScreenTab shopScreenTab = menuTab.menuTabCurrent as ShopScreenTab;
        if (shopScreenTab != null)
        {
            shopScreenTab.RollToShopGold();
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.SetGameState(GameState.MainMenu);
        btn_Play.GetComponent<ButtonPlay>().SetDisplayLevelType(LevelManager.Instance.GetCurrentLevelType());
    }

    private void OnDisable()
    {
        UIManager.OnRefeshBannerAndAds -= UpdateButtonRemoveAds;
        DOTween.Kill(this);
    }

    public RectTransform GetRectTargetGold()
    {
        if(CurrentPanel == 0)
        {
            return rect_TargetGoldPosShop;
        }
        return rect_TargetGoldPos;
    }

    public RectTransform GetRectTargetBooster()
    {
        if (CurrentPanel == 0)
        {
            return null;
        }
        return rect_TargetBoosterPos;
    }
}