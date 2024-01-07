module TinyRogue.Engine.Types

module Primitives =
    type Position =
        struct
            val x: int
            val y: int

            new(_x: int, _y: int) = { x = _x; y = _y }
            new(_x: uint, _y: uint) = { x = int _x; y = int _y }

            member p.offset(_x: int, _y: int) = Position(p.x + _x, p.y + _y)
        end

    type EntityId = uint

open Primitives

module Entities =
    type Role =
        | Player
        | NPC

    [<Literal>]
    let playerId = 0u

    type Actor =
        struct
            val id: EntityId
            val role: Role
            val position: Position

            private new(id: EntityId, role: Role, pos: Position) = { id = id; role = role; position = pos }

            static member Player pos = Actor(playerId, Player, pos)
            static member Npc id pos = Actor(id, NPC, pos)

            member a.move pos = Actor(a.id, a.role, pos)

            static member IsPlayer(a: Actor) = a.role = Player
        end

open Entities

type BitGrid = bool array2d
type Door = Position * bool

[<AbstractClass>]
type Room(x: uint, y: uint) =
    member val X = x with get
    member val Y = y with get

    abstract member Area: uint
    abstract member Center: uint * uint

    abstract member RandomPointInRoom: unit -> Position

type Visibility =
    | Visible
    | Explored
    | Unexplored

type ShadowMap = Visibility array2d

type Dungeon =
    { Walls: BitGrid
      Doors: Door list
      Rooms: Room list }

type GameEngine =
    { Turn: uint
      Actors: Actor list
      Dungeon: Dungeon
      ShadowMap: ShadowMap }
