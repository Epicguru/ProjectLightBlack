
using UnityEngine;

public class ShipEditor : MonoBehaviour
{
    public RawShip Raw = new RawShip();
    public Ship Target;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceTile(Tile.GetTile(0), Mathf.RoundToInt(InputManager.MousePos.x), Mathf.RoundToInt(InputManager.MousePos.y));
        }
    }

    public void PlaceTile(Tile prefab, int x, int y)
    {
        Raw.PlaceTile(prefab, x, y);
    }
}
