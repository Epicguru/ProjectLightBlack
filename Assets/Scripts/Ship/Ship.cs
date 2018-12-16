using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ship : MonoBehaviour
{
    public Rigidbody2D Rigidbody
    {
        get
        {
            if (_rb == null)
                _rb = GetComponent<Rigidbody2D>();
            return _rb;
        }
    }
    private Rigidbody2D _rb;

    public Tile Prefab;
    public int Width = 5, Height = 5;
    private Tile[,] Tiles;

    private void Awake()
    {
        Tiles = new Tile[Width, Height];
        Tiles[0, 0] = CreateTile(0, 0);
        Tiles[1, 0] = CreateTile(1, 0);
        Tiles[1, 1] = CreateTile(1, 1);
        Tiles[2, 1] = CreateTile(2, 1);
        Tiles[2, 2] = CreateTile(2, 2);
        Tiles[1, 2] = CreateTile(1, 2);

        UpdateAllTextures();
    }

    private void UpdateAllTextures()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                UpdateTexture(x, y);
            }
        }
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public Tile GetTile(int x, int y)
    {
        if(InBounds(x, y))
        {
            return Tiles[x, y];
        }
        else
        {
            return null;
        }
    }

    private void UpdateTexture(int x, int y)
    {
        if (!InBounds(x, y))
            return;

        var tile = Tiles[x, y];
        if (tile == null)
            return;

        tile.RefreshTexture();
    }

    private Tile CreateTile(int x, int y)
    {
        if (!InBounds(x, y))
            return null;

        Tile tile = Instantiate(Prefab, new Vector3(x, y, 0), Quaternion.identity, this.transform);
        tile.Ship = this;
        tile.X = x;
        tile.Y = y;
        return tile;
    }
}
