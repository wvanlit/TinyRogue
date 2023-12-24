module TinyRogue.Engine.Actor

open TinyRogue.Engine.Types


let pos x y = { x = x; y = y }

let createPlayer x y : Actor = { id = 0u; role = Player; position = pos x y }

let isPlayer a = a.role = Player
