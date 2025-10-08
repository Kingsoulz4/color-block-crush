using SFB;
using System.IO;
using UnityEngine;

namespace ColorBlockCrush.Tools
{
    public class LevelEditorController : MonoBehaviour
    {
        [Header("MVC")] 
        [SerializeField] private LevelEditorModel model;
        [SerializeField] private LevelEditorView view;
        
        [Header("Controllers")]
        public string inputTexturePath;
        public Texture2D inputTexture2D;
        public ColorType[,] currentColorArray;
        public int[] currentPixelCountByColor;
        
        [SerializeField] private LevelConfig currentLevelConfig;

        private void Awake()
        {
            InitButtons();
        }

        private void InitButtons()
        {
            view.ButtonChooseImage.onClick.AddListener(OpenImageFileBrowser);
        }
        
        #region Choose Input Image

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
                //UpdateAllowColor();
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
