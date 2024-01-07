module TinyRogue.Engine.Core

open TinyRogue.Engine.Actions
open TinyRogue.Engine.ProcGen
open TinyRogue.Engine.Types
open TinyRogue.Engine.Engine
open TinyRogue.Engine.Types.Entities

let UpdateEngine (engine: GameEngine) =
    { engine with Turn = engine.Turn + 1u } |> updateFieldOfView

let ExecuteNpcActions (engine: GameEngine) (actor: Actor) = apply engine actor (Move(1, 0))

let ExecutePlayerAction (engine: GameEngine, action: Action) =
    let player = List.find Actor.IsPlayer engine.Actors

    let engine, executed = apply engine player action

    let engine, npcActions =
        List.fold<Actor, GameEngine * ExecutedAction list>
            (fun (engine, actions) npc ->
                let move = [ Move(0, 1); Move(0, -1); Move(1, 0); Move(-1, 0); Skip; Skip ] |> Random.randomItem
                let engine, ex = apply engine npc move
                (engine, actions @ [ ex ]))
            (engine, [])
            (engine.Actors |> List.filter (Actor.IsPlayer >> not))

    UpdateEngine engine, executed :: npcActions
