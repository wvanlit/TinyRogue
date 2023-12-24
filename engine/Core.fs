module TinyRogue.Engine.Core

open TinyRogue.Engine.Actions
open TinyRogue.Engine.Actor
open TinyRogue.Engine.Types

let UpdateEngine (engine: TinyRogueEngine) = { engine with Turn = engine.Turn + 1u }

let ExecutePlayerAction (engine: TinyRogueEngine, action: Action) =
    let player = List.find isPlayer engine.Actors

    { apply engine player action with
        Turn = engine.Turn + 1u } // Also increase turn
