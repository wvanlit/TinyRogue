module TinyRogue.Engine.Actions

open TinyRogue.Engine.Engine
open TinyRogue.Engine.Map
open TinyRogue.Engine.Types

type Action =
    | Move of x: int * y: int
    | Skip

type ExecutedAction =
    | None
    | Moved of actor: uint * position: Position

let apply (engine: TinyRogueEngine) (actor: Actor) (action: Action) =
    match action with
    | Move(x, y) ->
        let newPosition =
            { x = x + actor.position.x
              y = y + actor.position.y }

        if
            canMoveTo engine.Dungeon newPosition.x newPosition.y
            && (List.tryFind (fun a -> a.position = newPosition) engine.Actors).IsNone
        then
            (replaceActorWith engine { actor with position = newPosition }, Moved(actor.id, newPosition))
        else
            (engine, None)
    | Skip -> (engine, None)
