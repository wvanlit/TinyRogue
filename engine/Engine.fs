module TinyRogue.Engine.Engine

open TinyRogue.Engine.Types
open TinyRogue.Engine.Actor
open TinyRogue.Engine.ProcGen.DungeonGeneration


let CreateEngine () =
    { Turn = 0u
      Actors = [ createPlayer 16 16 ]
      Dungeon = createSimpleDungeon 64u 42u }

let replaceActorWith engine actor =
    { engine with
        Actors = List.filter (fun a -> a.id <> actor.id) engine.Actors @ [ actor ] }
