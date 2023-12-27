module TinyRogue.Engine.Core

open TinyRogue.Engine.Actions
open TinyRogue.Engine.Actor
open TinyRogue.Engine.ProcGen
open TinyRogue.Engine.Types

let UpdateEngine (engine: TinyRogueEngine) = { engine with Turn = engine.Turn + 1u }

let ExecuteNpcActions (engine: TinyRogueEngine) (actor: Actor) = apply engine actor (Move(1, 0))

let ExecutePlayerAction (engine: TinyRogueEngine, action: Action) =
    let player = List.find isPlayer engine.Actors

    let engine, executed = apply engine player action

    let engine, npcActions =
        List.fold<Actor, TinyRogueEngine * ExecutedAction list>
            (fun (engine, actions) npc ->
                let move = [ Move(0, 1); Move(0, -1); Move(1, 0); Move(-1, 0) ] |> Random.randomItem
                let engine, ex = apply engine npc move
                (engine, actions @ [ ex ]))
            (engine, [])
            (engine.Actors |> List.filter (isPlayer >> not))

    UpdateEngine engine, executed :: npcActions
