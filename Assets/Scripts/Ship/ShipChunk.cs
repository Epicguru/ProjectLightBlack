
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipChunk : MonoBehaviour
{
    // A ship chink is a region that makes up a ship, which has it's own physics body.
    // They can be dynamically created as required.

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

    public int Width, Height;

    private Tile[,] tiles;

    public void CreateArray(int width, int height)
    {
        this.Width = width;
        this.Height = height;

        tiles = new Tile[width, height];
    }
}
