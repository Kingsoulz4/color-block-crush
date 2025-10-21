using System;
using ColorBlockCrush;
using UnityEngine;
using UnityEngine.UI;

public enum PopupSettingType
{
    IN_GAME,
    HOME
}

public class PopupSetting : PopupUI
{
    [SerializeField] Button btn_Close;
    [SerializeField] Button btn_Restore;
    [SerializeField] Button m_buttonExitGame;
    [SerializeField] Button m_buttonRestart;
    [SerializeField] Button m_buttonContact;
    [SerializeField] Button m_buttonPrivacySetting;
    [SerializeField] Text versionTxt;
    [SerializeField] private Button debugBtn;
    private float lastTimeDebugClick;
    private int debugClickCount = 0;

    [SerializeField] RectTransform btnVerticalLayout;
    [SerializeField] RectTransform bgVerticalLayout;
    
    private void Awake()
    {
        m_buttonExitGame.onClick.AddListener(OnClickExitGame);
        m_buttonRestart.onClick.AddListener(OnClickRestartGame);
        m_buttonPrivacySetting.onClick.AddListener(OpenPrivacySetting);
        m_buttonContact.onClick.AddListener(ContactUs);
        versionTxt.text = Application.version;
    }

    private void Start()
    {
        debugBtn.GetComponent<Image>().color = TestManager.IsCheating ? Color.white : new Color(1, 1, 1, 0);
        debugBtn.onClick.AddListener(OnDebugClick);
    }

    private void OnClickExitGame()
    {
        // if (LevelManager.Instance.LevelGame.IsFirstClick)
        // {
            var popupConfirmLeave = UIManager.Instance.ShowPopup<PopupConfirmLeave>(null);
            popupConfirmLeave.OnConfirm = () =>
            {
                var loading = UIManager.Instance.ShowScreen<LoadingScreen>();
                loading.Show(() =>
                {
                    UIManager.Instance.ShowScreen<MainScreenUI>();
                });
                Hide();
                HeartManager.UseHeart(1);
                
            };
        //}
        // else
        // {
        //     Hide();
        //     UIManager.Instance.ShowScreen<MainScreenUI>();
        // }    
    }

    private void OnClickRestartGame()
    {
        var popupConfirmLeave = UIManager.Instance.ShowPopup<PopupConfirmLeave>(null);
        popupConfirmLeave.SetTextButtonConfirm("Retry");
        popupConfirmLeave.OnConfirm = () =>
        {
            LevelManager.Instance.OnRetryGame();
            Hide();
            HeartManager.UseHeart(1);
        };
        popupConfirmLeave.OnClose = () => { };
    }

    private void ContactUs()
    {
        EmailComposer.ComposeEmail(
            to:  new[] { "tuandt@android.vn" },
            cc:  new[] { "ledinhcuong248@gmail.com" },
            bcc: null,
            subject: "Color Block Crush Support",
            body:
            "Hello! Nice To meet you.\n\n" +
            "— Sent from MyUnityApp"
        );
    }

    private void OpenPrivacySetting()
    {
        Application.OpenURL("https://sites.google.com/view/amobear-privacy-policy/home");
    }

    public void SetType(PopupSettingType type)
    {
        m_buttonExitGame.gameObject.SetActive(type == PopupSettingType.IN_GAME);
        m_buttonRestart.gameObject.SetActive(type == PopupSettingType.IN_GAME);
        
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(btnVerticalLayout);
        LayoutRebuilder.ForceRebuildLayoutImmediate(bgVerticalLayout);
    }

    public override void Initialize(UIManager manager)
    {
        base.Initialize(manager);
        btn_Close.onClick.AddListener(Hide);
        btn_Restore.onClick.AddListener(Resrote);
    }

    public void Resrote()
    {
        UIManager.Instance.CheckRestore();
        Hide();
    }
    
    private void OnDebugClick()
    {
        if (Time.time - lastTimeDebugClick < 1)
        {
            debugClickCount++;
            Debug.Log(debugClickCount);
            if (debugClickCount >= 30 || Application.isEditor)
            {
                TestManager.IsCheating = !TestManager.IsCheating;
                debugBtn.GetComponent<Image>().color = TestManager.IsCheating ? Color.white : new Color(1, 1, 1, 0);
                Debug.Log("active debug:" + TestManager.IsCheating, debugBtn);
                debugClickCount = 0;
                if (TestManager.Instance) TestManager.Instance.UpdateState();
            }
        }
        else
        {
            debugClickCount = 0;
        }

        lastTimeDebugClick = Time.time;
    }
}
