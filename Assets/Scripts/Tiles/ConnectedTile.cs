
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ConnectedTile : Tile
{
    private static Dictionary<string, byte> indices = new Dictionary<string, byte>();

    public TileShaderControl ShaderControl
    {
        get
        {
            if (_tsc == null)
                _tsc = GetComponent<TileShaderControl>();
            return _tsc;
        }
    }
    private TileShaderControl _tsc;

    public string SpriteName;

    private void Awake()
    {
        if (indices.Count == 0)
        {
            indices.Add("Center", 0);
            indices.Add("Left", 1);
            indices.Add("Top", 2);
            indices.Add("Right", 3);
            indices.Add("Bottom", 4);
            indices.Add("BL Corner", 5);
            indices.Add("TL Corner", 6);
            indices.Add("TR Corner", 7);
            indices.Add("BR Corner", 8);
        }

        ShaderControl.Init();
        ShaderControl.UpdateRegion(0, GetSprite(SpriteName, "Full"));
        ShaderControl.Apply();
    }

    public override void SurroundingsUpdated(int changeX, int changeY, Tile changedTile)
    {
        RefreshTexture();
    }

    public override void RefreshTexture()
    {
        // Updates shader properties to match surroundings.

        bool left = ConnectsTo(GetTileRelative(-1, 0));
        bool right = ConnectsTo(GetTileRelative(1, 0));
        bool below = ConnectsTo(GetTileRelative(0, -1));
        bool above = ConnectsTo(GetTileRelative(0, 1));
        bool topLeft = ConnectsTo(GetTileRelative(-1, 1));
        bool topRight = ConnectsTo(GetTileRelative(1, 1));
        bool bottomLeft = ConnectsTo(GetTileRelative(-1, -1));
        bool bottomRight = ConnectsTo(GetTileRelative(1, -1));

        SetTexturePart("Top", above ? null : GetSprite(SpriteName, "Top"));
        SetTexturePart("Bottom", below ? null : GetSprite(SpriteName, "Bottom"));
        SetTexturePart("Left", left ? null : GetSprite(SpriteName, "Left"));
        SetTexturePart("Right", right ? null : GetSprite(SpriteName, "Right"));

        SetTexturePart("TL Corner", (!left && !above) ? GetSprite(SpriteName, "Top_Left") : null);
        SetTexturePart("TR Corner", (!right && !above) ? GetSprite(SpriteName, "Top_Right") : null);
        SetTexturePart("BL Corner", (!left && !below) ? GetSprite(SpriteName, "Bottom_Left") : null);
        SetTexturePart("BR Corner", (!right && !below) ? GetSprite(SpriteName, "Bottom_Right") : null);

        if (left && above && !topLeft)
            SetTexturePart("TL Corner", GetSprite(SpriteName, "Top_Left_I"));
        if (right && above && !topRight)
            SetTexturePart("TR Corner", GetSprite(SpriteName, "Top_Right_I"));
        if (left && below && !bottomLeft)
            SetTexturePart("BL Corner", GetSprite(SpriteName, "Bottom_Left_I"));
        if (right && below && !bottomRight)
            SetTexturePart("BR Corner", GetSprite(SpriteName, "Bottom_Right_I"));

        ShaderControl.Apply();
    }

    private void SetTexturePart(string part, Sprite spr)
    {
        byte index = indices[part];
        ShaderControl.UpdateRegion(index, spr);
    }
}