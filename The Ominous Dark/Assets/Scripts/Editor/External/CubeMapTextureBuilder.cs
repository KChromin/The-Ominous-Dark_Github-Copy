using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NOS.Editor.Tools.External
{
    public class CubeMapTextureBuilder : EditorWindow
    {
        [MenuItem("No One's Sorrow/External/CubeMap Builder")]
        public static void OpenWindow()
        {
            GetWindow<CubeMapTextureBuilder>();
        }

        private readonly Texture2D[] _textures = new Texture2D[6];
        [SerializeField]
        private string[] labels =
        {
            "Right", "Left",
            "Top", "Bottom",
            "Front", "Back"
        };

        private readonly TextureFormat[] _hdrFormats =
        {
            TextureFormat.ASTC_HDR_10x10,
            TextureFormat.ASTC_HDR_12x12,
            TextureFormat.ASTC_HDR_4x4,
            TextureFormat.ASTC_HDR_5x5,
            TextureFormat.ASTC_HDR_6x6,
            TextureFormat.ASTC_HDR_8x8,
            TextureFormat.BC6H,
            TextureFormat.RGBAFloat,
            TextureFormat.RGBAHalf
        };

        private readonly Vector2Int[] _placementRects =
        {
            new (2, 1),
            new (0, 1),
            new (1, 2),
            new (1, 0),
            new (1, 1),
            new (3, 1)
        };


        // ReSharper disable once CognitiveComplexity
        private void OnGUI()
        {
            for (int i = 0; i < 6; i++)
            {
                _textures[i] = EditorGUILayout.ObjectField(labels[i], _textures[i], typeof(Texture2D), false) as Texture2D;
            }

            if (GUILayout.Button("Build CubeMap"))
            {
                // Missing Texture
                if (_textures.Any(t => !t))
                {
                    EditorUtility.DisplayDialog("CubeMap Builder Error", "One or more texture is missing.", "Ok");
                    return;
                }

                // Get size
                int size = _textures[0].width;

                // Not all the same size or square
                if (_textures.Any(t => t.width != size || t.height != size))
                {
                    EditorUtility.DisplayDialog("CubeMap Builder Error", "All the textures need to be the same size and square.", "Ok");
                    return;
                }

                bool isHDR = _hdrFormats.Any(f => f == _textures[0].format);
                string[] texturePaths = _textures.Select(AssetDatabase.GetAssetPath).ToArray();

                // Should be ok, ask for the file path.
                string path = EditorUtility.SaveFilePanel("Save CubeMap", Path.GetDirectoryName(texturePaths[0]), "Cubemap", isHDR ? "exr" : "png");

                if (string.IsNullOrEmpty(path)) return;

                // Save the readable flag to restore it afterward
                bool[] readableFlags = _textures.Select(t => t.isReadable).ToArray();

                // Get the importer and mark the textures as readable
                TextureImporter[] importers = texturePaths.Select(p => AssetImporter.GetAtPath(p) as TextureImporter).ToArray();

                foreach (TextureImporter importer in importers)
                {
                    importer.isReadable = true;
                }

                AssetDatabase.Refresh();

                foreach (string p in texturePaths)
                {
                    AssetDatabase.ImportAsset(p);
                }

                // Build the cubemap texture
                Texture2D cubeTexture = new (size * 4, size * 3, isHDR ? TextureFormat.RGBAFloat : TextureFormat.RGBA32, false);

                for (int i = 0; i < 6; i++)
                {
                    cubeTexture.SetPixels(_placementRects[i].x * size, _placementRects[i].y * size, size, size, _textures[i].GetPixels(0));
                }

                cubeTexture.Apply(false);

                // Save the texture to the specified path, and destroy the temporary object
                byte[] bytes = isHDR ? cubeTexture.EncodeToEXR() : cubeTexture.EncodeToPNG();

                File.WriteAllBytes(path, bytes);

                DestroyImmediate(cubeTexture);

                // Reset the read flags, and reimport everything
                for (int i = 0; i < 6; i++)
                {
                    importers[i].isReadable = readableFlags[i];
                }

                path = path.Remove(0, Application.dataPath.Length - 6);

                AssetDatabase.ImportAsset(path);

                TextureImporter cubeImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (cubeImporter)
                {
                    cubeImporter.textureShape = TextureImporterShape.TextureCube;
                    cubeImporter.sRGBTexture = false;
                    cubeImporter.generateCubemap = TextureImporterGenerateCubemap.FullCubemap;
                }

                foreach (string p in texturePaths)
                {
                    AssetDatabase.ImportAsset(p);
                }

                AssetDatabase.ImportAsset(path);

                AssetDatabase.Refresh();
            }
        }
    }
}