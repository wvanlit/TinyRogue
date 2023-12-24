module TinyRogue.Engine.Actions

open TinyRogue.Engine.Actor
open TinyRogue.Engine.Engine
open TinyRogue.Engine.Map

type Action =
    | Move of x: int * y: int
    | Skip

let apply (engine: TinyRogueEngine) (actor: Actor) (action: Action) =
    match action with
    | Move(x, y) ->
        let newPosition =
            { x = x + actor.position.x
              y = y + actor.position.y }

        if canMoveTo engine.Dungeon newPosition.x newPosition.y then
            replaceActorWith engine { actor with position = newPosition }
        else
            engine
    | Skip -> engine
