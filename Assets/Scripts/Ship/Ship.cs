using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour
{
    public Rigidbody2D Body
    {
        get
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody2D>();
            return _rb;
        }
    }
    private Rigidbody2D _rb;

    public int Width;
    public int Height;

    private Tile[,] tiles;

    public void Init(int width, int height)
    {
        Width = width;
        Height = height;

        tiles = new Tile[width, height];

        Debug.Log("Initialized ship that is {0}x{1} tiles.".Form(width, height));
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public void SetTile(Tile prefab, int x, int y, bool updateSurroundings = true)
    {
        if (!InBounds(x, y))
            return;

        if(prefab != null)
        {
            if(tiles[x, y] != null)
            {
                var old = tiles[x, y];

                Destroy(old.gameObject);

                tiles[x, y] = null;
            }


            Tile newTile = Instantiate(prefab);
            newTile.X = x;
            newTile.Y = y;
            newTile.Ship = this;
            newTile.transform.SetParent(this.transform);
            newTile.transform.localPosition = new Vector3(x, y, 0f);

            tiles[x, y] = newTile;

            if (updateSurroundings)
            {
                UpdateSurroundingsRadial(x, y);
            }

            SendSurroundingsUpdated(newTile, x, y, x, y);
        }
        else
        {
            if(tiles[x, y] != null)
                Destroy(tiles[x, y].gameObject);
            tiles[x, y] = null;

            if (updateSurroundings)
            {
                UpdateSurroundingsRadial(x, y);
            }
        }        
    }

    /// <summary>
    /// Sends the SurroundingsUpdated message to all tiles around the central changed tile.
    /// The central tile does not recieve the message.
    /// </summary>
    public void UpdateSurroundingsRadial(int x, int y)
    {
        var newTile = GetTile(x, y);
        SendSurroundingsUpdated(newTile, x, y, x - 1, y);
        SendSurroundingsUpdated(newTile, x, y, x - 1, y + 1);
        SendSurroundingsUpdated(newTile, x, y, x, y + 1);
        SendSurroundingsUpdated(newTile, x, y, x + 1, y + 1);
        SendSurroundingsUpdated(newTile, x, y, x + 1, y);
        SendSurroundingsUpdated(newTile, x, y, x + 1, y - 1);
        SendSurroundingsUpdated(newTile, x, y, x, y - 1);
        SendSurroundingsUpdated(newTile, x, y, x - 1, y - 1);
        
    }

    public Tile GetTile(int x, int y)
    {
        if(InBounds(x, y))
        {
            return tiles[x, y];
        }
        else
        {
            return null;
        }
    }

    public void SendSurroundingsUpdated(Tile changed, int changeX, int changeY, int x, int y)
    {
        Tile t = GetTile(x, y);
        if(t != null)
            t.SurroundingsUpdated(changeX, changeY, changed);
    }

    public void RefreshTextureAt(int x, int y)
    {
        Tile t = GetTile(x, y);
        if (t != null)
            t.RefreshTexture();
    }

    public void RefreshAllTextures()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                var t = tiles[x, y];
                if (t != null)
                    t.RefreshTexture();
            }
        }
    }
}
