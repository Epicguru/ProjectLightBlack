
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;

public abstract class Tile : MonoBehaviour
{
    private static Dictionary<byte, Tile> loadedTiles = new Dictionary<byte, Tile>();
    public static Dictionary<string, Sprite> loadedTextures = new Dictionary<string, Sprite>();

    [Header("Info")]
    public byte ID;
    public string Name = "Tile Name";

    public Ship Ship { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    /// <summary>
    /// Called when a tile adjacent (including diagonals) to this tile is removed or placed, or a visual change that requires surroundings to adapt occures.
    /// By default calls RefreshTexture()
    /// </summary>
    /// <param name="changeX">The X position of the tile that changed.</param>
    /// <param name="changeY">The Y position of the tile that changed.</param>
    /// <param name="changedTile">The newly placed/updated tile, or null if the tile was removed.</param>
    public virtual void SurroundingsUpdated(int changeX, int changeY, Tile changedTile)
    {
        RefreshTexture();
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
            string resPath = "Tile Sprites/" + baseName + '/' + baseName;
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
            }

            return target;
        }
    }

    public static void LoadAllTiles()
    {
        loadedTiles.Clear();
        var fromDisk = Resources.LoadAll<Tile>("Tiles");

        foreach (var item in fromDisk)
        {
            byte id = item.ID;
            if (loadedTiles.ContainsKey(id))
            {
                Debug.LogWarning("A tile has a duplicate ID: {0}. It will not be loaded.".Form(id));
            }
            else
            {
                loadedTiles.Add(id, item);
                Debug.Log("Loaded '{0}' [{1}]".Form(item.Name, id));
            }
        }
    }

    public static Tile GetTile(byte id)
    {
        if (loadedTiles.ContainsKey(id))
        {
            return loadedTiles[id];
        }
        else
        {
            Debug.LogWarning("The tile for ID {0} is not loaded. ({1} tiles loaded)".Form(id, loadedTiles.Count));
            return null;
        }
    }

    public static void UnloadAllTiles()
    {
        loadedTiles.Clear();
    }

    public bool CanBeSlanted()
    {
        // To be slanted, a tile must have exatcly two neighbours.
        // Also they must actually form a corner position (so they cant form a straight line).

        // I'm pretty happy with how the logic is implemented here. Clean and effective. I'm patting myself on the back as I type this.

        bool left = ConnectsTo(GetTileRelative(-1, 0));
        bool right = ConnectsTo(GetTileRelative(1, 0));

        if (!(left ^ right))
            return false;

        bool above = ConnectsTo(GetTileRelative(0, 1));
        bool below = ConnectsTo(GetTileRelative(0, -1));

        if (!(above ^ below))
            return false;

        return true;
    }
}
