module TinyRogue.Engine.Map

type BitGrid = bool array2d

type Dungeon = { Walls: BitGrid }

let rect (grid: BitGrid) width height x y =
    for rx in x .. (x + width - 1) do
        grid[y, rx] <- true
        grid[y + height - 1, rx] <- true

    for ry in y .. (y + height - 1) do
        grid[ry, x] <- true
        grid[ry, x + width - 1] <- true

let createSimpleDungeon width height =
    let walls = Array2D.create height width false
    let smallestDimension = (min width height) - 1

    // Create border
    rect walls 6 6 0 0
    rect walls 6 6 5 0
    rect walls 6 6 5 5
    
    walls[3,5] <- false
    walls[5,8] <- false

    { Walls = walls }

let canMoveTo (dungeon: Dungeon) (x: int) (y: int) =
    if dungeon.Walls[y, x] then false else true
