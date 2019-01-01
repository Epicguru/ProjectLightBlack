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

        // Left
        metas.Add(CreateRegion(0, 0, EDGE, EDGE, baseName + "Bottom_Left"));
        metas.Add(CreateRegion(0, EDGE, EDGE, FULL, baseName + "Left"));
        metas.Add(CreateRegion(0, EDGE + FULL, EDGE, EDGE, baseName + "Top_Left"));

        // Right
        metas.Add(CreateRegion(EDGE + FULL, 0, EDGE, EDGE, baseName + "Bottom_Right"));
        metas.Add(CreateRegion(EDGE + FULL, EDGE, EDGE, FULL, baseName + "Right"));
        metas.Add(CreateRegion(EDGE + FULL, EDGE + FULL, EDGE, EDGE, baseName + "Top_Right"));

        // Bottom
        metas.Add(CreateRegion(EDGE, 0, FULL, EDGE, baseName + "Bottom"));

        // Top
        metas.Add(CreateRegion(EDGE, EDGE + FULL, FULL, EDGE, baseName + "Top"));

        // Full
        metas.Add(CreateRegion(EDGE, EDGE, FULL, FULL, baseName + "Full"));

        // Inverted corners
        metas.Add(CreateRegion(2 * EDGE + FULL, EDGE + FULL, EDGE, EDGE, baseName + "Bottom_Right_I"));
        metas.Add(CreateRegion(3 * EDGE + FULL, EDGE + FULL, EDGE, EDGE, baseName + "Bottom_Left_I"));
        metas.Add(CreateRegion(2 * EDGE + FULL, FULL, EDGE, EDGE, baseName + "Top_Right_I"));
        metas.Add(CreateRegion(3 * EDGE + FULL, FULL, EDGE, EDGE, baseName + "Top_Left_I"));

        // Full tile slanted 'corners'
        metas.Add(CreateRegion(2 * EDGE + FULL, 0, FULL, FULL, baseName + "Corner_Top_Left"));
        metas.Add(CreateRegion(2 * EDGE + FULL * 2, 0, FULL, FULL, baseName + "Corner_Top_Right"));
        metas.Add(CreateRegion(2 * EDGE + FULL * 3, 0, FULL, FULL, baseName + "Corner_Bottom_Left"));
        metas.Add(CreateRegion(2 * EDGE + FULL * 4, 0, FULL, FULL, baseName + "Corner_Bottom_Right"));

        // Inverted corners for use with slanted edges.
        metas.Add(CreateRegion(4 * EDGE + FULL, EDGE + FULL, EDGE, EDGE, baseName + "Top_Left_IC"));
        metas.Add(CreateRegion(5 * EDGE + FULL, EDGE + FULL, EDGE, EDGE, baseName + "Top_Right_IC"));
        metas.Add(CreateRegion(4 * EDGE + FULL, FULL, EDGE, EDGE, baseName + "Bottom_Left_IC"));
        metas.Add(CreateRegion(5 * EDGE + FULL, FULL, EDGE, EDGE, baseName + "Bottom_Right_IC"));


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
