
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(TileShaderControl))]
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

    public Texture Filler;

    public bool Slanted
    {
        get
        {
            return _slanted;
        }
        set
        {
            if(_slanted != value)
            {
                _slanted = value;
                //RefreshTexture();
            }
        }
    }
    [SerializeField]
    private bool _slanted;

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
            indices.Add("BL", 5);
            indices.Add("TL", 6);
            indices.Add("TR", 7);
            indices.Add("BR", 8);
        }

        ShaderControl.Init();
        ShaderControl.UpdateFiller(Filler);
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

        Tile leftT = GetTileRelative(-1, 0);
        bool left = ConnectsTo(leftT);
        Tile rightT = GetTileRelative(1, 0);
        bool right = ConnectsTo(rightT);
        Tile belowT = GetTileRelative(0, -1);
        bool below = ConnectsTo(belowT);
        Tile aboveT = GetTileRelative(0, 1);
        bool above = ConnectsTo(aboveT);
        bool topLeft = ConnectsTo(GetTileRelative(-1, 1));
        bool topRight = ConnectsTo(GetTileRelative(1, 1));
        bool bottomLeft = ConnectsTo(GetTileRelative(-1, -1));
        bool bottomRight = ConnectsTo(GetTileRelative(1, -1));


        if (Slanted && !CanBeSlanted())
            Slanted = false;

        int cornerIndex = 0;
        if (Slanted)
        {
            // Determine corner...
            if(right && above)
            {
                cornerIndex = 0;
                ShaderControl.UpdateRegion(0, GetSprite(SpriteName, "Corner_Bottom_Left"));
            }
            else if (right && below)
            {
                cornerIndex = 1;
                ShaderControl.UpdateRegion(0, GetSprite(SpriteName, "Corner_Top_Left"));
            }
            else if(left && below)
            {
                cornerIndex = 2;
                ShaderControl.UpdateRegion(0, GetSprite(SpriteName, "Corner_Top_Right"));
            }
            else if(left && above)
            {
                cornerIndex = 3;
                ShaderControl.UpdateRegion(0, GetSprite(SpriteName, "Corner_Bottom_Right"));
            }
            else
            {
                // Something went wrong, the slanted flag should not be true.
                Debug.LogWarning("ConnectedTile is flagged as Slanted, but it is not on a corner!");
                ShaderControl.UpdateRegion(0, GetSprite(SpriteName, "Full"));
            }
        }
        else
        {
            ShaderControl.UpdateRegion(0, GetSprite(SpriteName, "Full"));
        }

        if (!Slanted)
        {
            SetTexturePart("Top", above ? null : GetSprite(SpriteName, "Top"));
            SetTexturePart("Bottom", below ? null : GetSprite(SpriteName, "Bottom"));
            SetTexturePart("Left", left ? null : GetSprite(SpriteName, "Left"));
            SetTexturePart("Right", right ? null : GetSprite(SpriteName, "Right"));
        }
        else
        {
            SetTexturePart("Top", null);
            SetTexturePart("Bottom", null);
            SetTexturePart("Left", null);
            SetTexturePart("Right", null);
        }       

        if(!Slanted || (Slanted && cornerIndex != 1))
            SetTexturePart("TL", (!left && !above) ? GetSprite(SpriteName, "Top_Left") : null);
        else
            SetTexturePart("TL", null);

        if (!Slanted || (Slanted && cornerIndex != 2))
            SetTexturePart("TR", (!right && !above) ? GetSprite(SpriteName, "Top_Right") : null);
        else
            SetTexturePart("TR", null);

        if (!Slanted || (Slanted && cornerIndex != 0))
            SetTexturePart("BL", (!left && !below) ? GetSprite(SpriteName, "Bottom_Left") : null);
        else
            SetTexturePart("BL", null);

        if (!Slanted || (Slanted && cornerIndex != 3))
            SetTexturePart("BR", (!right && !below) ? GetSprite(SpriteName, "Bottom_Right") : null);
        else
            SetTexturePart("BR", null);

        if (left && above && !topLeft)
        {
            var cta = leftT as ConnectedTile;
            var ctb = aboveT as ConnectedTile;
            bool slanted = (cta == null ? false: cta.Slanted) || (ctb == null ? false : ctb.Slanted);
            SetTexturePart("TL", slanted ? GetSprite(SpriteName, "Top_Left_IC") : GetSprite(SpriteName, "Top_Left_I"));
        }
        if (right && above && !topRight)
        {
            var cta = rightT as ConnectedTile;
            var ctb = aboveT as ConnectedTile;
            bool slanted = (cta == null ? false : cta.Slanted) || (ctb == null ? false : ctb.Slanted);
            SetTexturePart("TR", slanted ? GetSprite(SpriteName, "Top_Right_IC") : GetSprite(SpriteName, "Top_Right_I"));
        }
        if (left && below && !bottomLeft)
        {
            var cta = leftT as ConnectedTile;
            var ctb = belowT as ConnectedTile;
            bool slanted = (cta == null ? false : cta.Slanted) || (ctb == null ? false : ctb.Slanted);
            SetTexturePart("BL", slanted ? GetSprite(SpriteName, "Bottom_Left_IC") : GetSprite(SpriteName, "Bottom_Left_I"));
        }
        if (right && below && !bottomRight)
        {
            var cta = rightT as ConnectedTile;
            var ctb = belowT as ConnectedTile;
            bool slanted = (cta == null ? false : cta.Slanted) || (ctb == null ? false : ctb.Slanted);
            SetTexturePart("BR", slanted ? GetSprite(SpriteName, "Bottom_Right_IC") : GetSprite(SpriteName, "Bottom_Right_I"));
        }

        ShaderControl.Apply();
    }

    private void SetTexturePart(string part, Sprite spr)
    {
        byte index = indices[part];
        ShaderControl.UpdateRegion(index, spr);
    }
}