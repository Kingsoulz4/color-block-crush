using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yoolax.Framework;

namespace Analytics
{
    [RequireComponent (typeof(Button))]
    public class ButtonClickPlacement : MonoBehaviour
    {
        private Button btn;
        [SerializeField] private string button_name;       

        private void Awake()
        {            
            btn = GetComponent<Button>();
            btn.onClick.AddListener(LogClickEvent);
        }

        private void LogClickEvent()
        {
            ButtonAnalyticStruct buttonClickStruct = new ButtonAnalyticStruct(button_name, SceneManager.GetActiveScene().name);
            Server.Get<OnButtonClickEventLog>().Dispatch(buttonClickStruct);
        }
    }
}
