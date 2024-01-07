module TinyRogue.Engine.Actions

open TinyRogue.Engine.Engine
open TinyRogue.Engine.Map
open TinyRogue.Engine.Types
open TinyRogue.Engine.Utils

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
        let newPosition = actor.position.offset(x, y)
        let canMove = canMoveTo engine.Dungeon newPosition

        let target =
            engine.Actors
            |> List.tryFind (fun a -> a.position.x = newPosition.x && a.position.y = newPosition.y)

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
        let newPosition = actor.position.offset(x, y)

        let target =
            engine.Actors
            |> List.tryFind (fun a -> a.position.x = newPosition.x && a.position.y = newPosition.y)

        match target with
        | None -> (engine, NoAction)
        | Some defender ->
            // TODO: Add proper damage calculation based on actor stats later
            let damageEvent = DealtDamage(actor.id, defender.id, 1u)

            // Cannot remove player from list
            (engine |> unless (defender.id = 0u) (removeActor defender.id), damageEvent)
