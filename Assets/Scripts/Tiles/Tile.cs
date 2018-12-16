
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;

public abstract class Tile : MonoBehaviour
{
    public static Dictionary<string, Sprite> loadedTextures = new Dictionary<string, Sprite>();

    public Ship Ship { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    /// <summary>
    /// Called when a tile adjacent (including diagonals) to this tile is removed or placed, or a visual change that requires surroundings to adapt occures.
    /// </summary>
    /// <param name="changeX">The X position of the tile that changed.</param>
    /// <param name="changeY">The Y position of the tile that changed.</param>
    /// <param name="changedTile">The newly placed/updated tile, or null if the tile was removed.</param>
    public virtual void SurroundingsUpdated(int changeX, int changeY, Tile changedTile)
    {

    }

    public abstract void RefreshTexture();    

    public Tile GetTileRelative(int rx, int ry)
    {
        return Ship.GetTile(X + rx, Y + ry);
    }

    public virtual bool ConnectsTo(Tile other)
    {
        if (other == null)
            return false;
        else
            return true;
    }

    private static StringBuilder str = new StringBuilder();
    public static Sprite GetSprite(string baseName, string part)
    {
        str.Clear();
        str.Append(baseName);
        str.Append('_');
        str.Append(part);
        string full = str.ToString();


        if (loadedTextures.ContainsKey(full))
        {
            return loadedTextures[full];
        }
        else
        {
            string resPath = "Tiles/" + baseName + '/' + baseName;
            Sprite[] spr = Resources.LoadAll<Sprite>(resPath);

            if (loadedTextures.ContainsKey(spr[0].name))
            {
                Debug.LogWarning("Attempted to re-load sprites that have already been cached, check spelling of '{0}' for base tile '{1}'".Form(part, baseName));
                return null;
            }

            Debug.Log("Loaded {0} sprites for '{1}'".Form(spr.Length, baseName));

            Sprite target = null;
            foreach (var item in spr)
            {
                if (item.name == full)
                    target = item;

                loadedTextures.Add(item.name, item);
                Debug.Log(item.name);
            }

            return target;
        }
    }
}
