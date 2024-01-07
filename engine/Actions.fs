module TinyRogue.Engine.Actions

open TinyRogue.Engine.Engine
open TinyRogue.Engine.Map
open TinyRogue.Engine.Types

type Action =
    | Move of x: int * y: int
    | Attack of x: int * y: int
    | Skip

type ExecutedAction =
    | NoAction
    | Moved of actor: ActorId * position: Position
    | DealtDamage of attacker: ActorId * defender: ActorId * damage: uint

let rec apply (engine: TinyRogueEngine) (actor: Actor) (action: Action) =
    match action with
    | Move(x, y) ->
        let newPosition =
            { x = x + actor.position.x
              y = y + actor.position.y }

        let canMove = canMoveTo engine.Dungeon newPosition.x newPosition.y
        let target = engine.Actors |> List.tryFind (fun a -> a.position.x = newPosition.x && a.position.y = newPosition.y)

        if canMove && target.IsNone then
            (replaceActorWith engine { actor with position = newPosition }, Moved(actor.id, newPosition))
        elif canMove && target.IsSome then
            // Moving to a tile with an actor on it is an attack
            apply engine actor (Attack(x, y))
        else
            // Can't move to the target tile
            (engine, NoAction)
    | Skip -> (engine, NoAction)
    | Attack(x, y) ->
        let newPosition =
            { x = x + actor.position.x
              y = y + actor.position.y }
        let target = engine.Actors |> List.tryFind (fun a -> a.position.x = newPosition.x && a.position.y = newPosition.y)

        match target with
        | None -> (engine, NoAction)
        | Some defender ->
            // TODO: Add proper damage calculation based on actor stats later
            (engine |> removeActor defender.id, DealtDamage(actor.id, defender.id, 1u))
