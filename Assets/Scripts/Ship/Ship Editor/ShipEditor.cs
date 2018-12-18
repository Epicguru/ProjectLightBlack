
using UnityEngine;

public class ShipEditor : MonoBehaviour
{
    public Ship Target;

    private void Update()
    {
        var worldPos = InputManager.MousePos;
        int wpx = Mathf.RoundToInt(worldPos.x);
        int wpy = Mathf.RoundToInt(worldPos.y);
        int sx = (int)Target.transform.position.x;
        int sy = (int)Target.transform.position.y;

        int rx = wpx + sx;
        int ry = wpy + sy;

        if(Input.GetKeyDown(KeyCode.Space))
            PlaceTile(Tile.GetTile(0), rx, ry);
    }

    public void PlaceTile(Tile prefab, int x, int y)
    {
        int left = 0;
        int above = 0;
        if (!Target.InBounds(x, y) && prefab != null)
        {
            int right = 0;
            int below = 0;

            if(x < 0)
            {
                left = -x;
            }
            else if(x >= Target.Width)
            {
                right = x - (Target.Width - 1);
            }

            if (y < 0)
            {
                below = -y;
            }
            else if(y >= Target.Height)
            {
                above = y - (Target.Height - 1);
            }

            Debug.Log("({4}, {5}) Resizing {0}l, {1}r, {2}a, {3}b".Form(left, right, above, below, x, y));
            Target.Resize(right, left, above, below);
        }

        Target.SetTile(prefab, x - left, y - above);
    }
}
