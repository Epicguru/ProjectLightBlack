using UnityEngine;

public class TileShaderControl : MonoBehaviour
{
    public Tile Tile
    {
        get
        {
            if (_tile == null)
                _tile = GetComponent<Tile>();
            return _tile;
        }
    }
    private Tile _tile;

    public MeshRenderer MeshRenderer
    {
        get
        {
            if (_mr == null)
                _mr = GetComponent<MeshRenderer>();
            return _mr;
        }
    }
    private MeshRenderer _mr;
    public Material Material
    {
        get
        {
            return MeshRenderer.material;
        }
    }

    private Vector4[] Regions = new Vector4[18];
    private static int regionsID = Shader.PropertyToID("_Regions");
    private static int textureID = Shader.PropertyToID("_MainTex");
    private static int fillerID = Shader.PropertyToID("_FillerTex");
    private static int offsetID = Shader.PropertyToID("_Offset");
    private Texture cachedTex;
    const int SIZE = 32;
    const float EDGE = 5;
    const float EDGE_SIZE = EDGE / SIZE;
    private static Vector4[] positions = new Vector4[9]
    {
        new Vector4(0, 0, 1, 1), // Center
        new Vector4(0, 0, EDGE_SIZE, 1), // Left Edge
        new Vector4(0, 1 - EDGE_SIZE, 1, EDGE_SIZE), // Top Edge
        new Vector4(1 - EDGE_SIZE, 0, EDGE_SIZE, 1), // Right Edge
        new Vector4(0, 0, 1, EDGE_SIZE), // Bottom Edge
        new Vector4(0, 0, EDGE_SIZE, EDGE_SIZE), // Bottom Left Corner
        new Vector4(0, 1 - EDGE_SIZE, EDGE_SIZE, EDGE_SIZE), // Top Left Corner
        new Vector4(1 - EDGE_SIZE, 1 - EDGE_SIZE, EDGE_SIZE, EDGE_SIZE), // Top Right Corner
        new Vector4(1 - EDGE_SIZE, 0, EDGE_SIZE, EDGE_SIZE) // Bottom Right Corner
    };

    public void Init()
    {
        for (byte i = 0; i < 9; i++)
        {
            UpdateRegion(i, null);
        }
        Apply();
    }

    public bool UpdateRegion(byte index, Sprite sprite)
    {
        if(sprite == null)
        {
            return UpdateRegion(index, new Vector4(-1, -1, -1, -1));
        }
        else
        {
            Rect rect = sprite.textureRect;
            Texture t = sprite.texture;
            Vector4 region = new Vector4();
            region.x = rect.x / t.width;
            region.y = rect.y / t.height;
            region.z = rect.width / t.width;
            region.w = rect.height / t.height;

            if (cachedTex == null)
            {
                cachedTex = sprite.texture;
            }
            else if (cachedTex != sprite.texture)
            {
                Debug.LogError("Tried to use a sprite that was not packed next to previous sprites! This is not supported. Expect wierd results.");
            }

            return UpdateRegion(index, region);
        }
    }

    public bool UpdateRegion(byte index, Vector4 region)
    {
        if (index >= 9)
        {
            Debug.LogError("Cannot update region with index {0}. The max region index is 8.".Form(index));
            return false;
        }

        var pos = positions[index];
        return UpdateRegion(index, region, pos);
    }

    public bool UpdateRegion(byte index, Vector4 region, Vector4 position)
    {
        if(index >= 9)
        {
            Debug.LogError("Cannot update region with index {0}. The max region index is 8.".Form(index));
            return false;
        }

        if (Regions[index * 2] == region && Regions[index * 2 + 1] == position)
            return false;

        Regions[index * 2] = region;
        Regions[index * 2 + 1] = position;
        return true;
    }

    public void UpdateFiller(Texture filler)
    {
        Material.SetTexture(fillerID, filler);
    }

    public void Apply()
    {
        Material.SetTexture(textureID, cachedTex);
        Material.SetVectorArray(regionsID, Regions);
        Material.SetVector(offsetID, new Vector2(Tile.X, Tile.Y));
    }
}
