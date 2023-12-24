using Godot;
using TinyRogue.Engine;

namespace TinyRogue.Godot;

public partial class DungeonMap : Node2D
{
    [Export] public TileMap TileMap;

    public void Setup(Map.Dungeon dungeon)
    {
        for (int y = 0; y < dungeon.Walls.GetLength(0); y++)
        {
            for (int x = 0; x < dungeon.Walls.GetLength(1); x++)
            {
                if (dungeon.Walls[y, x])
                {
                    PlaceWall(x, y);
                }
            }
        }
    }

    public void PlaceWall(int x, int y)
    {
        TileMap.SetCell(0, new Vector2I(x, y), 0, new Vector2I(10, 17));
    }
}