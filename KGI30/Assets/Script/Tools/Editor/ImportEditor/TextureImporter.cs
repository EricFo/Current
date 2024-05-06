using UnityEditor;

public class TexturePostProcessor : AssetPostprocessor {
    void OnPreprocessTexture() {
        TextureImporter importer = assetImporter as TextureImporter;
        importer.maxTextureSize = 4096;

        //importer.isReadable = false;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        //importer.textureCompression = TextureImporterCompression.Compressed;
        importer.spritePackingTag = "";
    }
}


