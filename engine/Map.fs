module TinyRogue.Engine.Map

open TinyRogue.Engine.Types

let rect (grid: BitGrid) width height x y =
    for rx in x .. (x + width - 1) do
        grid[y, rx] <- true
        grid[y + height - 1, rx] <- true

    for ry in y .. (y + height - 1) do
        grid[ry, x] <- true
        grid[ry, x + width - 1] <- true

let canMoveTo (dungeon: Dungeon) (x: int) (y: int) =
    true // Ghost mode
    //if dungeon.Walls[y, x] then false else true
