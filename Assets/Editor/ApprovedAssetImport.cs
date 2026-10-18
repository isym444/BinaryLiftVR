using UnityEditor;

namespace BinaryLift.EditorTools
{
    /// <summary>Bound the approved art footprint; furniture materials are supplied at runtime.</summary>
    public sealed class ApprovedAssetImport : AssetPostprocessor
    {
        public override uint GetVersion() => 2;
        void OnPreprocessTexture()
        {
            if (!assetPath.StartsWith("Assets/Resources/Approved/")) return;
            var importer = (TextureImporter)assetImporter;
            importer.maxTextureSize = 1024;
            importer.mipmapEnabled = true;
            importer.isReadable = false;
            importer.textureCompression = TextureImporterCompression.Compressed;
            importer.wrapMode = assetPath.EndsWith("Sandstone.jpg") || assetPath.Contains("_diff.") || assetPath.Contains("_nor_gl.") || assetPath.Contains("_rough.") ? UnityEngine.TextureWrapMode.Repeat : UnityEngine.TextureWrapMode.Clamp;
            if(assetPath.Contains("_nor_gl."))importer.textureType=TextureImporterType.NormalMap;
            if(assetPath.Contains("_rough."))importer.sRGBTexture=false;
        }
        void OnPreprocessModel()
        {
            if (!assetPath.StartsWith("Assets/Resources/Approved/")) return;
            var importer = (ModelImporter)assetImporter;
            importer.importAnimation = false;
            importer.materialImportMode = ModelImporterMaterialImportMode.None;
            if(assetPath.Contains("/nature/")||assetPath.Contains("/furniture/"))
            {importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;importer.materialLocation=ModelImporterMaterialLocation.InPrefab;}
            importer.addCollider = false;
            importer.isReadable = false;
        }
    }
}
