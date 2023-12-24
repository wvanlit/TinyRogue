module TinyRogue.Engine.Actor

type Position = { x: int; y: int }

type Role =
    | Player
    | NPC

type Actor = { id: uint; role: Role; position: Position }


let pos x y = { x = x; y = y }

let createPlayer x y : Actor = { id = 0u; role = Player; position = pos x y }

let isPlayer a = a.role = Player
