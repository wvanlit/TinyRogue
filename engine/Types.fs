module TinyRogue.Engine.Types

type Position = { x: int; y: int }
let pos x y = { x = x; y = y }
let posu (x: uint) (y: uint) = { x = int x; y = int y }

type Role =
    | Player
    | NPC

type ActorId = uint

type Actor =
    { id: ActorId
      role: Role
      position: Position }

type BitGrid = bool array2d
type Door = Position * bool

[<AbstractClass>]
type Room(x: uint, y: uint) =
    member val X = x with get
    member val Y = y with get

    abstract member Area: uint
    abstract member Center: uint * uint
    
    abstract member RandomPointInRoom: unit -> Position

type Visibility = | Visible | Explored | Unexplored

type ShadowMap = Visibility array2d

type Dungeon =
    {
      Walls: BitGrid
      Doors: Door list
      Rooms: Room list  }

type TinyRogueEngine =
    { Turn: uint
      Actors: Actor list
      Dungeon: Dungeon
      ShadowMap: ShadowMap }
