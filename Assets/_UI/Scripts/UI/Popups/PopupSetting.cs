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
    [SerializeField] Button m_buttonContact;
    [SerializeField] Button m_buttonPrivacySetting;
    [SerializeField] Text versionTxt;

    private void Awake()
    {
        m_buttonExitGame.onClick.AddListener(OnClickExitGame);
        m_buttonPrivacySetting.onClick.AddListener(OpenPrivacySetting);
        m_buttonContact.onClick.AddListener(ContactUs);
        versionTxt.text = Application.version;
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
}
