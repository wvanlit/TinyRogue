using System;
using Godot;
using Godot.Collections;
using TinyRogue.Engine;
using Random = System.Random;

namespace TinyRogue.Godot;

public partial class DungeonMap : Node2D
{
    [Export] public TileMap TileMap;

    [Export(PropertyHint.ArrayType, "Vector2i")]
    public Array<Vector2I> WallTiles;

    [Export(PropertyHint.ArrayType, "Vector2i")]
    public Array<Vector2I> DecorationTiles;

    [Export(PropertyHint.Range, "0,100")] public int DecorationPercentage = 5;

    public void Setup(Types.Dungeon dungeon)
    {
        TileMap.Clear();

        for (int y = 0; y < dungeon.Walls.GetLength(0); y++)
        {
            for (int x = 0; x < dungeon.Walls.GetLength(1); x++)
            {
                if (dungeon.Walls[y, x])
                {
                    PlaceWall(x, y);
                }
                else
                {
                    if (Random.Shared.Next(100) <= DecorationPercentage)
                    {
                        PlaceDecoration(x, y);
                    }
                }
            }
        }

        foreach (var (door, open) in dungeon.Doors)
        {
            PlaceDoor(door.x, door.y, open);
        }
    }

    public void PlaceWall(int x, int y)
    {
        TileMap.SetCell(0, new Vector2I(x, y), 0, WallTiles[Random.Shared.Next(0, WallTiles.Count)]);
    }

    public void PlaceDecoration(int x, int y)
    {
        TileMap.SetCell(1, new Vector2I(x, y), 0, DecorationTiles[Random.Shared.Next(0, DecorationTiles.Count)]);
    }

    public void PlaceDoor(int x, int y, bool open)
    {
        TileMap.SetCell(2, new Vector2I(x, y), 0, open ? new Vector2I(2, 9) : new Vector2I(0, 9));
    }
}
