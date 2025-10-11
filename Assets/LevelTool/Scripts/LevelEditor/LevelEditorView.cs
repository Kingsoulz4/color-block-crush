using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    public class LevelEditorView : MonoBehaviour
    {
        [Header("Menu Panel")]
        public RectTransform levelItemContainer;
        public Button newLevelButton;
        public GameObject levelSelectPanel;
        public Button buttonBrowerLevelFolder;
        public Button buttonOpenLevelFolder;
        public TextMeshProUGUI textFolderLevelPath;
        public Button buttonBrowerPictureFolder;
        public Button buttonOpenPictureFolder;
        public TextMeshProUGUI textFolderPicturePath;
        
        [Header("Level Infor")]
        public GameObject levelEditPanel;
        public TMP_InputField inputLevelId;
        public TMP_Dropdown difficultDropdown; 
        public TextMeshProUGUI truePictureImage;
        public Button buttonSaveLevel;
        
        [Header("Map")] 
        public Button buttonCreateMap;
        public TMP_InputField WidthMapSize;
        public TMP_InputField HeightMapSize;
        public DragCellMapSelection dragCellMapSelection;

        public Button buttonSetColor;
        public Button buttonClearAll;
        public Button buttonDel;

        [Header("Properties Map")] 
        public List<ButtonChooseDragType> buttonChooseDragTypes;
        public List<ButtonColorChoose> buttonCellGridColorChooses =  new List<ButtonColorChoose>();
        
        [Header("Choose Image Area")]
        public Button ButtonChooseImage;
        public TextMeshProUGUI TruePictureImageTxt;
        
        [Header("Tank Line")]
        public List<TankLineEditorView> tankLineViews;
        public List<ButtonColorChoose> buttonTankColorChooses =  new List<ButtonColorChoose>();
        
        public Toggle toggleTankLock;
        public Toggle toggleTankHidden;
        public TMP_InputField bulletInputField;

        public Button buttonSetTankInfor;
        public Button buttonDelTankInfor;
        public Button buttonClearAllTankSelected;

        public Transform uiLineParent;
        public Button buttonSetLineConnect;
        public Button buttonDelLineConnect;

        public TextMeshProUGUI tankTunnelQueueNumber;
        public List<ButtonColorChoose> buttonTankTunnelQueueColorChooses =  new List<ButtonColorChoose>();
        public TMP_InputField bulletTankTunnelQueueInputField;
        public Button buttonAddTunnelItemQueue;
        public Transform tunnelElementQueueParent;
        public Button buttonSetTunnelInfor;
        public Button buttonDelTunnel;

        public TextMeshProUGUI tankNumberTxtValue;
        public TextMeshProUGUI tankHiddenNumberTxtValue;
        public TextMeshProUGUI tankLockNumberTxtValue;
        public TextMeshProUGUI tankConnectionNumberTxtValue;
        public TextMeshProUGUI tunneNumberTxtValue;
        public TextMeshProUGUI tankColorNumberTxtValue;
    }
}
