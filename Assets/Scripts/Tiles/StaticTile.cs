
using UnityEngine;

public class StaticTile : Tile
{
    // TODO make this for the Thruster.

    public TileOrientation Orientation; 

    public override void RefreshTexture()
    {
        // Does nothing by default.
        transform.localEulerAngles = new Vector3(0f, 0f, (int)Orientation * 90f);
    }
}
