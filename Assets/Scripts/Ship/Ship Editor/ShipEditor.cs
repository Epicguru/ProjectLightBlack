
using UnityEngine;

public class ShipEditor : MonoBehaviour
{
    public int Width, Height;
    public Ship Target;

    [Header("Main")]
    public EditorTool CurrentTool = EditorTool.PLACE;

    [Header("Selection")]
    public SpriteRenderer SelectionSprite;
    public bool Display = false;
    public Color CurrentColour;
    public float Frequency = 1f;
    public float Amplitude = 0.2f;

    [Header("Selection Colours")]
    public Color PlacingColour = Color.green;
    public Color CannotPlaceColour = Color.red;
    public Color WarnColour = Color.yellow;

    public int MouseX { get; private set; }
    public int MouseY { get; private set; }
    public bool CanPlace { get; private set; }

    public byte ID;

    private void Awake()
    {
        Target.Init(Width, Height);
    }

    private void Update()
    {
        MouseX = Mathf.RoundToInt(InputManager.MousePos.x);
        MouseY = Mathf.RoundToInt(InputManager.MousePos.y);

        CanPlace = Target.InBounds(MouseX, MouseY);        

        // Set current colour.
        if (!CanPlace)
        {
            CurrentColour = CannotPlaceColour;
        }
        else
        {
            CurrentColour = PlacingColour;
        }        

        // Update actual tools.
        DoTool(CurrentTool);

        float wave = Mathf.Sin(Time.time * Mathf.PI * 2f * Frequency) * Amplitude;
        Color c = new Color(CurrentColour.r, CurrentColour.g, CurrentColour.b, Display ? CurrentColour.a + wave : 0);
        SelectionSprite.color = c;
        SelectionSprite.transform.position = new Vector3(MouseX, MouseY, 0f);
    }

    public void DoTool(EditorTool tool)
    {
        switch (tool)
        {
            case EditorTool.PLACE:
                if (CanPlace && Input.GetMouseButtonDown(0))
                {
                    PlaceTile(Tile.GetTile(ID), MouseX, MouseY);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    PlaceTile(null, MouseX, MouseY);
                }
                break;

            case EditorTool.MAKE_SLANTED:

                var tile = Target.GetTile(MouseX, MouseY);

                if(tile == null)
                {
                    CurrentColour = CannotPlaceColour;
                    break;
                }
                if(!(tile is ConnectedTile))
                {
                    CurrentColour = WarnColour;
                    break;
                }

                bool canBeSlanted = tile.CanBeSlanted();
                if (!canBeSlanted)
                {
                    CurrentColour = WarnColour;
                    break;
                }

                if (CanPlace && Input.GetMouseButtonDown(0))
                {
                    var ct = tile as ConnectedTile;
                    bool s = !InputManager.IsPressed("Alternative Tool Use");

                    if(ct.Slanted != s)
                    {
                        ct.Slanted = s;
                        ct.RefreshTexture();
                        Target.UpdateSurroundingsRadial(ct.X, ct.Y);
                    }                    
                }
                break;
        }
    }

    public void PlaceTile(Tile prefab, int x, int y)
    {
        Target.SetTile(prefab, x, y, true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Target.transform.position + new Vector3(Width / 2f - 0.5f, Height / 2f - 0.5f), new Vector3(Width, Height, 0.1f));
    }
}
