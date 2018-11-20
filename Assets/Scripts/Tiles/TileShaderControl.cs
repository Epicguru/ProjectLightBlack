using UnityEngine;

public class TileShaderControl : MonoBehaviour
{
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
    public Sprite[] Parts = new Sprite[9];

    private Vector4[] Regions = new Vector4[18];
    private int regionsID;
    private int textureID;
    private bool dirty = false;
    private Texture cachedTex;
    const int SIZE = 32;
    const float EDGE = 5;
    const float EDGE_SIZE = EDGE / SIZE;
    private static Vector4[] positions = new Vector4[9]
    {
        new Vector4(0, 0, 1, 1),
        new Vector4(0, 0, EDGE_SIZE, 1),
        new Vector4(0, 1 - EDGE_SIZE, 1, EDGE_SIZE),
        new Vector4(1 - EDGE_SIZE, 0, EDGE_SIZE, 1),
        new Vector4(0, 0, 1, EDGE_SIZE),
        new Vector4(0, 0, EDGE_SIZE, EDGE_SIZE),
        new Vector4(0, 1 - EDGE_SIZE, EDGE_SIZE, EDGE_SIZE),
        new Vector4(1 - EDGE_SIZE, 1 - EDGE_SIZE, EDGE_SIZE, EDGE_SIZE),
        new Vector4(1 - EDGE_SIZE, 0, EDGE_SIZE, EDGE_SIZE)
    };

    private void Start()
    {
        // Find property ID's.
        regionsID = Shader.PropertyToID("_Regions");
        textureID = Shader.PropertyToID("_MainTex");

        for (byte i = 0; i < 9; i++)
        {
            UpdateRegion(i, new Vector4(-1, -1, -1, -1));
        }
    }

    private void LateUpdate()
    {
        for (byte i = 0; i < 9; i++)
        {
            if (Parts.Length == i)
                break;

            var spr = Parts[i];
            if (spr == null)
                continue;
            var rect = spr.textureRect;

            Vector4 region = new Vector4();
            region.x = rect.x / spr.texture.width;
            region.y = rect.y / spr.texture.height;
            region.z = rect.width / spr.texture.width;
            region.w = rect.height / spr.texture.height;

            if(cachedTex == null)
            {
                cachedTex = spr.texture;
            }
            else if(cachedTex != spr.texture)
            {
                Debug.LogError("Tried to use a sprite that was not packed next to previous sprites! This is not supported. Expect wierd results.");
            }

            UpdateRegion(i, region);
        }

        if (dirty)
        {
            Material.SetTexture(textureID, cachedTex);
            Material.SetVectorArray(regionsID, Regions);
            dirty = false;
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
        dirty = true;
        return true;
    }
}
