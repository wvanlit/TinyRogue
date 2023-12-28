using Godot;
using TinyRogue.Engine;

namespace TinyRogue.Godot;

public partial class VisionMap : TileMap
{
    private const int OcclusionLayer = 0;
    private readonly Vector2I _atlasTile = new(8, 5);

    private const int OccludedAlt = 0;
    private const int SeenAlt = 1;
    private const int VisibleAlt = 2;

    public override void _Ready()
    {
        for (int x = -100; x < 100; x++)
        {
            for (int y = -100; y < 100; y++)
            {
                SetCell(0, new Vector2I(x, y), OcclusionLayer, _atlasTile, OccludedAlt);
            }
        }
    }

    public void Setup(Types.Visibility[,] shadowMap)
    {
        for (var y = 0; y < shadowMap.GetLength(0); y++)
        {
            for (int x = 0; x < shadowMap.GetLength(1); x++)
            {
                var v = shadowMap[y, x];
                SetVisibility(new Vector2I(x, y), v.IsVisible, v.IsExplored);
            }
        }
    }

    public void SetVisibility(Vector2I cell, bool visible, bool seen)
    {
        var tile = visible ? VisibleAlt : seen ? SeenAlt : OccludedAlt;
        SetCell(0, cell, OcclusionLayer, _atlasTile, tile);
    }

    public (bool visible, bool seen) CheckVisibility(Vector2I cell)
    {
        var id = GetCellAlternativeTile(OcclusionLayer, cell);

        return (id == VisibleAlt, id == SeenAlt);
    }
}