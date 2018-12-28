
using System.Collections.Generic;
using UnityEngine;

public class RawShip
{
    public Ship ShipPrefab;

    public List<TilePosPair> data = new List<TilePosPair>();

    public Vector2 GetMeanPoint()
    {
        Vector2Int sum = new Vector2Int();
        foreach (var item in data)
        {
            sum += item.Position;
        }
        float x = sum.x / (float)data.Count;
        float y = sum.y / (float)data.Count;

        return new Vector2(x, y);
    }

    public Ship CreateShip()
    {
        Ship newShip = GameObject.Instantiate(ShipPrefab);
        newShip.transform.position = Vector3.zero;
        newShip.transform.rotation = Quaternion.identity;

        int minX = int.MaxValue;
        int minY = int.MinValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (var item in data)
        {
            var pos = item.Position;

            if (pos.x < minX)
                minX = pos.x;
            if (pos.y < minY)
                minY = pos.y;

            if (pos.x > maxX)
                maxX = pos.x;
            if (pos.y > maxY)
                maxY = pos.y;
        }

        int width = maxX - minX;
        int height = maxY - minY;
        int offX = -minX;
        int offY = -minY;

        newShip.Init(width, height);

        foreach (var item in data)
        {
            var pos = item.Position;
            var prefab = Tile.GetTile(item.Tile.ID);

            newShip.SetTile(prefab, pos.x + offX, pos.y + offY, item.Tile.Data, false);
        }

        return newShip;
    }

    public void PlaceTile(Tile prefab, int x, int y)
    {
        Tile t = GameObject.Instantiate(prefab);
        t.transform.position = new Vector3(x, y);
        data.Add(new TilePosPair() { Position = new Vector2Int(x, y), Tile = t });
    }
}
