module TinyRogue.Engine.ProcGen.DungeonGeneration

open TinyRogue.Engine.Map
open TinyRogue.Engine.Types
open TinyRogue.Engine.ProcGen.Random
open TinyRogue.Engine.ProcGen.BinarySpacePartition

let carveCorridor (walls: BitGrid) (from: Position) (towards: Position) =
    let fx, tx = (min from.x towards.x, max from.x towards.x)
    let fy, ty = (min from.y towards.y, max from.y towards.y)

    for cx in fx..tx do
        walls[ty, cx] <- false

    for cy in fy..ty do
        walls[cy, tx] <- false

let canPlaceDoorX (walls: BitGrid) (pos: Position) =
    let check rx ry = walls[pos.y + ry, pos.x + rx] = false
    check 0 0 && check 1 0 && check -1 0

let canPlaceDoorY (walls: BitGrid) (pos: Position) =
    let check rx ry = walls[pos.y + ry, pos.x + rx] = false

    check 0 0 && check 0 1 && check 0 -1

let createSimpleDungeon (width: uint) (height: uint) (border: uint) : Dungeon =
    let heightWithBorder = (height + border * 2u)
    let widthWithBorder = (width + border * 2u)

    let walls = Array2D.create (int heightWithBorder) (int widthWithBorder) true // Fill with walls

    let rooms =
        BinarySpacePartition(5u, width, height).Generate()
        |> List.map (fun r -> r.Offset border border)

    let mutable prev = Option.None

    for room in rooms do
        rect walls (int room.Width) (int room.Height) (int room.X) (int room.Y)

        let centerPos = room.RandomPointInRoom()

        if prev.IsSome then
            carveCorridor walls prev.Value centerPos

        prev <- Some centerPos

    let mutable doors = []

    for room in rooms do
        for dx in room.X .. room.X + room.Width do
            let tryPlace pos =
                if canPlaceDoorY walls pos then
                    doors <- pos :: doors

            tryPlace <| posu dx (room.Y - 1u)
            tryPlace <| posu dx (room.Y + room.Height)

        for dy in room.Y .. room.Y + room.Height do
            let tryPlace pos =
                if canPlaceDoorX walls pos then
                    doors <- pos :: doors

            tryPlace <| posu (room.X - 1u) dy
            tryPlace <| posu (room.X + room.Width) dy

    { Walls = walls
      Doors = List.map (fun d -> (d, chance 100)) doors
      Rooms = List.map (fun r -> r :> Room) rooms }
