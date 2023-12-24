module TinyRogue.Engine.Types

type Position = { x: int; y: int }

type Role =
    | Player
    | NPC

type Actor = { id: uint; role: Role; position: Position }

type BitGrid = bool array2d

type Dungeon = { Walls: BitGrid }

type TinyRogueEngine =
    { Turn: uint
      Actors: Actor list
      Dungeon: Dungeon }
