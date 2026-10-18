using UnityEditor;

namespace BinaryLift.EditorTools
{
    /// <summary>Bound the approved art footprint; furniture materials are supplied at runtime.</summary>
    public sealed class ApprovedAssetImport : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Resources/Approved/")) return;
            var importer = (TextureImporter)assetImporter;
            importer.maxTextureSize = 1024;
            importer.mipmapEnabled = true;
            importer.isReadable = false;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.wrapMode = assetPath.EndsWith("Sandstone.jpg") ? UnityEngine.TextureWrapMode.Repeat : UnityEngine.TextureWrapMode.Clamp;
        }
        void OnPreprocessModel()
        {
            if (!assetPath.StartsWith("Assets/Resources/Approved/")) return;
            var importer = (ModelImporter)assetImporter;
            importer.importAnimation = false;
            importer.materialImportMode = ModelImporterMaterialImportMode.None;
            importer.addCollider = false;
            importer.isReadable = false;
        }
    }
}
