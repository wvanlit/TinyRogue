module TinyRogue.Engine.ProcGen.DungeonGeneration

open TinyRogue.Engine.Map
open TinyRogue.Engine.Types
open TinyRogue.Engine.ProcGen.BinarySpacePartition

let createSimpleDungeon (width: uint) (height: uint) : Dungeon =
    let walls = Array2D.create (int height) (int width) false

    for bsp in generateBsp (width) (height) do
        printfn $"Room: [{bsp.x} {bsp.y}] [{bsp.width} {bsp.height}]"
        rect walls (int bsp.width) (int bsp.height) (int bsp.x) (int bsp.y)

    { Walls = walls }
