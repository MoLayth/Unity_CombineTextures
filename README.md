# Combine Texture Tool for Unity

This Unity Editor tool allows you to **pack multiple grayscale textures into a single texture** by assigning them to the Red, Green, Blue, and Alpha channels.  
It’s designed to optimize materials by reducing the number of texture samplers, improving performance in Unity projects.

---

## ✨ Features
- Drag‑and‑drop UI for assigning textures to R, G, B, and A channels.
- Supports **Roughness, Metallic, Emission, Alpha** (or any grayscale maps).
- Adjustable output resolution (250, 1024, 2048, 4096).
- **Auto Create** mode: automatically finds textures in selected folders based on suffix rules.
- Saves packed textures as `.png` with correct import settings (non‑sRGB, uncompressed).
- Works with Unity’s **Shader Graph** or custom shaders.

---

## 📦 Installation
1. Just put the **`CombineTextureWindow`** folder inside your project’s **`Editor`** folder.
2. In Unity, open the menu:  
   **Tools → Combine Texture**
3. The editor window will appear.

---

## 🖥️ Usage
1. Open the **Combine Texture** window from the Unity menu.
2. Assign textures to the **R, G, B, A** slots:
   - R → Roughness  
   - G → Metallic  
   - B → Emission  
   - A → Alpha Mask
3. Choose the output resolution from the dropdown.
4. Click **Create** to export a packed texture manually, or use **Auto Create** to process all selected folders.
5. The tool will generate a `.png` file with your chosen suffix (e.g., `_RMEA.png`).
6. The saved texture will be imported with:
   - **sRGB disabled**
   - **Uncompressed format**

---

## 📂 Auto Create Workflow
- Select one or more folders in the **Project window**.
- Click **Auto Create**.
- The tool will search for textures with matching suffixes (defined in `CompineTexture_SuffixToChanal`) and automatically pack them.
- Output textures are saved in the same folder with the folder name + suffix.

---

## ⚙️ Requirements
- Unity 2021.3+ (tested)

---

## 📝 Notes
- Make sure your grayscale maps are set to **Non‑Color** in import settings.
- Unity expects **Smoothness** instead of Roughness, so you may need to invert the R channel in your shader.
- You can customize suffix rules in `CompineTexture_SuffixToChanal.cs`.

---

## 📜 License
MIT License — free to use and modify in your projects.
