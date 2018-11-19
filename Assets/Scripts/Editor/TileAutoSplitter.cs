using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class TileAutoSplitter : MonoBehaviour
{
    [MenuItem("Assets/Tools/Slice Spritesheets")]
    public static void RunOnSelected()
    {
        var textures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

        foreach (var texture in textures)
        {
            ProcessTexture(texture);
        }
    }

    [MenuItem("Assets/Tools/Slice Spritesheets", validate = true)]
    private static bool CanExecute()
    {
        return Selection.GetFiltered<Texture2D>(SelectionMode.Assets).Length > 0;
    }

    private static void ProcessTexture(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        importer.spriteImportMode = SpriteImportMode.Multiple;

        // Why do I do this? Well it turns out that Unity will not actually save the changes made to the importer if we only change the spritesheet.
        string r;
        do
        {
            r = Random.Range(0, int.MaxValue).ToString();
            if(importer.userData != r)
            {
                importer.userData = r;
                break;
            }
        }
        while (true);

        if(importer.spritesheet.Length != 0)
        {
            bool proceed = EditorUtility.DisplayDialog("Existing sprites", "There are already some existing sprites defined in this texture. Do you want to overwrite them with the auto generated ones?", "Yes, overwrite", "No, cancel");
            if (!proceed)
                return;
        }

        var metas = new List<SpriteMetaData>();
        string baseName = Path.GetFileNameWithoutExtension(path) + '_';

        const int FULL = 32;
        const int EDGE = 5;
        const int INNER = FULL - EDGE * 2;

        // Left
        metas.Add(CreateRegion(0, 0, EDGE, EDGE, baseName + "Bottom_Left"));
        metas.Add(CreateRegion(0, EDGE, EDGE, INNER, baseName + "Left"));
        metas.Add(CreateRegion(0, EDGE + INNER, EDGE, EDGE, baseName + "Top_Left"));

        // Right
        metas.Add(CreateRegion(EDGE + INNER, 0, EDGE, EDGE, baseName + "Bottom_Right"));
        metas.Add(CreateRegion(EDGE + INNER, EDGE, EDGE, INNER, baseName + "Right"));
        metas.Add(CreateRegion(EDGE + INNER, EDGE + INNER, EDGE, EDGE, baseName + "Top_Right"));

        // Bottom
        metas.Add(CreateRegion(EDGE, 0, INNER, EDGE, baseName + "Bottom"));

        // Top
        metas.Add(CreateRegion(EDGE, EDGE + INNER, INNER, EDGE, baseName + "Top"));

        // Full
        metas.Add(CreateRegion(FULL, 0, FULL, FULL, "Full"));

        importer.spritesheet = metas.ToArray();
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

    private static SpriteMetaData CreateDefaultMetaData()
    {
        return new SpriteMetaData() { border = Vector4.zero, alignment = (int)(SpriteAlignment.Center) };
    }

    private static SpriteMetaData CreateRegion(int x, int y, int width, int height, string name)
    {
        var meta = CreateDefaultMetaData();
        meta.name = name;
        meta.rect = new Rect(x, y, width, height);
        Debug.Log("Generated new sprite '" + name + "'");
        return meta;
    }
}
