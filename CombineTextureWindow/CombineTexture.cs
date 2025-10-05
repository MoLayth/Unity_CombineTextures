using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CombineTexture : EditorWindow {
    [SerializeField] VisualTreeAsset visualTree;
    [SerializeField] Material CRTMatreial;
    [SerializeField] CustomRenderTexture CustomRenderTexture;

    [SerializeField] CompineTextureWindow_Data data;
    [SerializeField] CompineTexture_SuffixToChanal SuffixToChanal;

    DropdownField resDropDown;
    Label selectedFolders;
    TextField newTextureSuffix;

    [MenuItem("Tools/Combine Texture")]
    public static void ShowWindow() {
        var wnd = GetWindow<CombineTexture>();
        wnd.titleContent = new GUIContent("Combine Texture");
        wnd.Show();
    }

    public void CreateGUI() {
        if (visualTree != null) {
            VisualElement root = rootVisualElement;
            visualTree.CloneTree(root);

            ObjectField r = root.Q<ObjectField>("rField");
            ObjectField b = root.Q<ObjectField>("bField");
            ObjectField g = root.Q<ObjectField>("gField");
            ObjectField a = root.Q<ObjectField>("aField");

            r.objectType = typeof(Texture2D);
            b.objectType = typeof(Texture2D);
            g.objectType = typeof(Texture2D);
            a.objectType = typeof(Texture2D);

            r.RegisterValueChangedCallback((evt) => {
                CRTMatreial.SetTexture("_R", (Texture2D)evt.newValue);
                OnTextureChange();
            });
            b.RegisterValueChangedCallback((evt) => {
                CRTMatreial.SetTexture("_B", (Texture2D)evt.newValue);
                OnTextureChange();
            });
            g.RegisterValueChangedCallback((evt) => {
                CRTMatreial.SetTexture("_G", (Texture2D)evt.newValue);
                OnTextureChange();
            });
            a.RegisterValueChangedCallback((evt) => {
                CRTMatreial.SetTexture("_A", (Texture2D)evt.newValue);                
                OnTextureChange();
            });

            root.Q<Button>("SaveButton").RegisterCallback<ClickEvent>((evt) => { Creat(); });
            root.Q<Button>("AutoCreatButton").RegisterCallback<ClickEvent>((evt) => { AutoCreat(); });

            resDropDown = root.Q<DropdownField>("ResDropDown");

            selectedFolders = root.Q<Label>("selectedFoldersCount");

            newTextureSuffix = root.Q<TextField>("TextureSuffix");
            newTextureSuffix.value = data.TextureSuffix;

            if (selectedFolders != null)
                selectedFolders.text = "Folders: " + GetSelectedFolders().Length.ToString();


            resDropDown.index = data.SelecetdRes;
            resDropDown.RegisterValueChangedCallback((evt) => {
                SetRes(resDropDown.index);
            });

            Cleanup();
        }
        else {
            rootVisualElement.Add(new Label("Assign a VisualTreeAsset in the inspector"));
        }
    }

    private void OnSelectionChange() {
        if (selectedFolders != null)
            selectedFolders.text = "Folders: " + GetSelectedFolders().Length.ToString();
    }

    private void OnDestroy() {
        data.TextureSuffix = newTextureSuffix.value;
        data.SelecetdRes = resDropDown.index;
    }


    public void OnTextureChange() {
        CustomRenderTexture.Update();
    }
    private void Cleanup() {
        CRTMatreial.SetTexture("_R", null);
        CRTMatreial.SetTexture("_G", null);
        CRTMatreial.SetTexture("_B", null);
        CRTMatreial.SetTexture("_A", null);
        CustomRenderTexture.Update();
    }
    private void Creat() {
        SaveOutputTexture();
    }
    private async void AutoCreat() {
        string[] folders = GetSelectedFolders();
        for (int i = 0; i < folders.Length; i++) {
            CRTMatreial.SetTexture("_R", FindTextureWithSuffix(folders[i], SuffixToChanal._R));
            CRTMatreial.SetTexture("_G", FindTextureWithSuffix(folders[i], SuffixToChanal._G));
            CRTMatreial.SetTexture("_B", FindTextureWithSuffix(folders[i], SuffixToChanal._B));
            CRTMatreial.SetTexture("_A", FindTextureWithSuffix(folders[i], SuffixToChanal._A));
            CustomRenderTexture.Update(1);

            string folderName = new DirectoryInfo(folders[i]).Name;
            string suffix = string.Empty;
            if (!string.IsNullOrEmpty(newTextureSuffix.value))
                suffix = newTextureSuffix.value + ".png";

            await System.Threading.Tasks.Task.Delay(250);

            SaveOutputTexture(folders[i], folderName + suffix);
        }
    }
    private void SaveOutputTexture() {
        string currentPath = AssetDatabase.GetAssetPath(CRTMatreial.GetTexture("_R"));
        string folder = Path.GetDirectoryName(currentPath);

        string suffix = "";
        if (!string.IsNullOrEmpty(newTextureSuffix.value))
            suffix = newTextureSuffix.value;

        string path = EditorUtility.SaveFilePanel("Save File", folder, suffix, "png");

        if (!string.IsNullOrEmpty(path)) {
            RenderTexture.active = CustomRenderTexture;
            Texture2D result = new Texture2D(CustomRenderTexture.width, CustomRenderTexture.height, TextureFormat.ARGB32, false,true);
            result.ReadPixels(new Rect(0, 0, CustomRenderTexture.width, CustomRenderTexture.height), 0, 0);
            result.Apply();
            RenderTexture.active = null;

            File.WriteAllBytes(path, result.EncodeToPNG());
            AssetDatabase.Refresh();

            // disable sRGB and compression
            string assetPath = "Assets" + path.Substring(Application.dataPath.Length);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            importer.sRGBTexture = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();

            Debug.Log("Saved texture to: " + path);
        }
    }
    private void SaveOutputTexture(string path,string tetxureName) {
        if (!string.IsNullOrEmpty(path)) {
            string FullPath = Path.Combine(path, tetxureName);
            RenderTexture.active = CustomRenderTexture;
            Texture2D result = new Texture2D(CustomRenderTexture.width, CustomRenderTexture.height, TextureFormat.ARGB32, false, true);
            result.ReadPixels(new Rect(0, 0, CustomRenderTexture.width, CustomRenderTexture.height), 0, 0);
            result.Apply();
            RenderTexture.active = null;

            File.WriteAllBytes(FullPath, result.EncodeToPNG());
            AssetDatabase.Refresh();

            // disable sRGB and compression
            TextureImporter importer = AssetImporter.GetAtPath(FullPath) as TextureImporter;
            importer.sRGBTexture = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();

            Debug.Log("Saved texture to: " + FullPath );
        }
    }
    private void SetRes(int index) {
        // 0 => 250X250
        // 1 => 1024X1024
        // 2 => 2048X2048
        // 3 => 4096X4096
        switch (index) {
            case 0:
                CustomRenderTexture.width = 250;
                CustomRenderTexture.height = 250;
                break;
            case 1:
                CustomRenderTexture.width = 1024;
                CustomRenderTexture.height = 1024;
                break;
            case 2:
                CustomRenderTexture.width = 2048;
                CustomRenderTexture.height = 2048;
                break;
            case 3:
                CustomRenderTexture.width = 4096;
                CustomRenderTexture.height = 4096;
                break;
        }
        CustomRenderTexture.Update();
    }
    private string[] GetSelectedFolders() {
        HashSet<string> uniqueFolders = new HashSet<string>();

        foreach (Object obj in Selection.GetFiltered<Object>(SelectionMode.Assets)) {
            string path = AssetDatabase.GetAssetPath(obj);

            if (string.IsNullOrEmpty(path))
                continue;

            if (Directory.Exists(path)) {
                // It's a folder
                uniqueFolders.Add(path);
            }
        }

        return new List<string>(uniqueFolders).ToArray();
    }
    private Texture2D FindTextureWithSuffix(string folderPath, string[] suffixes) {
        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            return null;

        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });

        foreach (string guid in guids) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(assetPath);

            foreach (string suffix in suffixes) {
                if (fileName.EndsWith(suffix, System.StringComparison.OrdinalIgnoreCase)) {
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                }
            }
        }

        return null;
    }
}
