module TinyRogue.Engine.Actor

open TinyRogue.Engine.Types

let createPlayer pos : Actor = { id = 0u; role = Player; position = pos }

let isPlayer a = a.role = Player
