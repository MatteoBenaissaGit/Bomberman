using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 Position;
    public bool IsWall;

    public void Init(Vector2 position)
    {
        Position = position;
    }
}
