using SFB;
using System.IO;
using UnityEngine;
using System;
using System.Collections.Generic;

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
        public HashSet<ColorType> currentColorSet;
        public int[] currentPixelCountByColor;
        public Canvas canvas;
        
        [SerializeField] private LevelConfig currentLevelConfig;

        private void Awake()
        {
            InitButtons();
            InitEvents();

            UpdateTankLinesInfor();
        }

        private void Start()
        {
            string saveFolderPath = GetSaveFolderLevelPath();
            view.textFolderLevelPath.text = string.IsNullOrEmpty(saveFolderPath) ? "EMPTY!!!" : saveFolderPath;

            if (!string.IsNullOrEmpty(saveFolderPath))
            {
                UpdateItemLevel();
            }

            saveFolderPath = GetSaveFolderPicturePath();
            view.textFolderPicturePath.text = string.IsNullOrEmpty(saveFolderPath) ? "EMPTY!!!" : saveFolderPath;
            view.levelSelectPanel.SetActive(true);
            view.levelEditPanel.SetActive(false);
        }

        private void InitButtons()
        {
            #region ChooseLevel

            view.buttonBrowerLevelFolder.onClick.AddListener(() =>
            {
                OnClickBrowseLevelFolder();
            });

            view.buttonOpenLevelFolder.onClick.AddListener(() =>
            {
                OnClickOpenLevelFolder();
            });

            view.buttonBrowerPictureFolder.onClick.AddListener(() =>
            {
                OnClickBrowseFolderPicture();
            });

            view.buttonOpenPictureFolder.onClick.AddListener(() =>
            {
                OnClickOpenFolderPicture();
            });
            
            view.newLevelButton.onClick.AddListener(() =>
            {
                string saveFolder = GetSaveFolderLevelPath();
                if (string.IsNullOrEmpty(saveFolder))
                {
                    Debug.LogError("PLEASE CHOOSE A FOLDER TO SAVE LEVEL DATA!");
                    return;
                }
                OnOpenLevel();
            });
            
            view.buttonSaveLevel.onClick.AddListener(() =>
            {
               OnSaveLevel();
            });
            
            view.buttonExitLevel.onClick.AddListener(() =>
            {
                OnExitLevel();
            });
            
            view.inputLevelId.onEndEdit.AddListener((string value) =>
            {
                if (string.IsNullOrEmpty(value))
                {
                    Debug.LogError("Invalid level id");
                    return;
                }

                int levelId = int.Parse(value);

                if (levelId <= 0)
                {
                    Debug.LogError("Invalid level id");
                }
                currentLevelConfig.levelId = levelId;
            });

            #endregion
            
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
            UpdateCurrentColorChoose(ColorType.PowderPink);
            
            for (int i = 0; i < view.buttonChooseDragTypes.Count; i++)
            {
                view.buttonChooseDragTypes[i].Init(UpdateCurrentDragType);
            }
            UpdateCurrentDragType(DragType.Normal);
            
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

                GunConfig newTankConfig = new GunConfig();
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
            
            UpdateCurrentColorChooseTank(ColorType.PowderPink);

            for (int i = 0; i < view.buttonTankTunnelQueueColorChooses.Count; i++)
            {
                view.buttonTankTunnelQueueColorChooses[i].Init(UpdateCurrentColorChooseTankTunnelQueue);
            }
            
            UpdateCurrentColorChooseTankTunnelQueue(ColorType.PowderPink);
            
            #endregion
        }

        private void InitEvents()
        {
            onUpdateSelection = SetStateGridCell;
            onDeleteSelection = DeleteStateGridCell;
        }

        #region Choose Level
        
        private List<LevelConfig> _levelConfigs = new List<LevelConfig>();
        private List<ItemLevelView> itemLevelViews = new List<ItemLevelView>();
        [SerializeField] private int oldId;
        
        private void OnClickBrowseLevelFolder()
        {
            string folderPath = OpenFolderBrowser();
            SetSaveFolderLevelPath(folderPath);
            view.textFolderLevelPath.text = folderPath;
            UpdateItemLevel();
        }

        private void OnClickOpenLevelFolder()
        {
            string folderPath = GetSaveFolderLevelPath();

            if (string.IsNullOrEmpty(folderPath))
            {
                Debug.LogError("PLEASE CHOOSE A FOLDER FIRST!");
                return;
            }

            if (!System.IO.Directory.Exists(folderPath))
            {
                Debug.LogError("Folder does not exist: " + folderPath);
                return;
            }

            try
            {
#if UNITY_STANDALONE_WIN
                System.Diagnostics.Process.Start("explorer.exe", folderPath.Replace("/", "\\"));
#elif UNITY_STANDALONE_OSX
            System.Diagnostics.Process.Start("open", folderPath);
#elif UNITY_STANDALONE_LINUX
            System.Diagnostics.Process.Start("xdg-open", folderPath);
#else
            Debug.LogWarning("Opening folder not supported on this platform.");
#endif
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to open folder: " + ex.Message);
            }
        }
        
        private void OnClickBrowseFolderPicture()
        {
            string folderPath = OpenFolderBrowser();
            SetSaveFolderPicturePath(folderPath);
            view.textFolderPicturePath.text = folderPath;        
        }

        private void OnClickOpenFolderPicture()
        {
            string folderPath = GetSaveFolderPicturePath();

            if (string.IsNullOrEmpty(folderPath))
            {
                Debug.LogError("PLEASE CHOOSE A FOLDER FIRST!");
                return;
            }

            if (!System.IO.Directory.Exists(folderPath))
            {
                Debug.LogError("Folder does not exist: " + folderPath);
                return;
            }

            try
            {
#if UNITY_STANDALONE_WIN
                System.Diagnostics.Process.Start("explorer.exe", folderPath.Replace("/", "\\"));
#elif UNITY_STANDALONE_OSX
            System.Diagnostics.Process.Start("open", folderPath);
#elif UNITY_STANDALONE_LINUX
            System.Diagnostics.Process.Start("xdg-open", folderPath);
#else
            Debug.LogWarning("Opening folder not supported on this platform.");
#endif
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to open folder: " + ex.Message);
            }
        }
        
        public string OpenFolderBrowser()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            string path = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", false)[0];
            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log("Selected folder: " + path);
                return path;
            }
#endif
            return null;
        }

        public string GetSaveFolderLevelPath()
        {
            return PlayerPrefs.GetString("LevelSavedFolderKey", "");
        }    

        private void SetSaveFolderLevelPath(string path)
        {
            PlayerPrefs.SetString("LevelSavedFolderKey", path);
        }

        private void OnOpenLevel(LevelConfig levelConfig = null)
        {
            Debug.Log("Open Level");
            currentLevelConfig = levelConfig;

            if (currentLevelConfig == null)
            {
                currentLevelConfig = CreateNewLevelConfig();
            }
            
            oldId = currentLevelConfig.levelId;
            view.inputLevelId.text = currentLevelConfig.levelId.ToString();
            view.difficultDropdown.value = (int)currentLevelConfig.levelDifficult;
            // view.truePictureImage.text = Path.GetFileNameWithoutExtension(currentLevelConfig.imageConfig.SavePath) != null
            //     ?  Path.GetFileNameWithoutExtension(currentLevelConfig.imageConfig.SavePath) : null;
            if (levelConfig != null)
            {
                inputTexturePath = levelConfig.imageConfig != null ? levelConfig.imageConfig.SavePath : "";
                if (!string.IsNullOrEmpty(levelConfig.imageConfig.SavePath))
                {
                    UpdateInputTexture();
                }

                UpdateMapData();
                UpdateTankLineData();
                
                UpdateLevelState();
            }
            
            Debug.Log("Open Complete");
            
            view.levelSelectPanel.gameObject.SetActive(false);
            view.levelEditPanel.gameObject.SetActive(true);
        }

        private void OnSaveLevel()
        {
            SaveLevelData();
           OnExitLevel();
        }

        private void OnExitLevel()
        {
            currentLevelConfig = null;
                
            ClearAllMap();
            ClearAllTankLinesInfor();
            view.levelSelectPanel.gameObject.SetActive(true);
            view.levelEditPanel.gameObject.SetActive(false);
            
            UpdateItemLevel();
        }
        
        private void SaveLevelData()
        {
            currentLevelConfig.ValidateData();

            for (int i = 0; i < view.tankLineViews.Count; i++)
            {
                List<ItemTankLineElementView> elements = view.tankLineViews[i].GetElementView();
                
                currentLevelConfig.gunLines[i] = new GunLineConfig();
                for (int j = 0; j < elements.Count; j++)
                {
                    currentLevelConfig.gunLines[i].gunLineElementConfigs.Add(elements[j].elementConfig);
                }
            }
            if (oldId != currentLevelConfig.levelId)
            {
                RenameFile(oldId, currentLevelConfig.levelId);
                oldId = currentLevelConfig.levelId;
            }
            
            SaveLevelConfigEnCrypt(currentLevelConfig);
        }
        
        private void RenameFile(int oldId, int newId)
        {
            string tail = "bytes";
            string saveFolder = GetSaveFolderLevelPath();
            string oldName = Path.Combine(saveFolder, $"Level_{oldId}.{tail}");
            string newName = Path.Combine(saveFolder, $"Level_{newId}.{tail}");

            if (File.Exists(oldName))
            {
                try
                {
                    if (File.Exists(newName))
                        File.Delete(newName);

                    File.Move(oldName, newName);
                    Debug.Log($"[RenameJsonFile] Renamed {oldName} → {newName}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[RenameJsonFile] Failed to rename JSON file: {e}");
                }
            }
        }

        private LevelConfig CreateNewLevelConfig(int levelIndex = -1)
        {
            string folderPath = GetSaveFolderLevelPath() + "/";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Determine the next index for the level file
            string[] existingAssets = /* ResourcesManager.Instance.collectByJson */
            //     ? Directory.GetFiles(folderPath, "Level_*.json")
               /* : */ Directory.GetFiles(folderPath, "Level_*.bytes");
            int maxIndex = 0;
            foreach (string path in existingAssets)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                if (fileName.StartsWith("Level_") && int.TryParse(fileName.Substring(6), out int index))
                {
                    maxIndex = Mathf.Max(maxIndex, index);
                }
            }

            int nextIndex = levelIndex > 0 ? levelIndex : maxIndex + 1;
            string newFileName = $"Level_{nextIndex.ToString("D3")}.asset";
            string assetPath = Path.Combine(folderPath, newFileName);

            // Create the new LevelConfig
            LevelConfig newLevel = ScriptableObject.CreateInstance<LevelConfig>();
            newLevel.levelId = nextIndex;
            newLevel.gunLines = new List<GunLineConfig>();
            for (int i = 0; i < 5; i++)
            {
                GunLineConfig tankLineConfig = new GunLineConfig();
                tankLineConfig.gunLineElementConfigs  = new List<GunLineElementConfig>();
                newLevel.gunLines.Add(tankLineConfig);
            }
            
            Debug.Log("tank Congif " + newLevel.gunLines.Count);

            UpdateItemLevel();
            // if (ResourcesManager.Instance.collectByJson) SaveLevelConfigToJson(newLevel);
            /*else*/ SaveLevelConfigEnCrypt(newLevel);
            return newLevel;
        }
        
        private void UpdateItemLevel()
    {
        foreach (ItemLevelView itemLevelView in itemLevelViews)
        {
            Destroy(itemLevelView.gameObject);
        }

        itemLevelViews.Clear();
        string saveFolderPath = GetSaveFolderLevelPath();
        if (string.IsNullOrEmpty(saveFolderPath))
        {
            Debug.LogError("PLEASE CHOOSE A FOLDER TO SAVE LEVEL DATA!");
            return;
        }
        CollectLevelConfigs(saveFolderPath);
        foreach (LevelConfig levelConfig in _levelConfigs)
        {
            if (levelConfig == null || levelConfig.name == null)
            {
                Debug.LogError("ERROR LEVEL CONFIG");
                continue;
            }
            string fileName = levelConfig.name; // e.g. Level_001

            // Try to extract the number after "Level_"
            if (fileName.StartsWith("Level_"))
            {
                string numberPart = fileName.Substring("Level_".Length); // "001"
                if (int.TryParse(numberPart, out int id))
                {
                    levelConfig.levelId = id;
                }
            }

            ItemLevelView itemLevelView = Instantiate(model.itemLevelPrefab).GetComponent<ItemLevelView>();
            itemLevelView.iconWarning.SetActive(!levelConfig.ValidateData());
            itemLevelView.rectTransform.SetParent(view.levelItemContainer, false);
            itemLevelView.textLevel.text = "Level " + levelConfig.levelId.ToString();
            itemLevelView.levelConfig = levelConfig;
            itemLevelView.buttonOpen.onClick.RemoveAllListeners();
            itemLevelView.buttonOpen.onClick.AddListener(() =>
            {
                OnOpenLevel(itemLevelView.levelConfig);
            });

            itemLevelView.buttonCopy.onClick.RemoveAllListeners();
            // itemLevelView.buttonCopy.onClick.AddListener(() =>
            // {
            //     OnCopyLevel(itemLevelView.LevelConfig);
            // });

            itemLevelViews.Add(itemLevelView);
        }
    }
        
        public void CollectLevelConfigs(string absoluteFolderPath)
        {
            _levelConfigs = new List<LevelConfig>();

            if (!Directory.Exists(absoluteFolderPath))
            {
                Debug.LogError($"Directory does not exist: {absoluteFolderPath}");
                return;
            }

            string[] contentFiles = /*!collectByJson ? */
                Directory.GetFiles(absoluteFolderPath, "*.bytes", SearchOption.TopDirectoryOnly);
                // : Directory.GetFiles(absoluteFolderPath, "*.json", SearchOption.TopDirectoryOnly);

            foreach (string filePath in contentFiles)
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    json = QT.DecryptAndDecompress(json, LevelEditorController.pass);
                    LevelConfig configData = FromJson(json);

                    if (configData == null)
                    {
                        Debug.LogWarning($"Failed to parse: {filePath}");
                        continue;
                    }

                    _levelConfigs.Add(configData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error reading file: {filePath}\n{ex.Message}");
                }
            }

            Debug.Log($"Collected {_levelConfigs.Count} Level Collections from BYTES.");
        }
        
        public LevelConfig FromJson(string json)
        {
            LevelConfig config = ScriptableObject.CreateInstance<LevelConfig>();
            JsonUtility.FromJsonOverwrite(json, config);
            return config;
        }
        
        public static string pass = "colorblock@012356789";
        private void SaveLevelConfigEnCrypt(LevelConfig levelConfig)
        {
            // Convert to JSON
            string json = JsonUtility.ToJson(levelConfig, true);

            //Encrypt
            string qt = QT.EncryptAndCompress(json, pass);

            // Compose filename and save path
            string fileName = $"Level_{levelConfig.levelId}.bytes";
            string saveFolderPath = GetSaveFolderLevelPath();
            string fullPath = System.IO.Path.Combine(saveFolderPath, fileName);

            // Write to file
            try
            {
                System.IO.File.WriteAllText(fullPath, qt);
                Debug.Log($"Level data saved to {fullPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save level Bytes: {e}");
            }
        }

        #endregion
        
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

            UpdateInputTexture(true);
        }
        
        private void UpdateInputTexture(bool genMap = false)
        {
            inputTexture2D = LoadTextureFromAbsolutePath(inputTexturePath);
            Debug.Log("Load Texture 1 " + inputTexture2D);
            if (inputTexture2D == null) inputTexture2D = LoadTextureFromResources(inputTexturePath);
            Debug.Log("Load Texture 2 " + inputTexture2D);

            if (inputTexture2D != null)
            {
                Debug.Log("Image loaded and set as Sprite.");
                currentPixelCountByColor = new int[inputTexture2D.width * inputTexture2D.height];
                currentColorArray = ImageUtils.QuantizeTexture(inputTexture2D, currentPixelCountByColor, out currentColorSet);
                
                Debug.Log("Color Number " + currentColorSet.Count);

                Texture2D resultTex = ImageUtils.GenerateTextureFromColorArray(currentColorArray);
                // Convert Texture2D to Sprite
                Sprite sprite = Sprite.Create(
                    resultTex,
                    new Rect(0, 0, resultTex.width, resultTex.height),
                    new Vector2(0.5f, 0.5f), 32 // pivot center
                );

                //Set input image info
                currentLevelConfig.imageConfig = new InputImageConfig();
                currentLevelConfig.imageConfig.SavePath = inputTexturePath;
                currentLevelConfig.imageConfig.ColorFlatMap = ImageUtils.Flatten2DArray(currentColorArray,
                    out currentLevelConfig.imageConfig.ImageSize);
                currentLevelConfig.imageConfig.pixelCountByColor = currentPixelCountByColor;
                if (genMap)
                {
                    ClearAllMap();
                    CreateMapByPicture();   
                }
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

        public void UpdateLevelState()
        {
            UpdateButtonChooseTankColor();
            UpdateButtonChooseTunnelQueueColor();
            UpdateBlockBulletValidate();
        }
    }
}
