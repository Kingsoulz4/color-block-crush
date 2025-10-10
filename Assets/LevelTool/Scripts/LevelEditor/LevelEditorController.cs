using SFB;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBlockCrush.Tools
{
    public partial class LevelEditorController : MonoBehaviour
    {
        [Header("MVC")] 
        [SerializeField] private LevelEditorModel model;
        [SerializeField] private LevelEditorView view;
        
        [Header("Controllers")]
        public string inputTexturePath;
        public Texture2D inputTexture2D;
        public ColorType[,] currentColorArray;
        public int[] currentPixelCountByColor;
        public Canvas canvas;
        
        [SerializeField] private LevelConfig currentLevelConfig;

        private void Awake()
        {
            InitButtons();
            InitEvents();

            UpdateTankLinesInfor();
        }

        private void InitButtons()
        {
            #region Map

            view.HeightMapSize.text = mapHeight.ToString();
            view.WidthMapSize.text = mapWidth.ToString();
            
            view.HeightMapSize.onEndEdit.AddListener(ValidateMapWHeight);
            view.WidthMapSize.onEndEdit.AddListener(ValidateMapWidth);
            view.buttonCreateMap.onClick.AddListener(CreateMapEmpty);
            
            view.buttonClearAll.onClick.AddListener(ClearAllColor);
            view.buttonSetColor.onClick.AddListener(SetColorSelected);
            view.buttonDel.onClick.AddListener(DeleteColorSelected);

            for (int i = 0; i < view.buttonCellGridColorChooses.Count; i++)
            {
                view.buttonCellGridColorChooses[i].Init(UpdateCurrentColorChoose);
            }
            UpdateCurrentColorChoose(ColorType.Pink);
            
            view.dragCellMapSelection.enabled = false;
            
            #endregion
            
            view.ButtonChooseImage.onClick.AddListener(OpenImageFileBrowser);

            #region Tank
            
            view.buttonSetTankInfor.onClick.AddListener(SetTankInfor);
            view.buttonDelTankInfor.onClick.AddListener(DellTankInfor);
            view.buttonClearAllTankSelected.onClick.AddListener(ClearAllTankLineELementSelected);
            
            view.buttonSetLineConnect.onClick.AddListener(SetConnectLine);
            view.buttonDelLineConnect.onClick.AddListener(DelConnectLine);
            
            view.buttonAddTunnelItemQueue.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(view.bulletTankTunnelQueueInputField.text))
                {
                    Debug.LogError("Invalid bullet number");
                    return;
                }
                
                int bulletNumber = int.Parse(view.bulletTankTunnelQueueInputField.text);

                if (bulletNumber <= 0)
                {
                    Debug.LogError("Invalid bullet number");
                    return;
                }

                TankConfig newTankConfig = new TankConfig();
                newTankConfig.bulletNumber = bulletNumber;
                newTankConfig.colorType = currentTankTunnelQueueColor;
                newTankConfig.isHidden = false;
                newTankConfig.hasLock = false;
                
                OnAddTankQueueToTunnel(newTankConfig);
            });
            view.buttonSetTunnelInfor.onClick.AddListener(SetTunnelInfor);
            view.buttonDelTunnel.onClick.AddListener(DellTunnelInfor);

            for (int i = 0; i < view.tankLineViews.Count; i++)
            {
                int index = i;
                view.tankLineViews[i].SetId(index);
                view.tankLineViews[i].buttonAddElement.onClick.RemoveAllListeners();
                view.tankLineViews[i].buttonAddElement.onClick.AddListener(() =>
                {
                    AddElementToLine(index);
                });
            }
            
            for (int i = 0; i < view.buttonTankColorChooses.Count; i++)
            {
                view.buttonTankColorChooses[i].Init(UpdateCurrentColorChooseTank);
            }
            
            UpdateCurrentColorChooseTank(ColorType.Pink);

            for (int i = 0; i < view.buttonTankTunnelQueueColorChooses.Count; i++)
            {
                view.buttonTankTunnelQueueColorChooses[i].Init(UpdateCurrentColorChooseTankTunnelQueue);
            }
            
            UpdateCurrentColorChooseTankTunnelQueue(ColorType.Pink);
            
            #endregion
        }

        private void InitEvents()
        {
            onUpdateSelection = UpdateColorGridCell;
            onDeleteSelection = DeleteColorGridCell;
        }
        
        #region Input Image

        public void OpenImageFileBrowser()
        {
            // In Editor, use OpenFilePanel to pick image files
            string[] paths = StandaloneFileBrowser.OpenFilePanel(
                "Select Image",
                "", // or set initial directory like @"C:\Users\..."
                new[] {
                    new ExtensionFilter("Image Files", "png", "jpg", "jpeg"),
                    new ExtensionFilter("All Files", "*" )
                },
                false
            );
            if (paths.Length > 0)
            {
                inputTexturePath = paths[0];
            }

            UpdateInputTexture();
        }
        
        private void UpdateInputTexture()
        {
            inputTexture2D = LoadTextureFromAbsolutePath(inputTexturePath);
            Debug.Log("Load Texture 1 " + inputTexture2D);
            if (inputTexture2D == null) inputTexture2D = LoadTextureFromResources(inputTexturePath);
            Debug.Log("Load Texture 2 " + inputTexture2D);

            if (inputTexture2D != null)
            {
                Debug.Log("Image loaded and set as Sprite.");
                currentPixelCountByColor = new int[inputTexture2D.width * inputTexture2D.height];
                currentColorArray = ImageUtils.QuantizeTexture(inputTexture2D, currentPixelCountByColor);

                Texture2D resultTex = ImageUtils.GenerateTextureFromColorArray(currentColorArray);
                // Convert Texture2D to Sprite
                Sprite sprite = Sprite.Create(
                    resultTex,
                    new Rect(0, 0, resultTex.width, resultTex.height),
                    new Vector2(0.5f, 0.5f), 32 // pivot center
                );

                //Set input image info
                currentLevelConfig = new LevelConfig();
                currentLevelConfig.imageConfig = new InputImageConfig();
                currentLevelConfig.imageConfig.SavePath = inputTexturePath;
                currentLevelConfig.imageConfig.ColorFlatMap = ImageUtils.Flatten2DArray(currentColorArray,
                    out currentLevelConfig.imageConfig.ImageSize);
                currentLevelConfig.imageConfig.pixelCountByColor = currentPixelCountByColor;
                //UpdateAllowColor();
                ClearAllMap();
                CreateMapByPicture();
            }
        }
        
        public static Texture2D LoadTextureFromAbsolutePath(string absolutePath)
        {
            if (!File.Exists(absolutePath))
            {
                Debug.LogError("File does not exist: " + absolutePath);
                return null;
            }

            try
            {
                // Read image data
                byte[] fileData = File.ReadAllBytes(absolutePath);

                // Create Texture2D with automatic size adjustment
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);

                if (tex.LoadImage(fileData)) // Will auto-resize the texture
                {
                    tex.filterMode = FilterMode.Point;
                    tex.wrapMode = TextureWrapMode.Clamp;
                    tex.Apply();

                    Debug.Log("Texture loaded successfully from: " + absolutePath);
                    return tex;
                }
                else
                {
                    Debug.LogError("Failed to decode image from: " + absolutePath);
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Exception loading texture: {ex.Message}");
                return null;
            }
        }
        
        public Texture2D LoadTextureFromResources(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("LoadTextureFromResources: empty fileName");
                return null;
            }        

            string nameNoExt = Path.GetFileNameWithoutExtension(fileName);
            
            string folder = GetSaveFolderPicturePath();
            string filePath = Path.Combine(folder, nameNoExt + ".png");       

            try
            {
                byte[] fileData = File.ReadAllBytes(filePath);
                
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);

                if (tex.LoadImage(fileData)) // Will auto-resize the texture
                {
                    tex.filterMode = FilterMode.Point;
                    tex.wrapMode = TextureWrapMode.Clamp;
                    tex.Apply();

                    Debug.Log("Texture loaded successfully from: " + filePath);
                    return tex;
                }
                else
                {
                    Debug.LogError("Failed to decode image from: " + filePath);
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Exception loading texture: {ex.Message}");
                return null;
            }

            //return tex;
        }
        
        public string GetSaveFolderPicturePath()
        {
            return PlayerPrefs.GetString("LevelSavedPictureFolderKey", "");
        }

        public void SetSaveFolderPicturePath(string path)
        {
            PlayerPrefs.SetString("LevelSavedPictureFolderKey", path);
        }

        #endregion
    }
}
