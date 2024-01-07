module TinyRogue.Engine.Map

open TinyRogue.Engine.Types
open TinyRogue.Engine.Types.Primitives

let inverseRect (grid: BitGrid) width height x y =
    for rx in x .. (x + width - 1) do
        grid[y, rx] <- false
        grid[y + height - 1, rx] <- false

    for ry in y .. (y + height - 1) do
        grid[ry, x] <- false
        grid[ry, x + width - 1] <- false

let rect (grid: BitGrid) width height x y =
    for rx in x .. (x + width - 1) do
        for ry in y .. (y + height - 1) do
            grid[ry, rx] <- false

let inBounds (grid: BitGrid) (pos: Position) =
    not
    <| (pos.x < 0 || pos.y < 0 || pos.x > grid.GetLength(1) || pos.y > grid.GetLength(0))

let raycast (grid: BitGrid) (pos: Position) (step: Position) =
    let mutable cpos = pos
    let mutable oob = false

    while (not oob) && grid[cpos.y, cpos.x] = false do
        let next = cpos.offset(step.x, step.y)

        if not (inBounds grid next) then
            oob <- true
        else
            cpos <- next

    cpos

let canMoveTo (dungeon: Dungeon) (pos: Position) = not (dungeon.Walls[pos.y, pos.x])
